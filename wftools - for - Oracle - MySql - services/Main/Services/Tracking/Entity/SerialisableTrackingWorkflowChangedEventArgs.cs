using System.Collections.Generic;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents a serialised version of <see cref="TrackingWorkflowChangedEventArgs" />.
    /// </summary>
    public class SerialisableTrackingWorkflowChangedEventArgs : SerialisableData
    {
        private readonly IList<SerialisableWorkflowChangeAction> _changes = new List<SerialisableWorkflowChangeAction>();
        /// <summary>
        /// Gets a list of <see cref="SerialisableWorkflowChangeAction" /> objects
        /// representing the changes that have been made to the workflow.
        /// </summary>
        public IList<SerialisableWorkflowChangeAction> Changes
        {
            get { return _changes; }
        }
    }
}
