using System;
using System.Collections.Generic;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Tracking.Entity;
using WFTools.Utilities;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Interface that exposes data-access functionality to the
    /// <see cref="GenericTrackingQueryManager" />.
    /// </summary>
    public interface ITrackingQueryResourceAccessor : IDisposable
    {
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
        TrackingWorkflowInstance GetWorkflow(Guid instanceId);

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
        Boolean TryGetWorkflow(Guid instanceId, out TrackingWorkflowInstance trackingWorkflowInstance);

        /// <summary>
        /// Returns tracking data for the entire set of workflow instances that
        /// were invoked by requested workflow instance.
        /// </summary>
        /// <param name="instanceId">
        /// The <see cref="Guid" /> of the workflow instance for which the
        /// tracking data is requested.
        /// </param>
        /// <returns>
        /// A list containing <see cref="TrackingWorkflowInstance" /> objects
        /// that provide access to the tracking data associated with the set of
        /// workflow instances that were invoked by requested workflow instance.
        /// </returns>
        IList<TrackingWorkflowInstance> GetInvokedWorkflows(Guid instanceId);

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
        IList<TrackingWorkflowInstance> GetWorkflows(TrackingQuery query);

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
        SearchResults<TrackingWorkflowInstance> GetWorkflows(TrackingQuery query, Int32 startOffset, Int32 maxResults);

        /// <summary>
        /// Returns a list of <see cref="ActivityTrackingRecord" /> objects
        /// associated with the specified internal identifier of a workflow instance
        /// since the specified date.
        /// </summary>
        /// <param name="internalId">
        /// <see cref="Object" /> representing the unique internal identifier 
        /// of a workflow instance.
        /// </param>
        /// <param name="recordsSince">
        /// <see cref="DateTime" /> indicating the date/time at which
        /// new records should be retrieved. Should always be in UTC.
        /// </param>
        /// <returns>
        /// A list of <see cref="ActivityTrackingRecord" /> objects.
        /// </returns>
        IList<ActivityTrackingRecord> GetActivityEvents(Object internalId, DateTime recordsSince);

        /// <summary>
        /// Returns a list of <see cref="UserTrackingRecord" /> objects
        /// associated with the specified internal identifier of a workflow instance
        /// since the specified date.
        /// </summary>
        /// <param name="internalId">
        /// <see cref="Object" /> representing the unique internal identifier 
        /// of a workflow instance.
        /// </param>
        /// <param name="recordsSince">
        /// <see cref="DateTime" /> indicating the date/time at which
        /// new records should be retrieved. Should always be in UTC.
        /// </param>
        /// <returns>
        /// A list of <see cref="UserTrackingRecord" /> objects.
        /// </returns>
        IList<UserTrackingRecord> GetUserEvents(Object internalId, DateTime recordsSince);

        /// <summary>
        /// Returns a list of <see cref="WorkflowTrackingRecord" /> objects
        /// associated with the specified internal identifier of a workflow instance
        /// since the specified date.
        /// </summary>
        /// <param name="internalId">
        /// <see cref="Object" /> representing the unique internal identifier 
        /// of a workflow instance.
        /// </param>
        /// <param name="recordsSince">
        /// <see cref="DateTime" /> indicating the date/time at which
        /// new records should be retrieved. Should always be in UTC.
        /// </param>
        /// <returns>
        /// A list of <see cref="WorkflowTrackingRecord" /> objects.
        /// </returns>
        IList<WorkflowTrackingRecord> GetWorkflowEvents(Object internalId, DateTime recordsSince);
    }
}