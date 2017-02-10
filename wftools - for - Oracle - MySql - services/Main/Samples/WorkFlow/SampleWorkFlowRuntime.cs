using System;
using System.Collections.Generic;
using System.Configuration;
using System.Workflow.Activities;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Runtime.Tracking;
using WFTools.Samples.Windows;
using WFTools.Samples.WorkFlow;
using WFTools.Samples.WorkFlow.Services;
using WFTools.Services.Batching.Ado;
using WFTools.Services.Persistence.Ado;
using WFTools.Services.Tracking.Ado;
using WFTools.Utilities.Workflow;

namespace WFTools.Samples.WorkFlow
{
    /// <summary>
    /// Wrapper around the workflow runtime with helper methods
    /// for retrieving the sample workflows.
    /// </summary>
    public class SampleWorkflowRuntime : WorkflowRuntimeWrapperBase
    {
        private SampleWorkflowRuntime(DatabaseSettings databaseSettings)
        {
            if (databaseSettings == null)
                throw new ArgumentNullException("databaseSettings");

            if (!databaseSettings.HasPersistenceConnectionString)
                throw new ArgumentException("No persistence connection string specified.");

            _persistenceConnectionString = databaseSettings.PersistenceConnectionString;
            _trackingConnectionString = databaseSettings.TrackingConnectionString;
            _useLocalTransactions = databaseSettings.UseLocalTransactions;
        }

        private static readonly Object currentLock = new Object();

        private static SampleWorkflowRuntime current;
        /// <summary>
        /// Gets the currently active workflow runtime.
        /// </summary>
        /// <remarks>
        /// <see cref="Start" /> should be called before attempting
        /// to access the currently executing runtime.
        /// </remarks>
        public static SampleWorkflowRuntime Current
        {
            get { return current; }
        }

        /// <summary>
        /// Starts the workflow runtime.
        /// </summary>
        public static void Start(DatabaseSettings databaseSettings)
        {
            if (current != null)
                throw new ApplicationException("Runtime already created.");

            lock (currentLock)
            {
                current = new SampleWorkflowRuntime(databaseSettings);
                current.InitialiseAndStartWorkflowRuntime();
            }
        }

        /// <summary>
        /// Stops the workflow runtime.
        /// </summary>
        public static void Stop()
        {
            if (current == null)
                throw new ApplicationException("Runtime not created.");

            lock (currentLock)
            {
                current.StopWorkflowRuntime();
                current = null;
            }
        }

        private readonly ConnectionStringSettings _persistenceConnectionString;
        /// <summary>
        /// Connection String settings for the persistence service.
        /// </summary>
        protected ConnectionStringSettings PersistenceConnectionString
        {
            get { return _persistenceConnectionString; }
        }

        private readonly ConnectionStringSettings _trackingConnectionString;
        /// <summary>
        /// Connection String settings for the tracking service.
        /// </summary>
        protected ConnectionStringSettings TrackingConnectionString
        {
            get { return _trackingConnectionString; }
        }

        private readonly Boolean _useLocalTransactions;
        /// <summary>
        /// Indicates whether to use local transactions when using an AdoWorkBatchService.
        /// </summary>
        protected Boolean UseLocalTransactions
        {
            get { return _useLocalTransactions; }
        }

        /// <summary>
        /// Use ADO persistence service.
        /// </summary>
        protected override WorkflowPersistenceService PersistenceService
        {
            get
            {
                return new AdoPersistenceService(
                    PersistenceConnectionString, true, TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30));
            }
        }

        /// <summary>
        /// Use ADO tracking service.
        /// </summary>
        protected override TrackingService TrackingService
        {
            get
            {
                if (TrackingConnectionString != null)
                    return new AdoTrackingService(TrackingConnectionString);

                return null;
            }
        }

        /// <summary>
        /// Use the ADO work batch service.
        /// </summary>
        protected override WorkflowCommitWorkBatchService WorkBatchService
        {
            get
            {
                return new AdoWorkBatchService(UseLocalTransactions);
            }
        }

        /// <summary>
        /// Use manual workflow scheduling service.
        /// </summary>
        protected override WorkflowSchedulerService SchedulerService
        {
            get
            {
                return new ManualWorkflowSchedulerService(true);
            }
        }

        private List<Object> externalServices;
        /// <summary>
        /// List of services that should be added to an <see cref="ExternalDataExchangeService" />.
        /// <remarks>
        /// This includes the <see cref="StateMachineService" />.
        /// </remarks>
        /// </summary>
        protected override IList<Object> ExternalServices
        {
            get
            {
                if (externalServices == null)
                {
                    externalServices = new List<Object> {new StateMachineService()};
                }

                return externalServices;
            }
        }

        /// <summary>
        /// Create an sequential workflow.
        /// </summary>
        public WorkflowInstance CreateSequentialWorkflow()
        {
            return CreateWorkflow(typeof(SequentialWorkFlow));
        }

        /// <summary>
        /// Create a sequential workflow with the specified instance identifier.
        /// </summary>
        public WorkflowInstance CreateSequentialWorkflow(Guid instanceGuid)
        {
            return CreateWorkflow(typeof(SequentialWorkFlow), instanceGuid);
        }

        /// <summary>
        /// Create a sequential workflow with the specified instance identifier.
        /// </summary>
        public WorkflowInstance CreateSequentialWorkflow(Guid instanceGuid, Dictionary<String, Object> parameters)
        {
            return CreateWorkflow(typeof(SequentialWorkFlow), instanceGuid, parameters);
        }

        /// <summary>
        /// Create an article workflow for the specified article.
        /// </summary>
        public WorkflowInstance CreateStateMachineWorkflow()
        {
            return CreateWorkflow(typeof(StateMachineWorkflow));
        }
    }
}