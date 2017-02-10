using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using HA.CartableService;

namespace AspHostApplication
{
    public partial class Interview : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["ItemId"] != null && !string.IsNullOrEmpty(Request.QueryString["ItemId"].ToString()))
                DataBind(Convert.ToInt32(Request.QueryString["ItemId"].ToString()));

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            (Master as MasterPage).workflowManager.SubWorkflowStarted += new EventHandler<HA.Workflow.Services.Interfaces.SubWorkflowArgs>(manager_SubWorkflowStarted);
        }

        private void DataBind(int p)
        {
            wfProfile empl = (from emp in context.wfProfile
                              where emp.Id == p
                              select emp).First();

            txtIQScore.Text = empl.IQScore != null ? empl.IQScore.ToString() : string.Empty;
            txtLanguage.Text = empl.LanguageScore != null ? empl.LanguageScore.ToString() : string.Empty;
            txtPersonalScore.Text = empl.PersonalityScore != null ? empl.PersonalityScore.ToString() : string.Empty;
            txtTechnicalScore.Text = empl.TechnicalScore != null ? empl.TechnicalScore.ToString() : string.Empty;

            Label1.Text = empl.FistName + " " + empl.LastName;

        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(Request.QueryString["ItemId"]);
            wfProfile profile = (from c in context.wfProfile
                                 where c.Id == i
                                 select c).First();

            if (!string.IsNullOrEmpty(txtIQScore.Text))
                profile.IQScore = Convert.ToInt16(txtIQScore.Text);
            if (!string.IsNullOrEmpty(txtLanguage.Text))
                profile.LanguageScore = Convert.ToInt16(txtLanguage.Text);
            if (!string.IsNullOrEmpty(txtPersonalScore.Text))
                profile.PersonalityScore = Convert.ToInt16(txtPersonalScore.Text);
            if (!string.IsNullOrEmpty(txtTechnicalScore.Text))
                profile.TechnicalScore = Convert.ToInt16(txtTechnicalScore.Text);


            context.SaveChanges();
        }

        protected void btnGoNext_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master as MasterPage;

            ArrayList ScoresArray = new ArrayList();
            ScoresArray.Add(txtIQScore.Text);
            ScoresArray.Add(txtLanguage.Text);
            ScoresArray.Add(txtPersonalScore.Text);
            ScoresArray.Add(txtTechnicalScore.Text);

            WorkflowManager manager = master.workflowManager;

            string nextState = manager.GoNext(ScoresArray);

            if (!string.IsNullOrEmpty(nextState))
            {
                if (nextState == "RejectApplicant")
                {
                    Guid cartableId = new Guid(Session["CartableId"].ToString());
                    wfCartable OldCart = (from cart in context.wfCartable
                                          where cart.CartableId == cartableId
                                          select cart).First();

                    OldCart.wfWorkflowStatusReference.Load();

                    OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Completed");
                    OldCart.ActionDate = DateTime.Now;
                }

                context.SaveChanges();

                btnGoNext.Enabled = false;
                BtnOk.Enabled = false;
            }
        }

        void manager_SubWorkflowStarted(object sender, HA.Workflow.Services.Interfaces.SubWorkflowArgs e)
        {
            MasterPage master = this.Master as MasterPage;
            WorkflowManager manager = master.workflowManager;

            Guid cartableId = new Guid(Session["CartableId"].ToString());
            wfCartable OldCart = (from cart in context.wfCartable
                                  where cart.CartableId == cartableId
                                  select cart).First();

            OldCart.wfWorkflowStatusReference.Load();
            OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Wait");


            wfCartable cartable = new wfCartable();
            cartable.CartableId = Guid.NewGuid();
            cartable.ItemId = Convert.ToInt32(Request.QueryString["ItemId"].ToString());
            cartable.WorkflowId = e.InstanceId;
            cartable.CreateDate = DateTime.Now;
            cartable.ParentWorkflowId = e.ParentWorkflowId;

            cartable.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Run") as wfWorkflowStatus;
            cartable.wfActivityStates = context.wfActivityStates.First(id => id.ActivityStateName == manager._PageToGoTO) as wfActivityStates;

            context.AddTowfCartable(cartable);

        }


    }
}
