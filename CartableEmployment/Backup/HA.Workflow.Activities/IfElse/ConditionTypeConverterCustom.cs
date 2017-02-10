using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Workflow.Activities.IfElse
{
  internal sealed class ConditionTypeConverterCustom : TypeConverter
  {
    #region Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }
      return base.CanConvertFrom(context, sourceType);
    }
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (!(value is string))
      {
        return base.ConvertFrom(context, culture, value);
      }
      else if (((string)value).Length != 0)
      {
        return Activator.CreateInstance(Type.GetType((string)value));
      }
      return null;
    }
    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
      return true;
    }
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
      TypeConverter __typeConverter = TypeDescriptor.GetConverter(value.GetType());
      if (((__typeConverter != null) && (__typeConverter.GetType() != base.GetType())) && __typeConverter.GetPropertiesSupported())
      {
        return __typeConverter.GetProperties(context, value, attributes);
      }

      PropertyDescriptorCollection __propertyDescriptors = TypeDescriptor.GetProperties(value, true);
      return __propertyDescriptors;
    }

    #endregion
  }
}
