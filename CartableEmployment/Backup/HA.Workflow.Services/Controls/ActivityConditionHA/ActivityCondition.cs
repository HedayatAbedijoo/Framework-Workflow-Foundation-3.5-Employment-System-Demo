using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Reflection;
using HA.Workflow.Services.Interfaces;
using System.ComponentModel;
using System.Workflow.Activities;

namespace HA.Workflow.Services.Controls.ActivityConditionHA
{
    public class ActivityConditionHA : ActivityCondition
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemProperty;

        #endregion
        public enum enmTransitionCondition
        {
            [Description("چنانچه قرار باشد بوسیله‌ی نام استیت بعدی تغییر حالت دهیم")]
            DirectTransit = 0,
            [Description("تصمیم گیری حرکت بین وضعیت بوسیله‌ی تابعی خاص انجام می‌شود")]
            ConditionalTransit = 1
        }
        //لیست متد‌های شرطی را برای پراپرتی ارسال میکند تا جهت انتخاب برنامه نویس نشان داده شود
        private static void SetMethodListsToPeropertyGrid()
        {
            MethodInfo[] method = typeof(ICustomConditions).GetMethods();

            foreach (MethodInfo item in method)
            {
                methodList.Add(item.Name);
            }

            HA.Workflow.Services.BaseClasses.PropertyGrid.HA_GlobalVars._ListofRules = methodList.ToArray();
        }
        private static List<string> methodList = new List<string>();

        static ActivityConditionHA()
        {
            ActivityConditionHA.ItemProperty = DependencyProperty.Register("Item", typeof(StateBehaveArgs), typeof(ActivityConditionHA), new PropertyMetadata(null, DependencyPropertyOptions.Default, new Attribute[] { new ValidationOptionAttribute(ValidationOption.Required) }));
            SetMethodListsToPeropertyGrid();
        }
        #region Properties

        public StateBehaveArgs Item
        {
            get
            {
                return (base.GetValue(ActivityConditionHA.ItemProperty) as StateBehaveArgs);
            }
            set
            {
                base.SetValue(ActivityConditionHA.ItemProperty, value);
            }
        }


        #endregion

        public override bool Evaluate(Activity activity, IServiceProvider provider)
        {
            if (TransitionCondition == enmTransitionCondition.DirectTransit)
            {
                bool flag = false;
                CompositeActivity comAct = activity as CompositeActivity;

                foreach (Activity item in comAct.Activities)
                {
                    if (item is SetStateActivity)
                    {
                        if (((SetStateActivity)item).TargetStateName == this.Item.Parameters[0].ToString())
                        {
                            flag = true;
                        }
                    }
                }

                return flag;
            }
            else
            {

                ICustomConditions customCondtion = provider.GetService(typeof(ICustomConditions)) as ICustomConditions;
                Type type = customCondtion.GetType();
                MethodInfo method = type.GetMethod(this._Rule);
                if (method == null)
                    return false;
                object[] param = new object[1];
                param[0] = this.Item;
                object result = method.Invoke(customCondtion, param);

                if (result == null)
                    return false;

                return (bool)result;
            }
        }
      

        private string _Rule = "";
        [Browsable(true)]
        [TypeConverter(typeof(HA.Workflow.Services.BaseClasses.PropertyGrid.RuleConverter))]
        public string Rule
        {

            //When first loaded set property with the first item in the rule list.
            get
            {
                string S = "";
                if (_Rule != null)
                {
                    S = _Rule;
                }
                else
                {
                    if (HA.Workflow.Services.BaseClasses.PropertyGrid.HA_GlobalVars._ListofRules.Length > 0)
                    {
                        //Sort the list before displaying it
                        Array.Sort(HA.Workflow.Services.BaseClasses.PropertyGrid.HA_GlobalVars._ListofRules);

                        S = HA.Workflow.Services.BaseClasses.PropertyGrid.HA_GlobalVars._ListofRules[0];
                    }
                }

                return S;
            }
            set { _Rule = value; }

        }

        private enmTransitionCondition transitionCondition = enmTransitionCondition.DirectTransit;
        public enmTransitionCondition TransitionCondition
        {
            get { return transitionCondition; }
            set { transitionCondition = value; }
        }
    }
}
