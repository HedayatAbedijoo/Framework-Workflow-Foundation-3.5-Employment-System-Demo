using System;

namespace WFTools.Samples.Windows
{
    /// <summary>
    /// Represents options used by the sample application.
    /// </summary>
    public class Options
    {
        private DatabaseSettings databaseSettings;
        /// <summary>
        /// Gets/sets the <see cref="DatabaseSettings" />.
        /// </summary>
        public DatabaseSettings DatabaseSettings
        {
            get { return databaseSettings; }
            set { databaseSettings = value; }
        }

        /// <summary>
        /// Gets a value indicating whether database settings have been specified.
        /// </summary>
        public Boolean HasDatabaseSettings
        {
            get { return databaseSettings != null; }
        }

        private WorkflowSettings workflowSettings;
        /// <summary>
        /// Gets/sets the <see cref="WorkflowSettings" />.
        /// </summary>
        public WorkflowSettings WorkflowSettings
        {
            get { return workflowSettings; }
            set { workflowSettings = value; }
        }

        /// <summary>
        /// Gets a value indicating whether workflow settings have been specified.
        /// </summary>
        public Boolean HasWorkflowSettings
        {
            get { return workflowSettings != null; }
        }
    }
}
