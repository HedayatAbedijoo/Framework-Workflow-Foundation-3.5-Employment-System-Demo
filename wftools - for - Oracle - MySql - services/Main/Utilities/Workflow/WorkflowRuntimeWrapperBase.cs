using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Workflow.Activities;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Utilities.Workflow
{
    /// <summary>
    /// Wrapper that provides helpful functionality in creating and
    /// managing the lifetime of a workflow runtime.
    /// </summary>
    public abstract class WorkflowRuntimeWrapperBase : IDisposable
    {
        private WorkflowRuntime workflowRuntime;
        /// <summary>
        /// The instance of the workflow runtime.
        /// </summary>
        public WorkflowRuntime WorkflowRuntime
        {
            get { return workflowRuntime; }
        }

        /// <summary>
        /// Persistence service that should be used for the runtime.
        /// </summary>
        protected virtual WorkflowPersistenceService PersistenceService
        {
            get { return null; }
        }

        /// <summary>
        /// Tracking service that should be used for the runtime.
        /// </summary>
        protected virtual TrackingService TrackingService
        {
            get { return null; }
        }

        /// <summary>
        /// The service used for scheduling runtime events.
        /// </summary>
        /// <remarks>
        /// Derived classes should use the <see cref="ManualWorkflowSchedulerService" /> 
        /// under ASP.NET.
        /// </remarks>
        protected virtual WorkflowSchedulerService SchedulerService
        {
            get { return null; }
        }

        /// <summary>
        /// The service used to batch up and commit workflow actions.
        /// </summary>
        protected virtual WorkflowCommitWorkBatchService WorkBatchService
        {
            get { return null; }
        }

        /// <summary>
        /// List of services that should be added to an <see cref="ExternalDataExchangeService" />.
        /// </summary>
        protected virtual IList<Object> ExternalServices
        {
            get { return null; }
        }

        /// <summary>
        /// List of custom local services that should be added to the workflow runtime.
        /// </summary>
        protected virtual IList<Object> CustomServices
        {
            get { return null; }
        }

        /// <summary>
        /// Initialises and starts a workflow runtime with services specified
        /// by the derived implementation.
        /// </summary>
        protected void InitialiseAndStartWorkflowRuntime()
        {
            workflowRuntime = new WorkflowRuntime();

            if (SchedulerService != null)
                workflowRuntime.AddService(SchedulerService);

            if (PersistenceService != null)
                workflowRuntime.AddService(PersistenceService);

            if (TrackingService != null)
                workflowRuntime.AddService(TrackingService);

            if (WorkBatchService != null)
                workflowRuntime.AddService(WorkBatchService);

            if (ExternalServices != null && ExternalServices.Count > 0)
            {
                ExternalDataExchangeService externalDataExchangeService = new ExternalDataExchangeService();
                workflowRuntime.AddService(externalDataExchangeService);

                foreach (Object externalService in ExternalServices)
                    externalDataExchangeService.AddService(externalService);
            }

            if (CustomServices != null && CustomServices.Count > 0)
            {
                foreach (Object customService in CustomServices)
                    workflowRuntime.AddService(customService);
            }

            workflowRuntime.Started += workflowRuntime_Started;
            workflowRuntime.Stopped += workflowRuntime_Stopped;
            workflowRuntime.ServicesExceptionNotHandled += workflowRuntime_ServicesExceptionNotHandled;
            workflowRuntime.WorkflowAborted += workflowRuntime_WorkflowAborted;
            workflowRuntime.WorkflowCompleted += workflowRuntime_WorkflowCompleted;
            workflowRuntime.WorkflowCreated += workflowRuntime_WorkflowCreated;
            workflowRuntime.WorkflowIdled += workflowRuntime_WorkflowIdled;
            workflowRuntime.WorkflowLoaded += workflowRuntime_WorkflowLoaded;
            workflowRuntime.WorkflowPersisted += workflowRuntime_WorkflowPersisted;
            workflowRuntime.WorkflowResumed += workflowRuntime_WorkflowResumed;
            workflowRuntime.WorkflowStarted += workflowRuntime_WorkflowStarted;
            workflowRuntime.WorkflowSuspended += workflowRuntime_WorkflowSuspended;
            workflowRuntime.WorkflowTerminated += workflowRuntime_WorkflowTerminated;
            workflowRuntime.WorkflowUnloaded += workflowRuntime_WorkflowUnloaded;

            workflowRuntime.StartRuntime();
        }

        /// <summary>
        /// Stops the workflow runtime.
        /// </summary>
        protected void StopWorkflowRuntime()
        {
            if (workflowRuntime != null)
                workflowRuntime.StopRuntime();
        }

        /// <summary>
        /// Create a new workflow.
        /// </summary>
        public WorkflowInstance CreateWorkflow(Type workflowType)
        {
            return workflowRuntime.CreateWorkflow(workflowType);
        }

        /// <summary>
        /// Create a new workflow with the specified instance identifier.
        /// </summary>
        public WorkflowInstance CreateWorkflow(Type workflowType, Guid instanceGuid)
        {
            return workflowRuntime.CreateWorkflow(workflowType, new Dictionary<string, Object>(), instanceGuid);
        }

        /// <summary>
        /// Create a new workflow with the specified instance identifier 
        /// and parameters.
        /// </summary>
        public WorkflowInstance CreateWorkflow(Type workflowType, Guid instanceGuid, Dictionary<string, Object> parameters)
        {
            return workflowRuntime.CreateWorkflow(workflowType, parameters, instanceGuid);
        }

        /// <summary>
        /// Retrieve an existing workflow.
        /// </summary>
        public WorkflowInstance GetWorkflow(Guid instanceId)
        {
            return workflowRuntime.GetWorkflow(instanceId);
        }

        /// <summary>
        /// Retrieve a service from the workflow runtime.
        /// </summary>
        public T GetService<T>()
        {
            return workflowRuntime.GetService<T>();
        }

        /// <summary>
        /// Runs a <see cref="WorkflowInstance" /> using the <see cref="ManualWorkflowSchedulerService" />
        /// with error handling support if available.
        /// </summary>
        /// <param name="instanceId">
        /// Unique identifier of a workflow instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the workflow is running, <c>false</c> otherwise.
        /// </returns>
        public Boolean RunWorkflow(Guid instanceId)
        {
            ManualWorkflowSchedulerService workflowScheduler = GetService<ManualWorkflowSchedulerService>();
            if (workflowScheduler == null)
            {
                // manual scheduler is not present, we do not need to 
                // manually run the workflow
                return true;
            }

            Exception workflowException = null;
            EventHandler<WorkflowTerminatedEventArgs> terminatedHandler = null;
            terminatedHandler =
                delegate(object sender, WorkflowTerminatedEventArgs e)
                {
                    if (e.WorkflowInstance.InstanceId == instanceId && e.Exception != null)
                    {
                        workflowRuntime.WorkflowTerminated -= terminatedHandler;
                        workflowException = e.Exception;
                    }
                };

            workflowRuntime.WorkflowTerminated += terminatedHandler;

            ErrorHandlingService errorHandlingService = GetService<ErrorHandlingService>();
            if (errorHandlingService != null)
            {
                EventHandler<ErrorHandlingEventArgs> errorHandler = null;
                errorHandler =
                    delegate(object sender, ErrorHandlingEventArgs e)
                        {
                            if (e.InstanceId == instanceId && e.Exception != null)
                            {
                                errorHandlingService.Error -= errorHandler;
                                workflowException = e.Exception;
                            }
                        };
                
                errorHandlingService.Error += errorHandler;
            }

            Boolean isRunning = workflowScheduler.RunWorkflow(instanceId);
            if (workflowException != null)
                throw workflowException;

            return isRunning;
        }

        /// <summary>
        /// Retrieve the <see cref="ErrorHandlingService" /> from
        /// the workflow runtime.
        /// </summary>
        /// <returns>
        /// A <see cref="ErrorHandlingService" /> implementation.
        /// </returns>
        public ErrorHandlingService GetErrorHandlingService()
        {
            return GetService<ErrorHandlingService>();
        }

        public void Dispose()
        {
            if (workflowRuntime != null)
            {
                workflowRuntime.StopRuntime();
                workflowRuntime.Dispose();
                workflowRuntime = null;
            }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.Started" />.
        /// </summary>
        public event EventHandler<WorkflowRuntimeEventArgs> Started
        {
            add { workflowRuntime.Started += value; }
            remove { workflowRuntime.Started -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.Stopped" />.
        /// </summary>
        public event EventHandler<WorkflowRuntimeEventArgs> Stopped
        {
            add { workflowRuntime.Stopped += value; }
            remove { workflowRuntime.Stopped -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.ServicesExceptionNotHandled" />.
        /// </summary>
        public event EventHandler<ServicesExceptionNotHandledEventArgs> ServicesExceptionNotHandled
        {
            add { workflowRuntime.ServicesExceptionNotHandled += value; }
            remove { workflowRuntime.ServicesExceptionNotHandled -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowAborted" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowAborted
        {
            add { workflowRuntime.WorkflowAborted += value; }
            remove { workflowRuntime.WorkflowAborted -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowCompleted" />.
        /// </summary>
        public event EventHandler<WorkflowCompletedEventArgs> WorkflowCompleted
        {
            add { workflowRuntime.WorkflowCompleted += value; }
            remove { workflowRuntime.WorkflowCompleted -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowCreated" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowCreated
        {
            add { workflowRuntime.WorkflowCreated += value; }
            remove { workflowRuntime.WorkflowCreated -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowIdled" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowIdled
        {
            add { workflowRuntime.WorkflowIdled += value; }
            remove { workflowRuntime.WorkflowIdled -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowLoaded" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowLoaded
        {
            add { workflowRuntime.WorkflowLoaded += value; }
            remove { workflowRuntime.WorkflowLoaded -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowPersisted" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowPersisted
        {
            add { workflowRuntime.WorkflowPersisted += value; }
            remove { workflowRuntime.WorkflowPersisted -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowResumed" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowResumed
        {
            add { workflowRuntime.WorkflowResumed += value; }
            remove { workflowRuntime.WorkflowResumed -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowStarted" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowStarted
        {
            add { workflowRuntime.WorkflowStarted += value; }
            remove { workflowRuntime.WorkflowStarted -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowSuspended" />.
        /// </summary>
        public event EventHandler<WorkflowSuspendedEventArgs> WorkflowSuspended
        {
            add { workflowRuntime.WorkflowSuspended += value; }
            remove { workflowRuntime.WorkflowSuspended -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowTerminated" />.
        /// </summary>
        public event EventHandler<WorkflowTerminatedEventArgs> WorkflowTerminated
        {
            add { workflowRuntime.WorkflowTerminated += value; }
            remove { workflowRuntime.WorkflowTerminated -= value; }
        }

        /// <summary>
        /// Wrapper around <see cref="System.Workflow.Runtime.WorkflowRuntime.WorkflowUnloaded" />.
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowUnloaded
        {
            add { workflowRuntime.WorkflowUnloaded += value; }
            remove { workflowRuntime.WorkflowUnloaded -= value; }
        }

        private static void workflowRuntime_WorkflowCompleted(Object sender, WorkflowCompletedEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Completed", e.WorkflowInstance.InstanceId));
            foreach (string parameterName in e.OutputParameters.Keys)
            {
                Trace.WriteLine(string.Format(
                    "Workflow {0} Output Parameter Name: {0}, Value: {1}", parameterName,
                    e.OutputParameters[parameterName]));
            }
        }

        private static void workflowRuntime_WorkflowAborted(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Aborted", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowCreated(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Created", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowIdled(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Idled", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowLoaded(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Loaded", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowPersisted(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Persisted", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowResumed(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Resumed", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowStarted(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Started", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_WorkflowSuspended(Object sender, WorkflowSuspendedEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Suspended", e.WorkflowInstance.InstanceId));
            if (!string.IsNullOrEmpty(e.Error))
                Trace.WriteLine(string.Format("Workflow {0} Error: {1}", e.WorkflowInstance.InstanceId, e.Error));
        }

        private static void workflowRuntime_WorkflowTerminated(Object sender, WorkflowTerminatedEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Terminated", e.WorkflowInstance.InstanceId));
            if (e.Exception != null)
                Trace.WriteLine(string.Format("Workflow {0} Fault: {1}", e.WorkflowInstance.InstanceId, e.Exception));
        }

        private static void workflowRuntime_WorkflowUnloaded(Object sender, WorkflowEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow {0}: Unloaded", e.WorkflowInstance.InstanceId));
        }

        private static void workflowRuntime_Started(Object sender, WorkflowRuntimeEventArgs e)
        {
            Trace.WriteLine("Workflow Runtime: Started");
        }

        private static void workflowRuntime_Stopped(Object sender, WorkflowRuntimeEventArgs e)
        {
            Trace.WriteLine("Workflow Runtime: Stopped");
        }

        private static void workflowRuntime_ServicesExceptionNotHandled(Object sender, ServicesExceptionNotHandledEventArgs e)
        {
            Trace.WriteLine(string.Format("Workflow Runtime: Service Exception - {0}", e.Exception));
        }
    }
}
