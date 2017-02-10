using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Compiler;

namespace Workflow.Activities.IfElse
{
  internal sealed class IfElseBranchValidatorCustom : CompositeActivityValidator
  {
    #region Ctors

    public IfElseBranchValidatorCustom()
    {
    }

    #endregion

    #region Methods

    public override ValidationErrorCollection Validate(ValidationManager manager, object obj)
    {
      ValidationErrorCollection __errors = base.Validate(manager, obj);
      IfElseBranchActivityCustom __branch = (IfElseBranchActivityCustom)obj;
      IfElseActivityCustom __parent = __branch.Parent as IfElseActivityCustom;
      if (__parent == null)
      {
        // TODO : Get the exception message from the System.Workflow.Activities.StringResources.resources resource file.
        __errors.Add(new ValidationError("Parent of CustomIfElseBranch must be an IfElse", 0x50e));
      }
      int __index = __parent.Activities.IndexOf(__branch);

      // The last branch can have a condition which is null.
      if (((__parent.Activities.Count <= 1) || (__index < (__parent.Activities.Count - 1))) && (__branch.Condition == null))
      {
        __errors.Add(ValidationError.GetNotSetValidationError("Condition"));
      }
      return __errors;
    }

    #endregion
  }
}
