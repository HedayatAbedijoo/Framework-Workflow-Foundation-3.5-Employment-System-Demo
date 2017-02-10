using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Types of tracking records recorded in a <see cref="TrackingChannel" />.
    /// </summary>
    public enum TrackingRecordType
    {
        /// <summary>
        /// Indicates that a tracking record is related to an 
        /// <see cref="ActivityTrackingRecord" />.
        /// </summary>
        Activity,
        /// <summary>
        /// Indicates that a tracking record is related to an 
        /// <see cref="UserTrackingRecord" />.
        /// </summary>
        User,
        /// <summary>
        /// Indicates that a tracking record is related to an 
        /// <see cref="WorkflowTrackingRecord" />.
        /// </summary>
        Workflow
    }
}
