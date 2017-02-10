using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Design;

namespace Workflow.Activities.IfElse
{
  internal sealed class IfElseBranchDesignerCustom : SequentialActivityDesigner
  {
    #region Ctors

    public IfElseBranchDesignerCustom()
    {
    }

    #endregion

    #region Methods

    public override bool CanBeParentedTo(CompositeActivityDesigner parentActivityDesigner)
    {
      if (parentActivityDesigner == null)
      {
        throw new ArgumentNullException("parentActivity");
      }
      if (!(parentActivityDesigner.Activity is IfElseActivityCustom))
      {
        return false;
      }
      return base.CanBeParentedTo(parentActivityDesigner);
    }

    #endregion

  }
}
