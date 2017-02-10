using System;

namespace WFTools.Samples.Windows
{
    /// <summary>
    /// Settings used when creating new workflows.
    /// </summary>
    public class WorkflowSettings
    {
        private Boolean modifySequentialWorkflow;
        /// <summary>
        /// Gets/sets whether sequential workflows should
        /// be modified as part of the creation process.
        /// </summary>
        public Boolean ModifySequentialWorkflow
        {
            get { return modifySequentialWorkflow; }
            set { modifySequentialWorkflow = value; }
        }

    }
}
