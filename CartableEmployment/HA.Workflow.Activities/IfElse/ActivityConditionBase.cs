using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;

namespace RioterDecker.Workflow.Activities.IfElse
{
    public abstract class ActivityConditionBase : ActivityCondition
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemProperty;

        #endregion
        static ActivityConditionBase()
        {
            ActivityConditionBase.ItemProperty = DependencyProperty.Register("Item", typeof(Item), typeof(ActivityConditionBase), new PropertyMetadata(null, DependencyPropertyOptions.Default, new Attribute[] { new ValidationOptionAttribute(ValidationOption.Required) }));
        }
        #region Properties

        public Item Item
        {
            get
            {
                return (base.GetValue(ActivityConditionBase.ItemProperty) as Item);
            }
            set
            {
                base.SetValue(ActivityConditionBase.ItemProperty, value);
            }
        }

        #endregion
        //virtual bool Evaluate(Activity activity, IServiceProvider provider)
        //{
        //    throw new NotImplementedException();
        //}
        //public abstract bool Evaluate(Activity activity, IServiceProvider provider);

        //public override abstract bool Evaluate(Activity activity, IServiceProvider provider)
        //{
        //    throw new NotImplementedException();
        //}
    }


    public class Item
    { }
}
