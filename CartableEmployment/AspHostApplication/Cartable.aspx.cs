using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace AspHostApplication
{
    public partial class Cartable : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            var catrabel = from cartable in context.wfCartable
                           where cartable.wfWorkflowStatus.Status == "Run"
                           select new { cartable.CartableId, cartable.WorkflowId, cartable.ParentWorkflowId, cartable.wfWorkflowStatus.StatusName, cartable.wfActivityStates.Title };

            this.GridView1.DataSource = catrabel;
            this.GridView1.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Guid parentWorkflowId = Guid.Empty;

            Guid cartableId = new Guid(GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Text);
            Guid workflowId = new Guid(GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text);
            if (!string.IsNullOrEmpty(GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[3].Text) && GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[3].Text != "&nbsp;")
                parentWorkflowId = new Guid(GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[3].Text);

            var cartableRow = from cart in context.wfCartable
                              where cart.CartableId == cartableId
                              select new { cart, cart.wfActivityStates.ActivityStateName };

            //var cur = context.wfCartable.First(id => id.CartableId == cartableId);
            Session["ItemId"] = cartableRow.First().cart.ItemId;
            Session["CartableId"] = cartableId;

            (Master as MasterPage).workflowManager.CurrentWorkflowInstanceId = workflowId;
            (Master as MasterPage).workflowManager.ParentWorkflowId = parentWorkflowId;


            Response.Redirect(cartableRow.First().ActivityStateName + ".aspx?ItemId=" + cartableRow.First().cart.ItemId.ToString());
        }
    }
}
