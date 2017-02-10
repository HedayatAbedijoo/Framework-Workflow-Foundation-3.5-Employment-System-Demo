using System;
using System.Workflow.Runtime.Tracking;
using WFTools.Utilities.Diagnostics;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Abstract implementation of <see cref="TrackingService" /> that provides the
    /// framework necessary for a very generic tracking service. 
    /// 
    /// A resource provider and accessor work hand-in-hand to actually
    /// persist workflow tracking information to the persistence store.
    /// </summary>
    public abstract class GenericWorkflowTrackingService : TrackingService
    {
        protected GenericWorkflowTrackingService() { }

        /// <summary>
        /// The active <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        private IResourceProvider resourceProvider;

        /// <summary>
        /// Create an <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        /// <remarks>
        /// The resource provider is created once upon Start of the 
        /// tracking service.
        /// </remarks>
        protected abstract IResourceProvider CreateResourceProvider();

        /// <summary>
        /// Create an <see cref="IResourceAccessor" /> that is responsible
        /// for manipulating the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected abstract IResourceAccessor CreateAccessor(IResourceProvider resourceProvider);

        /// <summary>
        /// Perform startup duties associated with this persistence service.
        /// <remarks>
        /// This implementation calls a virtual method to create a single 
        /// resource provider for this persistence service.
        /// </remarks>
        /// </summary>
        protected override void Start()
        {
            TraceHelper.Trace();

            // retrieve the active resource provider
            this.resourceProvider = CreateResourceProvider();

            base.Start();
        }

        ///<summary>
        ///Must be overridden in the derived class, and when implemented, retrieves the tracking profile for the specified workflow type if one is available.
        ///</summary>
        ///
        ///<returns>
        ///true if a <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> for the specified workflow <see cref="T:System.Type"></see> is available; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow for which to get the tracking profile.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        protected override bool TryGetProfile(Type workflowType, out TrackingProfile profile)
        {
            using (IResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                return resourceAccessor.TryGetProfile(workflowType, out profile);
            }
        }

        ///<summary>
        ///Must be overridden in the derived class, and when implemented, returns the tracking profile, qualified by version, for the specified workflow <see cref="T:System.Type"></see>. 
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow.</param>
        ///<param name="profileVersionId">The <see cref="T:System.Version"></see> of the tracking profile.</param>
        protected override TrackingProfile GetProfile(Type workflowType, Version profileVersionId)
        {
            using (IResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                return resourceAccessor.GetProfile(workflowType, profileVersionId);
            }
        }

        ///<summary>
        ///Must be overridden in the derived class, and when implemented, returns the tracking profile for the specified workflow instance.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///
        ///<param name="workflowInstanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        protected override TrackingProfile GetProfile(Guid workflowInstanceId)
        {
            using (IResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                return resourceAccessor.GetProfile(workflowInstanceId);
            }
        }

        ///<summary>
        ///Must be overridden in the derived class, and when implemented, retrieves a new tracking profile for the specified workflow instance if the tracking profile has changed since it was last loaded.
        ///</summary>
        ///
        ///<returns>
        ///true if a new <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> should be loaded; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow instance.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        ///<param name="workflowInstanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        protected override bool TryReloadProfile(Type workflowType, Guid workflowInstanceId, out TrackingProfile profile)
        {
            using (IResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                return resourceAccessor.TryReloadProfile(workflowType, workflowInstanceId, out profile);
            }
        }
    }
}