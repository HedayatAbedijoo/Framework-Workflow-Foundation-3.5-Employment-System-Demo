using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Collections.Specialized;

namespace HA.Workflow.Services.Interfaces
{
    /// <summary>
    /// از این اینترفیس برای استیت‌هایی استفاده می‌شود که وضعیت‌های خروجی خود را (نام ایونت‌هایی منتظر این استیت ) ایجاد میکند و در اختیار اپلیکشن قرار میدهد.
    /// </summary>
    [ExternalDataExchange]
    [CorrelationParameter("EventNameWaiting")]
    interface IStateBehaveCorrelation
    {
        /// <summary>
        ///  این ایونت یک استیت را با نام ایونت مورد نظر حرکت میدهد
        /// </summary>
        [CorrelationAlias("EventNameWaiting", "e.Command")]
        event EventHandler<StateBehaveCorrelationArgs> Process;

        /// <summary>
        /// هر استیت منتظر تعدادی رویدادی خواهد بود که بوسیله‌ی آنها به سایر استیت‌ها حرکت میکند. نام ایونت‌های منتظر در هر استیت با این متد مشخص میشود
        /// به ازای هر ایونت در هر استیت یک بار این متد صدا زده می‌شود.
        /// </summary>
        /// <param name="EventNameWaiting"></param>
        /// <param name="Alias"></param>
        [CorrelationInitializer()]
        void InitializeEventsName(string EventNameWaiting, string Alias);
    }

    [Serializable]
    public class StateBehaveCorrelationArgs : ExternalDataEventArgs
    {
        public IOrderedDictionary Parameters { get; set; }
        public string Alias { get; set; }
        public string EventName { get; set; }
        public StateBehaveCorrelationArgs(Guid instanceId, string EventNameWaiting, string Alias, IOrderedDictionary parameters)
            : base(instanceId)
        {
            this.Parameters = parameters;
            this.EventName = EventName;
            this.Alias = Alias;
        }

    }
}
