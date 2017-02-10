using System;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Interface that exposes data-access functionality to the 
    /// <see cref="GenericWorkflowTrackingService" /> and 
    /// <see cref="GenericWorkflowTrackingChannel"/>.
    /// </summary>
    public interface IResourceAccessor : IDisposable
    {
        ///<summary>
        /// Retrieves the tracking profile for the specified workflow type 
        /// if one is available.
        ///</summary>
        ///<returns>
        ///true if a <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> for the specified workflow <see cref="T:System.Type"></see> is available; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow for which to get the tracking profile.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        bool TryGetProfile(Type workflowType, out TrackingProfile profile);

        ///<summary>
        /// Returns the tracking profile, qualified by version, for the 
        /// specified workflow <see cref="T:System.Type"></see>. 
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow.</param>
        ///<param name="profileVersion">The <see cref="T:System.Version"></see> of the tracking profile.</param>
        TrackingProfile GetProfile(Type workflowType, Version profileVersion);

        ///<summary>
        /// Returns the tracking profile for the specified workflow instance.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///<param name="instanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        TrackingProfile GetProfile(Guid instanceId);

        ///<summary>
        /// Retrieves a new tracking profile for the specified workflow instance 
        /// if the tracking profile has changed since it was last loaded.
        ///</summary>
        ///<returns>
        ///true if a new <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> should be loaded; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow instance.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        ///<param name="instanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        bool TryReloadProfile(Type workflowType, Guid instanceId, out TrackingProfile profile);


    }
}
