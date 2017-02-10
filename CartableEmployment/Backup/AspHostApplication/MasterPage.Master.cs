using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HA.CartableService;
using System.Data;

namespace AspHostApplication
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        WorkflowManager _workflowManager;
        public WorkflowManager workflowManager
        {
            get
            {
                return _workflowManager;
            }
            set
            {
                _workflowManager = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            workflowManager = new WorkflowManager(Application, Session, Request, Response);
            workflowManager.SubWorkflowStarted += new EventHandler<HA.Workflow.Services.Interfaces.SubWorkflowArgs>(workflowManager_SubWorkflowStarted);
        }

        void workflowManager_SubWorkflowStarted(object sender, HA.Workflow.Services.Interfaces.SubWorkflowArgs e)
        {
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // if (!IsPostBack)
            // BindDropDownTask(Session["EventWaiting"] as DataSet);
        }




    }
}
