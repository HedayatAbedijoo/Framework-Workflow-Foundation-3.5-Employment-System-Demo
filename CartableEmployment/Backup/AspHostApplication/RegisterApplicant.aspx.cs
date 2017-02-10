using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using HA.CartableService;

namespace AspHostApplication
{
    public partial class RegisterApplicant : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {

            this.BindDropDownTask();

            if (!IsPostBack && Request.QueryString["ItemId"] != null && !string.IsNullOrEmpty(Request.QueryString["ItemId"].ToString()))
                BindData();
            else
                txtRegisterDate.Text = DateTime.Now.ToShortDateString();

        }

        protected void StartWorkflow()
        {
            //string pageTogo = this.Page.Master.woe
            MasterPage master = this.Master as MasterPage;
            string state = master.workflowManager.StartNewWorkflow();

            Session["EventWaiting"] = master.workflowManager.EventWatingDataSet;

            wfCartable cartable = new wfCartable();
            Session["CartableId"] = cartable.CartableId = Guid.NewGuid();
            cartable.ItemId = Convert.ToInt32(txtId.Text);
            cartable.WorkflowId = master.workflowManager.CurrentWorkflowInstanceId;
            cartable.CreateDate = DateTime.Now;
            cartable.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Run") as wfWorkflowStatus;
            cartable.wfActivityStates = context.wfActivityStates.First(id => id.ActivityStateName == "RegisterApplicant") as wfActivityStates;

            context.AddTowfCartable(cartable);
            context.SaveChanges();
            BindDropDownTask();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            wfProfile profile = new wfProfile();
            profile.Id = Convert.ToInt32(txtId.Text);
            profile.FistName = txtFirstName.Text;
            profile.LastName = txtLastName.Text;
            profile.Age = Convert.ToInt16(txtAge.Text);
            profile.Degree = txtDegree.Text;
            profile.IdentityNo = Convert.ToInt32(txtIdentityNo.Text);
            profile.IsDocumentsOK = chkOk.Checked;
            context.AddTowfProfile(profile);
            context.SaveChanges();

            StartWorkflow();
        }

        protected void btnGoNext_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master as MasterPage;

            string nextState = master.workflowManager.GoNext(this.DropDownList1.SelectedValue);

            if (!string.IsNullOrEmpty(nextState))
            {
                //wfCartable car = context.wfCartable.First(id => id.CartableId == "");
                Guid cartableId = new Guid(Session["CartableId"].ToString());
                wfCartable OldCart = (from cart in context.wfCartable
                                      where cart.CartableId == cartableId
                                      select cart).First();

                OldCart.wfWorkflowStatusReference.Load();

                OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Completed");
                OldCart.ActionDate = DateTime.Now;

                wfCartable cartable = new wfCartable();
                cartable.CartableId = Guid.NewGuid();
                cartable.ItemId = Convert.ToInt32(txtId.Text);
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



        public void BindDropDownTask()
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

        public void BindData()
        {
            int i = Convert.ToInt32(Session["ItemId"].ToString());
            var empl = from emp in context.wfProfile
                       where emp.Id == i
                       select new { emp };

            txtFirstName.Text = empl.First().emp.FistName;
            txtLastName.Text = empl.First().emp.LastName;
            txtAge.Text = empl.First().emp.Age.ToString();
            txtDegree.Text = empl.First().emp.Degree.ToString();
            txtGener.Text = empl.First().emp.Gender;
            txtRegisterDate.Text = empl.First().emp.RegistDate.ToString();
            txtId.Text = empl.First().emp.Id.ToString();
        }
    }
}
