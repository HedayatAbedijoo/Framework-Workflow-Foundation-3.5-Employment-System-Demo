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

namespace Workflow.Activities.IfElse
{
  [ActivityValidator(typeof(IfElseValidatorCustom)), Designer(typeof(IfElseDesignerCustom), typeof(IDesigner)), ToolboxItem(typeof(IfElseToolboxItemCustom)), Category("Standard"), Description("Executes contained activities based on condition specified.")]
  public sealed class IfElseActivityCustom : CompositeActivity, IActivityEventListener<ActivityExecutionStatusChangedEventArgs>
  {
    #region Ctors

    public IfElseActivityCustom()
    {
    }
    public IfElseActivityCustom(string name)
      : base(name)
    {
    }

    #endregion

    #region Methods

    protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
    {
      ActivityExecutionStatus __status = ActivityExecutionStatus.Closed;

      if (executionContext == null)
      {
        throw new ArgumentNullException("executionContext");
      }
      foreach (IfElseBranchActivityCustom branch in this.EnabledActivities)
      {
        if ((branch.Condition == null) || (branch.Condition.Evaluate(branch, executionContext)))
        {
          __status = ActivityExecutionStatus.Executing;
          branch.RegisterForStatusChange(Activity.ClosedEvent, this);
          executionContext.ExecuteActivity(branch);
          break;
        }
      }
      return __status;
    }
    protected override ActivityExecutionStatus Cancel(ActivityExecutionContext executionContext)
    {
      ActivityExecutionStatus __status = ActivityExecutionStatus.Closed;

      foreach (IfElseBranchActivityCustom branch in this.EnabledActivities)
      {
        if (branch.ExecutionStatus == ActivityExecutionStatus.Executing)
        {
          __status = ActivityExecutionStatus.Canceling;
          executionContext.CancelActivity(branch);
          break;
        }
        if ((branch.ExecutionStatus == ActivityExecutionStatus.Canceling) || (branch.ExecutionStatus == ActivityExecutionStatus.Faulting))
        {
          __status = ActivityExecutionStatus.Canceling;
          break;
        }
      }
      return __status;
    }
    public void OnEvent(object sender, ActivityExecutionStatusChangedEventArgs e)
    {
      if (sender == null)
      {
        throw new ArgumentNullException("sender");
      }
      if (e == null)
      {
        throw new ArgumentNullException("e");
      }
      ActivityExecutionContext __context = sender as ActivityExecutionContext;
      if (__context == null)
      {
        // TODO : Get the exception message from the System.Workflow.Activities.StringResources.resources resource file.
        throw new ArgumentException("The sender parameter must be of type ActivityExecutionContext.");
      }
      __context.CloseActivity();
    }

    #endregion
  }
}
