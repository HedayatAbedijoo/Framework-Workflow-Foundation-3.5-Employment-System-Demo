using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Drawing.Design;

namespace Workflow.Activities.IfElse
{
  [Designer(typeof(IfElseBranchDesignerCustom), typeof(IDesigner)), ActivityValidator(typeof(IfElseBranchValidatorCustom)), ToolboxItem(false), Category("Standard")]
	public sealed class IfElseBranchActivityCustom: SequenceActivity
	{
		#region Dependency Properties

    public static readonly DependencyProperty ConditionProperty;
    
    #endregion

    #region Ctors

    static IfElseBranchActivityCustom()
    {
      IfElseBranchActivityCustom.ConditionProperty = DependencyProperty.Register("Condition", typeof(ActivityCondition), typeof(IfElseBranchActivityCustom), new PropertyMetadata(DependencyPropertyOptions.Metadata));
    }
    public IfElseBranchActivityCustom()
    {
    }
    public IfElseBranchActivityCustom(string name)
      : base(name)
    {
    }

    #endregion

    #region Properties

    [Editor(typeof(TypeBrowserEditor), typeof(UITypeEditor)), TypeFilterProvider(typeof(ActivityConditionTypeFilterProvider)), TypeConverter(typeof(ConditionTypeConverterCustom)), Category("Conditions"), Description("Please specify a condition for enabling this activity."), DefaultValue((string)null), RefreshProperties(RefreshProperties.Repaint)]
    public ActivityCondition Condition
    {
      get
      {
        return (base.GetValue(IfElseBranchActivityCustom.ConditionProperty) as ActivityCondition);
      }
      set
      {
        base.SetValue(IfElseBranchActivityCustom.ConditionProperty, value);
      }
    }

    #endregion
	}
}
