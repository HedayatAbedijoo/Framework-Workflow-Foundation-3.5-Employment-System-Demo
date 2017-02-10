using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Data;
using System.Collections.Specialized;
using System.Reflection;
using HA.Workflow.Services.Interfaces;
using HA.Workflow.Services.Controls.Navigation;
using System.Workflow.ComponentModel;

namespace HA.Workflow.Services.BaseClasses
{
    public class StateMachineWorkflowBase : StateMachineWorkflowActivity
    {
        public virtual StateBehaveArgs RecievedEventDataEventsArgs { get; set; }

        /// <summary>
        /// از این ایونت برای ارسال اطلاعات مربوط به وورک فلو و نام ایونت‌های منتظر به هاست اپلیکشن استفاده میشود
        /// این ایونت بوسیله‌ای کد مقدار دهی شده و بوسیله‌ی صدا زدن یک تابع برای هاست اپلیکیشن ارسال میشود
        /// </summary>
        public virtual NavigationFlowEventArgs PageToGoToEventArgs { get; set; }

        /// <summary>
        /// این متد که در ابتدای ورود به هر استیت صدا زده میشود اطلاعات مربوط به صفحه‌ی وب متناسب با استیت جاری و نام ایونت‌های 
        /// منتظر و غیره را برای هاست اپلیکیشن ارسال می‌کند
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void InitializeOutgoingMessage(object sender, EventArgs e)
        {

            Dictionary<string, string> nextStates = GetSetStateInfoRecursive(this.Activities[this.CurrentStateName]);

            OrderedDictionary parameters = new OrderedDictionary();

            // initialize the outgoing args to send to the host application         
            string Goto = StatesHelper.GetGotoPage(this.CurrentStateName);

            /////////////////////////////Send Previous States Name to Host Application
            parameters.Add("PreviousStateName", this.PreviousStateName);
            parameters.Add("CurentStateName", this.CurrentStateName);
            ///////////////////////////// Send Next Events Name to Host Application            
            if (parameters.Count != 0)
            {
                DataTable dtNextEvents = new DataTable();
                dtNextEvents.Columns.Add("EventName", typeof(string));
                dtNextEvents.Columns.Add("EventAlias", typeof(string));
                foreach (KeyValuePair<string, string> kvp in nextStates)
                {
                    dtNextEvents.Rows.Add(kvp.Key, kvp.Value);

                }

                DataSet dsNextEvents = new DataSet();
                dsNextEvents.Tables.Add(dtNextEvents);
                parameters.Add("dsNextEvents", dsNextEvents);
            }

            PageToGoToEventArgs = new NavigationFlowEventArgs(this.WorkflowInstanceId, Goto, parameters);
        }

        private Dictionary<string, string> GetSetStateInfoRecursive(Activity act)
        {

            Dictionary<string, string> transictionState = new Dictionary<string, string>();
            CompositeActivity comAct = act as CompositeActivity;
            if (comAct != null)
            {
                foreach (Activity item in comAct.EnabledActivities)
                {
                    Dictionary<string, string> tmp = GetSetStateInfoRecursive(item);

                    foreach (KeyValuePair<string, string> kvp in tmp)
                    {
                        transictionState.Add(kvp.Key, kvp.Value);
                    }


                }
            }
            else
            {
                if (act is SetStateActivity)
                {
                    transictionState.Add(((SetStateActivity)act).TargetStateName, ((SetStateActivity)act).Description);
                }
            }
            return transictionState;
        }

    }
}
