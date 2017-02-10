using System.Collections.Generic;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Implementation of <see cref="ActivityTrackingRecord" /> that provides 
    /// a serialisable representation of the event args and body properties.
    /// </summary>
    public class SerialisableActivityTrackingRecord : ActivityTrackingRecord
    {
        private readonly IList<SerialisableTrackingDataItem> _body = new List<SerialisableTrackingDataItem>();
        /// <summary>
        /// Gets a list of <see cref="SerialisableTrackingDataItem" /> objects
        /// representing the serialised body of the <see cref="ActivityTrackingRecord" />.
        /// </summary>
        public new IList<SerialisableTrackingDataItem> Body
        {
            get { return _body; }
        }

        private SerialisableData _eventArgs;
        /// <summary>
        /// Gets/sets the serialisable representation of the event arguments of 
        /// the <see cref="ActivityTrackingRecord" />.
        /// </summary>
        public new SerialisableData EventArgs
        {
            get { return _eventArgs; }
            set { _eventArgs = value; }
        }
    }
}
