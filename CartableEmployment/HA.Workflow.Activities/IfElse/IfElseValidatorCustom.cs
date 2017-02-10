using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel;

namespace Workflow.Activities.IfElse
{
    internal sealed class IfElseValidatorCustom : CompositeActivityValidator
    {
        #region Ctors

        public IfElseValidatorCustom()
        {
        }

        #endregion

        #region Methods

        public override ValidationErrorCollection Validate(ValidationManager manager, object obj)
        {
            ValidationErrorCollection __errors = base.Validate(manager, obj);
            IfElseActivityCustom __ifElseActivity = obj as IfElseActivityCustom;
            if (__ifElseActivity == null)
            {
                throw new ArgumentException("obj");
            }
            if (__ifElseActivity.EnabledActivities.Count < 1)
            {
                // TODO : Get the exception message from the System.Workflow.Activities.StringResources.resources resource file.
                __errors.Add(new ValidationError("A IfElseActivity activity must have at least one child of type IfElseBranch.", 1292));
            }
            foreach (Activity activity in __ifElseActivity.EnabledActivities)
            {
                if (!(activity is IfElseBranchActivityCustom))
                {
                    // TODO : Get the exception message from the System.Workflow.Activities.StringResources.resources resource file.
                    __errors.Add(new ValidationError("All children must be of type IfElseBranch.", 1293));
                    return __errors;
                }
            }
            return __errors;
        }
        public override ValidationError ValidateActivityChange(Activity activity, ActivityChangeAction action)
        {
            if (activity == null)
            {
                throw new ArgumentNullException("activity");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if ((activity.ExecutionStatus != ActivityExecutionStatus.Initialized) && (activity.ExecutionStatus != ActivityExecutionStatus.Closed))
            {
                return new ValidationError(string.Format("CompositeActivity '{0}' status is currently '{1}'. Dynamic modifications are allowed only when the activity status is 'Enabled' or 'Suspended'.", activity.QualifiedName, activity.ExecutionStatus.ToString()), 260);
            }
            return null;
        }

        #endregion
    }
}
