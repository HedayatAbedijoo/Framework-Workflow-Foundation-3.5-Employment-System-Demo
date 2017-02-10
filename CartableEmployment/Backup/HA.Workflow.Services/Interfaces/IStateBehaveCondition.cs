using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Collections.Specialized;

namespace HA.Workflow.Services.Interfaces
{
    /// <summary>
    /// از این اینترفیس برای صدا کردن یک ایونت از یک استیت به همراه شرط خاصی استفاده میشود
    ///شرایط پارامتریک در غالب کالکشنی از پارامتر‌ها برای ایونت مورد نظر ارسال می‌شود
    /// </summary>
    [ExternalDataExchange]
    interface IStateBehaveCondition
    {

        event EventHandler<StateBehaveArgs> GoNext;

        void CallHostMethod(StateBehaveArgs Args);
    }

    [Serializable]
    public class StateBehaveArgs : ExternalDataEventArgs
    {
        public IOrderedDictionary Parameters { get; set; }
        public StateBehaveArgs(Guid instanceId, IOrderedDictionary parameters)
            : base(instanceId)
        {
            this.Parameters = parameters;
        }

    }
}
