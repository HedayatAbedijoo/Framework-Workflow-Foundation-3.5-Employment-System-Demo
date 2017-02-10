using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspHostApplication
{
    public partial class CartableArchive : System.Web.UI.Page
    {
        CartableEmploymentEntities context = new CartableEmploymentEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            var catrabel = from cartable in context.wfCartable
                           orderby cartable.ItemId, cartable.wfWorkflowStatus.Status, cartable.CreateDate, cartable.ActionDate
                           select new { cartable.ItemId, cartable.CreateDate, cartable.wfWorkflowStatus.StatusName, cartable.ActionDate, cartable.wfActivityStates.Title };

            this.GridView1.DataSource = catrabel;
            this.GridView1.DataBind();
        }
    }
}
