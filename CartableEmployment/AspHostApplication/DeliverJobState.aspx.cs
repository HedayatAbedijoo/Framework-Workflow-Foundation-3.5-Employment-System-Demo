using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspHostApplication
{
    public partial class DeliverJobState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            CartableEmploymentEntities context = new CartableEmploymentEntities();
            MasterPage master = this.Master as MasterPage;
            master.workflowManager.GoNext("");

            Guid cartableId = new Guid(Session["CartableId"].ToString());
            wfCartable OldCart = (from cart in context.wfCartable
                                  where cart.CartableId == cartableId
                                  select cart).First();

            OldCart.wfWorkflowStatusReference.Load();
            OldCart.wfWorkflowStatus = context.wfWorkflowStatus.First(id => id.Status == "Completed");
            OldCart.ActionDate = DateTime.Now;

            context.SaveChanges();
            btnConfirm.Enabled = false;
        }
    }
}
