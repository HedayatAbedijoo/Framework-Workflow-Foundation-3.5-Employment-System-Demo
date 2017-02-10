using System;
using System.Collections.Generic;
using System.Configuration;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using WFTools.Services.Persistence.Ado;
using WFTools.Utilities;

namespace WFTools.Samples.WorkFlow
{
    /// <summary>
    /// Wrapper around the workflow runtime with helper methods
    /// for retrieving the sample workflows.
    /// </summary>
	public class SampleWorkFlowRuntime : WorkflowRuntimeWrapperBase
	{
        public SampleWorkFlowRuntime(ConnectionStringSettings connectionStringSettings)
        {
            this.connectionStringSettings = connectionStringSettings;

            InitialiseAndStartWorkflowRuntime();
        }

        private ConnectionStringSettings connectionStringSettings;
        /// <summary>
        /// Return the default connection string settings.
        /// </summary>
        protected ConnectionStringSettings ConnectionStringSettings
        {
            get { return connectionStringSettings; }
        }

        /// <summary>
        /// Use ADO workflow persistence service.
        /// </summary>
        protected override WorkflowPersistenceService PersistenceService
        {
            get
            {
                return new AdoWorkflowPersistenceService(
                    ConnectionStringSettings, true, TimeSpan.FromMinutes(1), 
                    TimeSpan.FromMinutes(1));
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

        /// <summary>
        /// Create an sequential workflow.
        /// </summary>
        public WorkflowInstance CreateSequentialWorkflow()
        {
            return CreateWorkflow(typeof (SequentialWorkFlow));
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
        public WorkflowInstance CreateSequentialWorkflow(Guid instanceGuid, Dictionary<string, object> parameters)
        {
            return CreateWorkflow(typeof (SequentialWorkFlow), instanceGuid, parameters);
        }
    }
}