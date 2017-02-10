using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace HA.Workflow.Services.Controls.ActivityConditionHA
{

    public class DynamicTypeDescriptor
    {
        public DynamicTypeDescriptor() { }

        public static PropertyDescriptorCollection GetProperties(object component)
        {
            PropertyInfo[] propsInfo = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            ArrayList list = new ArrayList(propsInfo.Length);

            foreach (PropertyInfo prop in propsInfo)
            {
                ArrayList attributeList = new ArrayList();
                foreach (Attribute attrib in prop.GetCustomAttributes(true))
                {
                    if (attrib is DependsOnPropertyAttribute)
                        attributeList.Add(((DependsOnPropertyAttribute)attrib).Evaluate(component));
                    else
                        attributeList.Add(attrib);
                }
                list.Add(new PropertyInfoDescriptor(prop, (Attribute[])attributeList.ToArray(typeof(Attribute))));
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[])list.ToArray(typeof(PropertyDescriptor)));
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of class
        /// </summary>
        /// <param name="expression">Property name</param>
        protected DependsOnPropertyAttribute(string property)
            : base()
        {
            _property = property;
            _index = null;
        }
        /// <summary>
        /// Create new instance of class
        /// </summary>
        /// <param name="property">Property name</param>
        /// <param name="index">Property element index</param>
        protected DependsOnPropertyAttribute(string property, int index)
        {
            _property = property;
            _index = new object[] { index };
        }

        private string _property;
        private object[] _index;

        /// <summary>
        /// Evaluate attribute using property container supplied
        /// </summary>
        /// <param name="container">Object that contains property to evaluate</param>
        /// <returns>Dynamically evaluated attribute</returns>
        public Attribute Evaluate(object container)
        {
            return OnEvaluateCoplete(RuntimeEvaluator.Eval(container, _property, _index));
        }
        /// <summary>
        /// Specific dynamic attribute check implementation
        /// </summary>
        /// <param name="value">Evaluated value</param>
        /// <returns>Dynamically evaluated attribute</returns>
        protected abstract Attribute OnEvaluateCoplete(object value);

        private class RuntimeEvaluator
        {
            public static object Eval(object container, string property, object[] index)
            {
                PropertyInfo pInfo = container.GetType().GetProperty(property);
                if (pInfo != null)
                    return pInfo.GetValue(container, index);

                return null;
            }
        }
    }    
    public class DynamicReadonlyAttribute : DependsOnPropertyAttribute
    {
        public DynamicReadonlyAttribute(string property) : base(property) { }
        public DynamicReadonlyAttribute(string property, int index) : base(property, index) { }
        protected override Attribute OnEvaluateCoplete(object value)
        {
            Attribute output;
            try
            {
                // check if value is provided
                if (value == null)
                    value = false; // asume default
                // create attribute
                output = new ReadOnlyAttribute((bool)value);
            }
            catch
            {
                output = new ReadOnlyAttribute(false);
            }
            return output;
        }

    }
    public class DynamicBrowsableAttribute : DependsOnPropertyAttribute
    {
        public DynamicBrowsableAttribute(string property) : base(property) { }
        public DynamicBrowsableAttribute(string property, int index) : base(property, index) { }
        protected override Attribute OnEvaluateCoplete(object value)
        {

            Attribute output;
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition chk = (HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition)value;
            try
            {
                if (chk == HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit)
                    return new ReadOnlyAttribute(true);
                else
                    return new ReadOnlyAttribute(false);

            }
            catch
            {
                output = new ReadOnlyAttribute(false);
            }
            return output;
            //Attribute output;
            //try
            //{
            //    // check if value is provided
            //    if (value == null)
            //        value = true; // asume default
            //    // create attribute
            //    output = new BrowsableAttribute((bool)value);
            //}
            //catch
            //{
            //    output = new ReadOnlyAttribute(true);
            //}
            //return output;
        }


    }
    public class PropertyInfoDescriptor : PropertyDescriptor
    {

        private PropertyInfo propInfo;

        public PropertyInfoDescriptor(PropertyInfo prop, Attribute[] attribs)
            : base(prop.Name, attribs)
        {
            propInfo = prop;
        }
        private object DefaultValue
        {
            get
            {
                if (propInfo.IsDefined(typeof(DefaultValueAttribute), false))
                {
                    return ((DefaultValueAttribute)propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false)[0]).Value;
                }
                return null;
            }
        }
        public override bool IsReadOnly
        {
            get { return this.Attributes.Contains(new System.ComponentModel.ReadOnlyAttribute(true)); }
        }

        public override object GetValue(object component)
        {
            return propInfo.GetValue(component, null);
        }

        public override bool CanResetValue(object component)
        {
            return (!this.IsReadOnly & (this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component))));
        }

        public override void ResetValue(object component)
        {
            this.SetValue(component, this.DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            propInfo.SetValue(component, value, null);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return (!this.IsReadOnly & (this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component))));
        }

        public override Type ComponentType
        {
            get
            {
                return propInfo.DeclaringType;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return propInfo.PropertyType;
            }
        }
    }
}
