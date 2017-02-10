using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace AspHostApplication
{
    public partial class FlowPanel : System.Web.UI.UserControl
    {
        public event EventHandler<WorkFlowEventArgs> OnConfirm;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindDropDown();
        }

        private void BindDropDown()
        {
            if (Session["EventWaiting"] != null)
            {
                DataSet ds = Session["EventWaiting"] as DataSet;
                if (ds.Tables.Count == 0)
                {
                    DropDownList1.Items.Add(new ListItem("state1", "state1"));
                    DropDownList1.Items.Add(new ListItem("state2", "state2"));
                    DropDownList1.Items.Add(new ListItem("state3", "state3"));
                }
                else
                {
                    this.DropDownList1.DataSource = ds;
                    DropDownList1.DataTextField = "EventAlias";
                    DropDownList1.DataValueField = "EventName";
                    DropDownList1.DataBind();
                }
            }
            else
            {
                DropDownList1.Items.Add(new ListItem("state1", "state1"));
                DropDownList1.Items.Add(new ListItem("state2", "state2"));
                DropDownList1.Items.Add(new ListItem("state3", "state3"));

            }
        }

        public void BindDrp()
        {
            BindDropDown();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (OnConfirm != null)
            {
                if (OnConfirm != null)
                    OnConfirm(this, new WorkFlowEventArgs(DropDownList1.SelectedItem.Value));
            }
        }
    }

    public class WorkFlowEventArgs : EventArgs
    {
        public WorkFlowEventArgs(string name)
        {
            this.CommandName = name;
        }
        public string CommandName
        { get; set; }
    }
}