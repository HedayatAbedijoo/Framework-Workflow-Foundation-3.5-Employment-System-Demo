using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using CartableService;
using WorkflowService.Controls.CallSubWorkflow;
using System.Configuration;
using WFTools.Services.Persistence.Ado;

namespace ASPHostCartable
{
    public class Globalaa : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // create an instance of workflow runtime, loading settings from web.config
            WorkflowRuntime workflowRuntime = new WorkflowRuntime(WorkflowManager.WorkflowRuntimeKey);

            // add the external data exchange service to the runtime to allow for local services
            ExternalDataExchangeService exchangeService = new ExternalDataExchangeService(WorkflowManager.LocalServicesKey);

            ////////////// Add Oracel Persistence instead of SQLServer Persistence
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["Oracle - ODP.NET"];
            workflowRuntime.AddService(new AdoPersistenceService(
              connectionStringSetting, true, TimeSpan.FromMinutes(1),
              TimeSpan.FromMinutes(1)));
            //////////////

            CallWorkflowService serv = new CallWorkflowService(); // This is for Sub workflow service
            workflowRuntime.AddService(exchangeService);
            workflowRuntime.AddService(serv);

            // start the workflow runtime
            workflowRuntime.StartRuntime();

            // save the runtime for use by the entire application
            Application[WorkflowManager.WorkflowRuntimeKey] = workflowRuntime;
        }


        protected void Session_Start(object sender, EventArgs e)
        {
            WorkflowManager workflowManager = new WorkflowManager(Application, Session, Request, Response);
            Session["workflowManager"] = workflowManager;
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