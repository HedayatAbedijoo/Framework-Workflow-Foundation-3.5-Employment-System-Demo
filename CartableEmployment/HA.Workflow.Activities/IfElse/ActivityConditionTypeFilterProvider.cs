using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel;

namespace Workflow.Activities.IfElse
{
  internal class ActivityConditionTypeFilterProvider : ITypeFilterProvider
  {
    #region Fields

    private IServiceProvider _serviceProvider;

    #endregion

    #region Ctors

    public ActivityConditionTypeFilterProvider(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    #endregion

    #region Methods

    private static bool IsSubclassOf(Type type1, Type type2)
    {
      if (type1 != type2)
      {
        Type __baseType = type1.BaseType;
        while (__baseType != null)
        {
          if (__baseType == type2)
          {
            return true;
          }
          __baseType = __baseType.BaseType;
        }
      }
      return false;

    }

    bool ITypeFilterProvider.CanFilterType(Type type, bool throwOnError)
    {
      return IsSubclassOf(type, typeof(ActivityCondition));
      //return type.IsAssignableFrom(typeof(ActivityCondition));
      //return type.IsSubclassOf(typeof(ActivityCondition));
    }
    string ITypeFilterProvider.FilterDescription
    {
      get { return "Please select a class which derives from the ActivityCondition class."; }
    }

    #endregion
  }
}
