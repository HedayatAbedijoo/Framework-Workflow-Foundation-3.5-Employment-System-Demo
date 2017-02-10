using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Design;
using System.Collections.ObjectModel;
using System.Workflow.ComponentModel;

namespace Workflow.Activities.IfElse
{
    internal sealed class IfElseDesignerCustom : ParallelActivityDesigner
    {
        #region Ctors

        public IfElseDesignerCustom()
        {
        }

        #endregion

        #region Methods

        public override bool CanInsertActivities(HitTestInfo insertLocation, ReadOnlyCollection<Activity> activitiesToInsert)
        {
            foreach (Activity activity in activitiesToInsert)
            {
                if (!(activity is IfElseBranchActivityCustom))
                {
                    return false;
                }
            }
            return base.CanInsertActivities(insertLocation, activitiesToInsert);
        }
        public override bool CanMoveActivities(HitTestInfo moveLocation, ReadOnlyCollection<Activity> activitiesToMove)
        {
            if ((((this.ContainedDesigners.Count - activitiesToMove.Count) < 1) && (moveLocation != null)) && (moveLocation.AssociatedDesigner != this))
            {
                return false;
            }
            return true;
        }
        public override bool CanRemoveActivities(ReadOnlyCollection<Activity> activitiesToRemove)
        {
            if ((this.ContainedDesigners.Count - activitiesToRemove.Count) < 1)
            {
                return false;
            }
            return true;
        }
        protected override CompositeActivity OnCreateNewBranch()
        {
            return new IfElseBranchActivityCustom();
        }

        #endregion
    }
}
