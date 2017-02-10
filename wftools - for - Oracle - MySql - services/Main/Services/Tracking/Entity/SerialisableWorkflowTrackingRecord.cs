using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Implementation of <see cref="WorkflowTrackingRecord" /> that provides 
    /// a serialisable representation of the event args property.
    /// </summary>
    public class SerialisableWorkflowTrackingRecord : WorkflowTrackingRecord
    {
        private SerialisableData _eventArgs;
        /// <summary>
        /// Gets/sets a serialisable representation of the event arguments 
        /// of the <see cref="WorkflowTrackingRecord" />.
        /// </summary>
        public new SerialisableData EventArgs
        {
            get { return _eventArgs; }
            set { _eventArgs = value; }
        }
    }
}
