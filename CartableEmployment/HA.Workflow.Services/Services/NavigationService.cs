using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Runtime;
using HA.Workflow.Services.Interfaces;

namespace HA.Workflow.Services.Services
{
    public class NavigationService : INavigationFlow, IStateBehaveCondition, IStateBehaveCorrelation
    {
        #region INavigationFlow Members

        public event EventHandler<NavigationFlowEventArgs> PageToGoToReceived;
        /// <summary>
        /// این متد که توسط وورک فلو صدا زده می‌شود بوسیله‌ی صدا زدن یک ایونت، سرویس میزبان را از وقوع رخداد این متد مطلع میکند
        /// </summary>
        /// <param name="Args"></param>
        public void OnPageToGoTo(NavigationFlowEventArgs Args)
        {
            if (PageToGoToReceived != null)
            {
                PageToGoToReceived(null, Args);
            }
        }

        public bool HasPageToGoToReceived
        {
            get
            {
                if (PageToGoToReceived != null)
                    return true;
                else return false;
            }
        }


        public event EventHandler<NavigationFlowEventArgs> Rehydrated;
        /// <summary>
        /// Wrapper for the host app to call the Rehydrated event.
        /// </summary>
        /// <param name="args"></param>
        public void OnRehydrated(NavigationFlowEventArgs Args)
        {
            if (Rehydrated != null)
            {
                Rehydrated(null, Args);
            }
        }

        #endregion

        #region IStateBehaveCondition Members

        public event EventHandler<StateBehaveArgs> GoNext;
        public void OnGoNext(StateBehaveArgs Args)
        {
            if (GoNext != null)
            {
                GoNext(null, Args);
            }
        }


        public event EventHandler<StateBehaveArgs> CallBackMethodWorkflow;
        public void CallHostMethod(StateBehaveArgs Args)
        {
            if (CallBackMethodWorkflow != null)
            {
                CallBackMethodWorkflow(null, Args);
            }
        }

        #endregion

        #region IStateBehaveCorrelation Members

        public event EventHandler<StateBehaveCorrelationArgs> Process;
        public void OnProcess(StateBehaveCorrelationArgs Args)
        {
            if (Process != null)
                Process(null, Args);
        }


        public event EventHandler<StateBehaveCorrelationArgs> OnInitializeEventsName;
        public void InitializeEventsName(string EventNameWaiting, string Alias)
        {
            if (OnInitializeEventsName != null)
            {
                StateBehaveCorrelationArgs args =
                        new StateBehaveCorrelationArgs(WorkflowEnvironment.WorkflowInstanceId, EventNameWaiting, Alias, null);
                OnInitializeEventsName(null, args);
            }

        }

        #endregion

    }
}
