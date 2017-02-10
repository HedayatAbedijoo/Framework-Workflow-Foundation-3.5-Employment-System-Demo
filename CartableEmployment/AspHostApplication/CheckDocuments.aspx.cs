using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace AspHostApplication
{
    public partial class CheckDocuments : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindDropDownTask();

            if (!IsPostBack && Request.QueryString["ItemId"] != null && !string.IsNullOrEmpty(Request.QueryString["ItemId"].ToString()))
                DataBind(Convert.ToInt32(Request.QueryString["ItemId"].ToString()));
        }

        private void BindDropDownTask()
        {
            MasterPage master = this.Master as MasterPage;
            DataTable dt = master.workflowManager.GetActionName();
            if (dt != null)
            {
                this.DropDownList1.DataSource = dt;
                DropDownList1.DataTextField = "EventAlias";
                DropDownList1.DataValueField = "EventName";
                DropDownList1.DataBind();
                btnGoNext.Visible = true;
                DropDownList1.Visible = true;
            }

            else
            {
                btnGoNext.Visible = false;
                DropDownList1.Visible = false;
            }
        }

        private void DataBind(int p)
        {
            wfProfile empl = (from emp in context.wfProfile
                              where emp.Id == p
                              select emp).First();

            txtInteresting.Text = empl.Interesting;
            txtOccupation.Text = empl.Occupation;

            if (empl.YearExperiences != null)
                txtYearExperiences.Text = empl.YearExperiences.ToString();
            chkIsDocumentOk.Checked = Convert.ToBoolean(empl.IsDocumentsOK);

            Label1.Text = empl.FistName + " " + empl.LastName;

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(Request.QueryString["ItemId"]);
            wfProfile profile = (from c in context.wfProfile
                                 where c.Id == i
                                 select c).First();

            profile.Interesting = txtInteresting.Text;
            profile.Occupation = txtOccupation.Text;
            profile.YearExperiences = txtYearExperiences.Text;
            profile.IsDocumentsOK = chkIsDocumentOk.Checked;

            context.SaveChanges();
        }

        protected void btnGoNext_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master as MasterPage;
            string nextState = master.workflowManager.GoNext(this.DropDownList1.SelectedValue);

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
                cartable.WorkflowId = master.workflowManager.CurrentWorkflowInstanceId;
                cartable.CreateDate = DateTime.Now;

                cartable.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Run") as wfWorkflowStatus;
                cartable.wfActivityStates = context.wfActivityStates.First(id => id.ActivityStateName == nextState) as wfActivityStates;

                context.AddTowfCartable(cartable);

                context.SaveChanges();

                btnGoNext.Enabled = false;
                btnSave.Enabled = false;
            }
        }
    }
}
