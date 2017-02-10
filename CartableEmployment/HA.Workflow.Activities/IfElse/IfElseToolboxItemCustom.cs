using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using System.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.ComponentModel.Design;

namespace Workflow.Activities.IfElse
{
    [Serializable]
    internal sealed class IfElseToolboxItemCustom : ActivityToolboxItem
    {
        #region Ctors

        public IfElseToolboxItemCustom(Type type) : base(type) { }

        #endregion

        #region Methods

        protected override IComponent[] CreateComponentsCore(IDesignerHost designerHost)
        {
            CompositeActivity __activity = new IfElseActivityCustom();
            __activity.Activities.Add(new IfElseBranchActivityCustom());
            __activity.Activities.Add(new IfElseBranchActivityCustom());
            return new IComponent[1] { __activity };
        }

        #endregion
    }
}
