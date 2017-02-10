using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Collections.Specialized;

namespace HA.Workflow.Services.Interfaces
{
    /// <summary>
    /// از این اینترفیس جهت حرکت در صفحه‌های وب مربوط به هر استیت استفاده می‌شود.
    /// به ازای هر استیک یه صفحه‌ی وب خاص وجود خواهد داشت
    /// </summary>
    [ExternalDataExchange]
    interface INavigationFlow
    {

        /// <summary>
        /// ابتدای ورود به هر استیت این متد فراخوانی می‌شود که بوسیله‌ی آن مشخص میگردد برای استیت جاری به کدام صحفه‌ی وب باید ارجاع داده شود
        /// </summary>
        /// <param name="Args"></param>
        void OnPageToGoTo(NavigationFlowEventArgs Args);


        /// <summary>
        /// از این ایونت استفاده می‌گردد تا مشخص شود که یک فلوی در حالت انتظار در کدام وضعیت قرار داشته است و صفحه‌ی وب مناسب با آخرین وضعیت آن چه می‌باشد 
        /// </summary>
        event EventHandler<NavigationFlowEventArgs> Rehydrated;
    }

    [Serializable]
    public class NavigationFlowEventArgs : ExternalDataEventArgs
    {
        public string PageToGoTo { get; set; }
        public IOrderedDictionary Parameters { get; set; }
        public NavigationFlowEventArgs(Guid instanceId, string pageToGoTo, IOrderedDictionary parameters)
            : base(instanceId)
        {
            this.PageToGoTo = pageToGoTo;
            this.Parameters = parameters;

        }
    }
}
