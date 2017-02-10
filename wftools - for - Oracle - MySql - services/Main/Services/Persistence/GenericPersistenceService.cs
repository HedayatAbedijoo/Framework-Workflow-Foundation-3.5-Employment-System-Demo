using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using WFTools.Utilities.Diagnostics;

namespace WFTools.Services.Persistence
{
    /// <summary>
    /// Abstract implementation of <see cref="WorkflowPersistenceService" /> that 
    /// provides the framework necessary for a very generic persistence service. 
    /// 
    /// A resource provider and accessor work hand-in-hand to actually
    /// persist workflow data to the persistence store.
    /// <remarks>
    /// This implementation also supports automatic expiration of workflows
    /// making use of DelayActivity-type constructs.
    /// </remarks>
    /// </summary>
    public abstract class GenericPersistenceService : WorkflowPersistenceService, IPendingWork, IDisposable
    {
        /// <summary>
        /// Construct an instance of the persistence service using the 
        /// specified configuration parameters.
        /// </summary>
        /// <param name="unloadOnIdle">
        /// Indicates whether to unload idle workflow instances.
        /// </param>
        /// <param name="instanceOwnershipDuration">
        /// The timeout before the ownership of a workflow instance is invalidated.
        /// </param>
        /// <param name="loadInterval">
        /// The interval to check for workflow instances with expired timers.
        /// </param>
        protected GenericPersistenceService(Boolean unloadOnIdle, TimeSpan instanceOwnershipDuration, TimeSpan loadInterval)
        {
            _unloadOnIdle = unloadOnIdle;
            _instanceOwnershipDuration = instanceOwnershipDuration;
            _loadInterval = loadInterval;
            _serviceId = Guid.NewGuid();
            _timerLock = new object();
        }

        /// <summary>
        /// Indicates whether to unload idle workflow instances.
        /// </summary>
        private readonly Boolean _unloadOnIdle;

        /// <summary>
        /// Length of time before ownership of a workflow instance
        /// is automatically invalidated.
        /// </summary>
        private readonly TimeSpan _instanceOwnershipDuration;

        /// <summary>
        /// Interval between checking for expired DelayActivity workflows.
        /// </summary>
        private readonly TimeSpan _loadInterval;

        /// <summary>
        /// Unique identifier of this service.
        /// </summary>
        private readonly Guid _serviceId;

        /// <summary>
        /// Lock used when updating the timer.
        /// </summary>
        private readonly object _timerLock;

        /// <summary>
        /// Timer used to load workflows with expired timer activities.
        /// </summary>
        private SmartTimer _smartTimer;

        /// <summary>
        /// Helper property that indicates when the ownership of a particular
        /// workflow instance will expire.
        /// </summary>
        private DateTime ownershipTimeout
        {
            get
            {
                if (_instanceOwnershipDuration == TimeSpan.MaxValue)
                    return DateTime.MaxValue;

                return (DateTime.UtcNow + _instanceOwnershipDuration);
            }
        }

        /// <summary>
        /// The active <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying persistence store.
        /// </summary>
        private IResourceProvider _resourceProvider;

        /// <summary>
        /// Create an <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying persistence store.
        /// </summary>
        /// <remarks>
        /// The resource provider is created once upon Start of the 
        /// persistence service.
        /// </remarks>
        protected abstract IResourceProvider CreateResourceProvider();

        /// <summary>
        /// Create an <see cref="IPersistenceResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected abstract IPersistenceResourceAccessor CreateAccessor(IResourceProvider resourceProvider);

        /// <summary>
        /// Create an <see cref="IPersistenceResourceAccessor" /> that is responsible
        /// for manipulating the underlying persistence store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <param name="transaction">
        /// Transaction to perform operations within.
        /// </param>
        protected abstract IPersistenceResourceAccessor CreateAccessor(IResourceProvider resourceProvider, Transaction transaction);

        /// <summary>
        /// Perform startup duties associated with this persistence service.
        /// <remarks>
        /// This implementation calls a virtual method to create a single 
        /// resource provider for this persistence service.
        /// </remarks>
        /// </summary>
        protected override void Start()
        {
            TraceHelper.Trace();

            try
            {
                // retrieve the active resource provider
                _resourceProvider = CreateResourceProvider();

                base.Start();
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException = 
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, Guid.Empty);

                throw persistenceException;
            }
        }

        /// <summary>
        /// Perform shutdown duties associated with this persistence service.
        /// <remarks>
        /// This implementation disposes the internal <see cref="SmartTimer" />
        /// responsible for loading expired workflows into memory.
        /// </remarks>
        /// </summary>
        protected override void Stop()
        {
            TraceHelper.Trace();

            try
            {
                base.Stop();

                lock (_timerLock)
                {
                    if (_smartTimer != null)
                    {
                        _smartTimer.Dispose();
                        _smartTimer = null;
                    }
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException =
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, Guid.Empty);

                throw persistenceException;
            }
        }

        /// <summary>
        /// Kick-off the timer to check for workflows with expired timers, 
        /// and recover running workflow instances.
        /// </summary>
        protected override void OnStarted()
        {
            TraceHelper.Trace();

            try
            {
                base.OnStarted();

                if (_loadInterval > TimeSpan.Zero)
                {
                    lock (_timerLock)
                    {
                        _smartTimer = new SmartTimer(
                            loadWorkflowsWithExpiredTimers,
                            null, _loadInterval, _loadInterval);
                    }
                }

                recoverRunningWorkflows();
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException = 
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, Guid.Empty);

                throw persistenceException;
            }
        }

        /// <summary>
        /// Callback used by the <see cref="SmartTimer" /> to load workflows
        /// with expired timers.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exception is raised using base.RaiseServicesExceptionNotHandledEvent where the workflow runtime deals with it.")]
        private void loadWorkflowsWithExpiredTimers(object state)
        {
            lock (_timerLock)
            {
                if (State == WorkflowRuntimeServiceState.Started)
                {
                    IList<Guid> expiredWorkflowIds = null;
                    try
                    {
                        expiredWorkflowIds = LoadExpiredWorkflowIds();
                    }
                    catch (Exception e)
                    {
                        String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                        TraceHelper.Trace(errorMessage);

                        RaiseServicesExceptionNotHandledEvent(
                            new PersistenceException(errorMessage, e),
                            Guid.Empty);
                    }

                    if (expiredWorkflowIds != null)
                    {
                        // load each expired workflow into memory
                        foreach (Guid expiredWorkflowId in expiredWorkflowIds)
                        {
                            try
                            {
                                Runtime.GetWorkflow(expiredWorkflowId).Load();
                            }
                            catch (WorkflowOwnershipException)
                            {
                                continue;
                            }
                            catch (ObjectDisposedException)
                            {
                                throw;
                            }
                            catch (InvalidOperationException e)
                            {
                                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                                TraceHelper.Trace(errorMessage);

                                if (!e.Data.Contains("WorkflowNotFound"))
                                {
                                    RaiseServicesExceptionNotHandledEvent(
                                        new PersistenceException(errorMessage, e),
                                        expiredWorkflowId);
                                }

                                continue;
                            }
                            catch (Exception e)
                            {
                                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                                TraceHelper.Trace(errorMessage);

                                RaiseServicesExceptionNotHandledEvent(
                                    new PersistenceException(errorMessage, e),
                                    expiredWorkflowId);

                                continue;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recovers any running workflows that no longer have a valid owner.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exception is raised using base.RaiseServicesExceptionNotHandledEvent where the workflow runtime deals with it.")]
        private void recoverRunningWorkflows()
        {
            IList<Guid> workflowIds = LoadNonBlockedWorkflowIds();
            foreach (Guid workflowId in workflowIds)
            {
                try
                {
                    Runtime.GetWorkflow(workflowId).Load();
                }
                catch (Exception e)
                {
                    String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                    TraceHelper.Trace(errorMessage);

                    RaiseServicesExceptionNotHandledEvent(
                        new PersistenceException(errorMessage, e),
                        workflowId);

                    continue;
                }
            }
        }

        /// <summary>
        /// Retrieve a list of all workflow identifiers that no longer have
        /// a valid owner.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exception is raised using base.RaiseServicesExceptionNotHandledEvent where the workflow runtime deals with it.")]
        protected virtual IList<Guid> LoadNonBlockedWorkflowIds()
        {
            TraceHelper.Trace();

            IList<Guid> instanceIds;
            try
            {
                using (IPersistenceResourceAccessor resourceAccessor = CreateAccessor(_resourceProvider))
                {
                    instanceIds = resourceAccessor.RetrieveNonBlockedInstanceIds(_serviceId, ownershipTimeout);
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                RaiseServicesExceptionNotHandledEvent(
                    new PersistenceException(errorMessage, e),
                    Guid.Empty);

                instanceIds = new List<Guid>();
            }

            return instanceIds;
        }

        /// <summary>
        /// Retrieve a list of all expired workflow identifiers.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exception is raised using base.RaiseServicesExceptionNotHandledEvent where the workflow runtime deals with it.")]
        protected virtual IList<Guid> LoadExpiredWorkflowIds()
        {
            TraceHelper.Trace();

            IList<Guid> instanceIds;
            try
            {
                using (IPersistenceResourceAccessor resourceAccessor = CreateAccessor(_resourceProvider))
                {
                    instanceIds = resourceAccessor.RetrieveExpiredTimerIds(_serviceId, ownershipTimeout);
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                RaiseServicesExceptionNotHandledEvent(
                    new PersistenceException(errorMessage, e),
                    Guid.Empty);

                instanceIds = new List<Guid>();
            }

            return instanceIds;
        }

        /// <summary>
        /// Persist a workflow instance to the persistence store.
        /// </summary>
        /// <param name="rootActivity">
        /// Root activity of the workflow instance.
        /// </param>
        /// <param name="unlock">
        /// Indicates whether to unlock the instance when complete.
        /// </param>
        protected override void SaveWorkflowInstanceState(Activity rootActivity, Boolean unlock)
        {
            TraceHelper.Trace();

            try
            {
                if (rootActivity == null)
                    throw new ArgumentNullException("rootActivity");

                PendingWorkItem workItem = new PendingWorkItem();
                workItem.Status = GetWorkflowStatus(rootActivity);
                workItem.IsBlocked = GetIsBlocked(rootActivity);
                workItem.Info = GetSuspendOrTerminateInfo(rootActivity);
                workItem.StateId = (Guid) rootActivity.GetValue(Activity.ActivityContextGuidProperty);
                workItem.Type = PendingWorkItem.ItemType.Instance;
                workItem.InstanceId = WorkflowEnvironment.WorkflowInstanceId;
                workItem.Unlock = unlock;
                if (workItem.Status != WorkflowStatus.Completed && workItem.Status != WorkflowStatus.Terminated)
                    workItem.SerialisedActivity = GetDefaultSerializedForm(rootActivity);
                else
                    workItem.SerialisedActivity = new byte[0];

                TimerEventSubscription timerEventSubscription =
                    ((TimerEventSubscriptionCollection) rootActivity.GetValue(
                    TimerEventSubscriptionCollection.TimerCollectionProperty)).Peek();

                if (timerEventSubscription == null)
                    workItem.NextTimer = null;
                else
                    workItem.NextTimer = timerEventSubscription.ExpiresAt;

                WorkflowEnvironment.WorkBatch.Add(this, workItem);
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException =
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException,
                    WorkflowEnvironment.WorkflowInstanceId);

                throw persistenceException;
            }
        }

        /// <summary>
        /// Unlock a workflow instance.
        /// </summary>
        /// <param name="rootActivity">
        /// The root activity of the workflow instance.
        /// </param>
        protected override void UnlockWorkflowInstanceState(Activity rootActivity)
        {
            TraceHelper.Trace();

            try
            {
                PendingWorkItem workItem = new PendingWorkItem();
                workItem.Type = PendingWorkItem.ItemType.UnlockWorkflow;
                workItem.InstanceId = WorkflowEnvironment.WorkflowInstanceId;

                WorkflowEnvironment.WorkBatch.Add(this, workItem);
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException = 
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, 
                    WorkflowEnvironment.WorkflowInstanceId);

                throw persistenceException;
            }
        }

        ///<summary>
        /// Load the specified workflow instance into memory from the 
        /// persistence store.
        ///</summary>
        ///<param name="instanceId">
        /// The <see cref="T:System.Guid"></see> of the root activity of the workflow instance.
        /// </param>
        protected override Activity LoadWorkflowInstanceState(Guid instanceId)
        {
            TraceHelper.Trace();

            Activity rootActivity;
            try
            {
                using (IPersistenceResourceAccessor resourceAccessor = CreateAccessor(_resourceProvider))
                {
                    byte[] instanceState = resourceAccessor.RetrieveInstanceState(
                        instanceId, _serviceId, ownershipTimeout);

                    rootActivity = RestoreFromDefaultSerializedForm(instanceState, null);
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException = 
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, instanceId);

                throw persistenceException;
            }

            return rootActivity;
        }

        /// <summary>
        /// Persist a completed scope to the persistence store.
        /// </summary>
        /// <param name="activity">
        /// The completed scope to pesist.
        /// </param>
        protected override void SaveCompletedContextActivity(Activity activity)
        {
            TraceHelper.Trace();

            try
            {
                PendingWorkItem workItem = new PendingWorkItem();
                workItem.Type = PendingWorkItem.ItemType.CompletedScope;
                workItem.SerialisedActivity = GetDefaultSerializedForm(activity);
                workItem.InstanceId = WorkflowEnvironment.WorkflowInstanceId;
                workItem.StateId = (Guid)activity.GetValue(
                    Activity.ActivityContextGuidProperty);

                WorkflowEnvironment.WorkBatch.Add(this, workItem);
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException = 
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, 
                    WorkflowEnvironment.WorkflowInstanceId);

                throw persistenceException;
            }
        }

        /// <summary>
        /// Load a completed scope from the persistence store.
        /// </summary>
        /// <param name="scopeId">
        /// <see cref="Guid" /> representing the unique identifier of the completed scope.
        /// </param>
        /// <param name="outerActivity">
        /// The activity in which to load the completed scope into.
        /// </param>
        protected override Activity LoadCompletedContextActivity(Guid scopeId, Activity outerActivity)
        {
            TraceHelper.Trace();

            Activity contextActivity;
            try
            {
                using (IPersistenceResourceAccessor resourceAccessor = CreateAccessor(_resourceProvider))
                {
                    byte[] instanceState = resourceAccessor.RetrieveCompletedScope(scopeId);

                    contextActivity = RestoreFromDefaultSerializedForm(instanceState, outerActivity);
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException =
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, 
                    WorkflowEnvironment.WorkflowInstanceId);

                throw persistenceException;
            }

            return contextActivity;
        }

        /// <summary>
        /// Indicates whether to unload the specified activity when it is idle
        /// persisting them to the persistence store.
        /// </summary>
        /// <param name="activity">
        /// Activity considered for unloading.
        /// </param>
        protected override Boolean UnloadOnIdle(Activity activity)
        {
            TraceHelper.Trace();

            return _unloadOnIdle;
        }

        ///<summary>
        ///Allows the items in the work batch to assert whether they must commit immediately.
        ///</summary>
        ///
        ///<returns>
        ///true if any item in the collection must be committed immediately; otherwise false.
        ///</returns>
        ///
        ///<param name="items">An <see cref="T:System.Collections.ICollection"></see> of work items.</param>
        public Boolean MustCommit(ICollection items)
        {
            TraceHelper.Trace();

            return true;
        }

        ///<summary>
        ///Commits the list of work items by using the specified <see cref="T:System.Transactions.Transaction"></see> object.
        ///</summary>
        ///
        ///<param name="items">The work items to be committed.</param>
        ///<param name="transaction">The <see cref="T:System.Transactions.Transaction"></see> associated with the pending work.</param>
        public void Commit(Transaction transaction, ICollection items)
        {
            TraceHelper.Trace();

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            if (items == null)
                throw new ArgumentNullException("items");

            try
            {
                using (IPersistenceResourceAccessor resourceAccessor = CreateAccessor(_resourceProvider, transaction))
                {
                    foreach (PendingWorkItem item in items)
                    {
                        switch (item.Type)
                        {
                            case PendingWorkItem.ItemType.Instance:
                                resourceAccessor.InsertInstanceState(item,
                                    _serviceId, ownershipTimeout);

                                continue;

                            case PendingWorkItem.ItemType.CompletedScope:
                                resourceAccessor.InsertCompletedScope(
                                    item.InstanceId, item.StateId,
                                    item.SerialisedActivity);

                                continue;

                            case PendingWorkItem.ItemType.UnlockWorkflow:
                                resourceAccessor.UnlockInstanceState(
                                    item.InstanceId, _serviceId);

                                continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException =
                    new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, 
                    Guid.Empty);

                throw persistenceException;
            }
        }

        ///<summary>
        ///Called when the transaction has completed.
        ///</summary>
        ///
        ///<param name="items">An <see cref="T:System.Collections.ICollection"></see> of work items.</param>
        ///<param name="succeeded">true if the transaction succeeded; otherwise, false.</param>
        public void Complete(Boolean succeeded, ICollection items)
        {
            TraceHelper.Trace();

            try
            {
                if (succeeded)
                {
                    foreach (PendingWorkItem item in items)
                    {
                        if (item.Type == PendingWorkItem.ItemType.Instance && item.NextTimer != null)
                            _smartTimer.Update(item.NextTimer.Value);
                    }
                }
            }
            catch (Exception e)
            {
                String errorMessage = RM.Get_Error_PersistenceServiceException(e.ToString());

                TraceHelper.Trace(errorMessage);

                PersistenceException persistenceException
                    = new PersistenceException(errorMessage, e);

                RaiseServicesExceptionNotHandledEvent(
                    persistenceException, Guid.Empty);

                throw persistenceException;
            }
        }

        ///<summary>
        /// Clean-up the timers used by the persistence service.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///<summary>
        /// Clean-up the timers used by the persistence service.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                lock (_timerLock)
                {
                    if (_smartTimer != null)
                    {
                        _smartTimer.Dispose();
                        _smartTimer = null;
                    }
                }
            }
        }
    }
}