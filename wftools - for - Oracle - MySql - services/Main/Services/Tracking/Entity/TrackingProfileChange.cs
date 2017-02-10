using System;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents a changed tracking profile.
    /// </summary>
    public class TrackingProfileChange
    {
        private Type _workflowType;
        /// <summary>
        /// Gets/sets the type of workflow with which the tracking profile is associated.
        /// </summary>
        public Type WorkflowType
        {
            get { return _workflowType; }
            set { _workflowType = value; }
        }

        private TrackingProfile _trackingProfile;
        /// <summary>
        /// Gets/sets the <see cref="TrackingProfile" /> that was updated. This will be 
        /// <c>null</c> if the profile has been removed.
        /// </summary>
        public TrackingProfile TrackingProfile
        {
            get { return _trackingProfile; }
            set { _trackingProfile = value; }
        }
    }
}