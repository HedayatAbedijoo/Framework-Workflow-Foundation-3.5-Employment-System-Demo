using System.Collections.Generic;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Implementation of <see cref="UserTrackingRecord" /> that provides 
    /// a serialisable representation of the event args, body and user data
    /// properties.
    /// </summary>
    public class SerialisableUserTrackingRecord : UserTrackingRecord
    {
        private readonly IList<SerialisableTrackingDataItem> _body = new List<SerialisableTrackingDataItem>();
        /// <summary>
        /// Gets a list of <see cref="SerialisableTrackingDataItem" /> objects
        /// representing the serialisable representation of the body of the 
        /// <see cref="UserTrackingRecord" />.
        /// </summary>
        public new IList<SerialisableTrackingDataItem> Body
        {
            get { return _body; }
        }

        private SerialisableData _userData;
        /// <summary>
        /// Gets/sets a serialisable representation of the user data contained
        /// in the <see cref="UserTrackingRecord" />.
        /// </summary>
        public new SerialisableData UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }
	
        private SerialisableData _eventArgs;
        /// <summary>
        /// Gets/sets a serialisable representation of the event arguments 
        /// of the <see cref="UserTrackingRecord" />.
        /// </summary>
        public new SerialisableData EventArgs
        {
            get { return _eventArgs; }
            set { _eventArgs = value; }
        }
    }
}
