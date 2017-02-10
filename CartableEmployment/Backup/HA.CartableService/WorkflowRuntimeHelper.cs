using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Runtime;
using System.Web;
using System.Workflow.Activities;
using HA.Workflow.Services.Services;

namespace HA.CartableService
{
    public static class WorkflowRuntimeHelper
    {
        private const string WorkflowRuntimeKey = "WFRocks";
        private static object _SyncRuntiem = new object();

        public static WorkflowRuntime WorkflowRuntime
        {
            get
            {
                lock (_SyncRuntiem)
                {

                    WorkflowRuntime workflowRuntime = System.Web.HttpContext.Current.Application[WorkflowManager.WorkflowRuntimeKey] as WorkflowRuntime;
                    if (workflowRuntime == null)
                    {
                        #region Initialize WorkflowRuntime is in Global.asax
                        //// create an instance of workflow runtime, loading settings from web.config
                        //workflowRuntime = new WorkflowRuntime(WorkflowManager.WorkflowRuntimeKey);

                        //// add the external data exchange service to the runtime to allow for local services
                        //ExternalDataExchangeService exchangeService = new ExternalDataExchangeService(WorkflowManager.LocalServicesKey);
                        //workflowRuntime.AddService(exchangeService);

                        ////Add custom Condition service
                        //CustomConditionsService customConditionService = new CustomConditionsService();
                        //workflowRuntime.AddService(customConditionService);

                        ////Add Sub Workflow Service
                        //SubWorkflowService subWorkflowService = new SubWorkflowService();
                        //workflowRuntime.AddService(subWorkflowService);

                        //// start the workflow runtime
                        //workflowRuntime.StartRuntime();

                        //HttpContext.Current.Cache[WorkflowRuntimeKey] = workflowRuntime; 
                        #endregion
                        throw new Exception("There is not any WorkflowRuntime in the Domain application");
                    }

                    return workflowRuntime;

                }

            }
        }
    }
}
