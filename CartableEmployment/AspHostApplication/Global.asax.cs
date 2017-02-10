using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Workflow.Runtime;
using HA.CartableService;
using System.Workflow.Activities;
using HA.Workflow.Services.Services;

namespace AspHostApplication
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

            // create an instance of workflow runtime, loading settings from web.config
            WorkflowRuntime workflowRuntime = new WorkflowRuntime(WorkflowManager.WorkflowRuntimeKey);

            // add the external data exchange service to the runtime to allow for local services
            ExternalDataExchangeService exchangeService = new ExternalDataExchangeService(WorkflowManager.LocalServicesKey);

            CustomConditionsService condServ = new CustomConditionsService();

            SubWorkflowService serv = new SubWorkflowService(); // This is for Sub workflow service
            workflowRuntime.AddService(exchangeService);
            workflowRuntime.AddService(serv);
            workflowRuntime.AddService(condServ);

            // start the workflow runtime
            workflowRuntime.StartRuntime();

            // save the runtime for use by the entire application
            Application[WorkflowManager.WorkflowRuntimeKey] = workflowRuntime;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //WorkflowManager workflowManager = new WorkflowManager(Application, Session, Request, Response);
            //Session["workflowManager"] = workflowManager;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Session["workflowManager"] = null;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // obtain reference to workflow runtime
            WorkflowRuntime workflowRuntime = Application[WorkflowManager.WorkflowRuntimeKey] as WorkflowRuntime;

            // stop the runtime
            if (workflowRuntime != null)
            {
                workflowRuntime.StopRuntime();
                workflowRuntime.Dispose();
            }
        }
    }
}