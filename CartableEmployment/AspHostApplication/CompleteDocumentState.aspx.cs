﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspHostApplication
{
    public partial class CompleteDocumentState : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            (Master as MasterPage).workflowManager.SubWorkflowCompleted += new EventHandler<HA.Workflow.Services.Interfaces.SubWorkflowArgs>(workflowManager_SubWorkflowCompleted);
        }

        void workflowManager_SubWorkflowCompleted(object sender, HA.Workflow.Services.Interfaces.SubWorkflowArgs e)
        {
            Guid parentId = e.ParentWorkflowId;
            wfCartable OldCart = (from cart in context.wfCartable
                                  where cart.WorkflowId == parentId && cart.wfWorkflowStatus.Status == "Wait"
                                  select cart).First();

            OldCart.wfWorkflowStatusReference.Load();

            OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Completed");
            OldCart.ActionDate = DateTime.Now;
        }
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master as MasterPage;
            string nextState = master.workflowManager.GoNext("");

            if (!string.IsNullOrEmpty(nextState))
            {
                Guid cartableId = new Guid(Session["CartableId"].ToString());
                wfCartable OldCart = (from cart in context.wfCartable
                                      where cart.CartableId == cartableId
                                      select cart).First();

                OldCart.wfWorkflowStatusReference.Load();
                OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Completed");
                OldCart.ActionDate = DateTime.Now;

                wfCartable cartable = new wfCartable();
                cartable.CartableId = Guid.NewGuid();
                cartable.ItemId = Convert.ToInt32(Request.QueryString["ItemId"].ToString());
                cartable.WorkflowId = master.workflowManager.ParentWorkflowId;
                cartable.CreateDate = DateTime.Now;

                cartable.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Run") as wfWorkflowStatus;
                cartable.wfActivityStates = context.wfActivityStates.First(id => id.ActivityStateName == nextState) as wfActivityStates;

                context.AddTowfCartable(cartable);


                context.SaveChanges();
                btnConfirm.Enabled = false;
            }
        }
    }
}
