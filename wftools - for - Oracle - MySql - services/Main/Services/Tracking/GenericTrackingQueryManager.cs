using System;
using System.Collections.Generic;
using WFTools.Services;
using WFTools.Services.Tracking.Entity;
using WFTools.Utilities;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Abstract class used for querying tracking data within some form
    /// of durable storage in a very generic fashion.
    /// 
    /// An <see cref="IResourceProvider" /> and <see cref="ITrackingQueryResourceAccessor" />
    /// work hand-in-hand to actually perform the querying of the tracking store 
    /// and retrieve the results in a form that can be interpreted by the caller.
    /// </summary>
    public abstract class GenericTrackingQueryManager
    {
        private IResourceProvider _resourceProvider;
        /// <summary>
        /// The active <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        private IResourceProvider resourceProvider
        {
            get
            {
                if (_resourceProvider == null)
                    _resourceProvider = CreateResourceProvider();

                return _resourceProvider;
            }
        }

        /// <summary>
        /// Create an <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        /// <remarks>
        /// The resource provider is created upon first access of the 
        /// <see cref="resourceProvider" /> property.
        /// </remarks>
        protected abstract IResourceProvider CreateResourceProvider();

        /// <summary>
        /// Create an <see cref="ITrackingQueryResourceAccessor" /> that is responsible
        /// for querying the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <returns>
        /// A <see cref="ITrackingQueryResourceAccessor" />.
        /// </returns>
        protected abstract ITrackingQueryResourceAccessor CreateAccessor(IResourceProvider resourceProvider);

        /// <summary>
        /// Create an <see cref="ITrackingQueryResourceAccessor" /> that is responsible
        /// for querying the underlying tracking store.
        /// </summary>
        /// <remarks>
        /// This method is used by the <see cref="TrackingWorkflowInstance" /> so that
        /// it can retrieve data from the tracking store during refresh operations.
        /// </remarks>
        /// <returns>
        /// A <see cref="ITrackingQueryResourceAccessor" />.
        /// </returns>
        internal ITrackingQueryResourceAccessor CreateAccessor()
        {
            return CreateAccessor(resourceProvider);
        }

        /// <summary>
        /// Retrieves a <see cref="TrackingWorkflowInstance" /> from the 
        /// tracking store based upon the specified instance identifier.
        /// </summary>
        /// <param name="instanceId">
        /// The <see cref="Guid" /> of the workflow instance for which the
        /// tracking data is requested.
        /// </param>
        /// <returns>
        /// A <see cref="TrackingWorkflowInstance" /> containing tracking data
        /// associated with the workflow instance identifier or <c>null</c> if no
        /// data was found in the tracking store.
        /// </returns>
        public TrackingWorkflowInstance GetWorkflow(Guid instanceId)
        {
            using (ITrackingQueryResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                TrackingWorkflowInstance trackingWorkflowInstance = resourceAccessor.GetWorkflow(instanceId);
                if (trackingWorkflowInstance != null)
                    trackingWorkflowInstance.QueryManager = this;

                return trackingWorkflowInstance;
            }
        }

        /// <summary>
        /// Attempts to retrieve a <see cref="TrackingWorkflowInstance" /> from 
        /// the tracking store based upon the specified instance identifier.
        /// </summary>
        /// <param name="instanceId">
        /// The <see cref="Guid" /> of the workflow instance for which the
        /// tracking data is requested.
        /// </param>
        /// <param name="trackingWorkflowInstance">
        /// When this method returns <c>true</c>, contains a <see cref="TrackingWorkflowInstance" />
        /// that provides access to the tracking data associated with the workflow instance. 
        /// This parameter is passed uninitiailized.
        /// </param>
        /// <returns>
        /// <c>true</c> if tracking data is available for the requested workflow 
        /// instance, <c>false</c> otherwise.
        /// </returns>
        public Boolean TryGetWorkflow(Guid instanceId, out TrackingWorkflowInstance trackingWorkflowInstance)
        {
            using (ITrackingQueryResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                Boolean hasTrackingData = resourceAccessor.TryGetWorkflow(instanceId, out trackingWorkflowInstance);
                if (trackingWorkflowInstance != null)
                    trackingWorkflowInstance.QueryManager = this;

                return hasTrackingData;
            }
        }

        /// <summary>
        /// Returns tracking data for the entire set of workflow instances that
        /// match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// A <see cref="TrackingQuery" /> containing query criteria.
        /// </param>
        /// <returns>
        /// A list containing <see cref="TrackingWorkflowInstance" /> objects
        /// that provide access to the tracking data associated with the set of
        /// workflow instances that match the specified query criteria.
        /// </returns>
        public IList<TrackingWorkflowInstance> GetWorkflows(TrackingQuery query)
        {
            using (ITrackingQueryResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                IList<TrackingWorkflowInstance> trackingWorkflowInstances = resourceAccessor.GetWorkflows(query);
                foreach (TrackingWorkflowInstance trackingWorkflowInstance in trackingWorkflowInstances)
                    trackingWorkflowInstance.QueryManager = this;

                return trackingWorkflowInstances;
            }
        }

        /// <summary>
        /// Returns tracking data for a paged set of workflow instances that
        /// match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// A <see cref="TrackingQuery" /> containing query criteria.
        /// </param>
        /// <param name="startOffset">
        /// <see cref="Int32" /> indicating the start offset in the entire
        /// set of matching workflow instances to begin returning records.
        /// </param>
        /// <param name="maxResults">
        /// <see cref="Int32" /> indicating the maximum number of records
        /// to return from <paramref name="startOffset"/>.
        /// </param>
        /// <returns>
        /// A <see cref="SearchResults{T}" /> containing <see cref="TrackingWorkflowInstance" /> 
        /// objects that provide access to the tracking data associated with a paged set of
        /// workflow instances that match the specified query criteria.
        /// </returns>
        public SearchResults<TrackingWorkflowInstance> GetWorkflows(TrackingQuery query, Int32 startOffset, Int32 maxResults)
        {
            using (ITrackingQueryResourceAccessor resourceAccessor = CreateAccessor(resourceProvider))
            {
                SearchResults<TrackingWorkflowInstance> searchResults = resourceAccessor.GetWorkflows(query, startOffset, maxResults);
                foreach (TrackingWorkflowInstance trackingWorkflowInstance in searchResults.Results)
                    trackingWorkflowInstance.QueryManager = this;

                return searchResults;
            }
        }
    }
}