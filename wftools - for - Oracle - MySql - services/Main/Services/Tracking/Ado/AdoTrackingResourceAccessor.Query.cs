using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Common.Ado;
using WFTools.Services.Tracking.Entity;
using WFTools.Utilities;
using WFTools.Utilities.Workflow;

namespace WFTools.Services.Tracking.Ado
{
    //
    // Partial implementation of AdoTrackingResourceAccessor containing 
    // tracking query retrieval methods
    //
    public partial class AdoTrackingResourceAccessor
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
        public TrackingWorkflowInstance GetWorkflow(Guid instanceId)
        {
            TrackingWorkflowInstance trackingWorkflowInstance;
            if (!TryGetWorkflow(instanceId, out trackingWorkflowInstance))
                return null;

            return trackingWorkflowInstance;
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
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetWorkflow), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetWorkflow,
                    TrackingParameterName.InstanceId), instanceId,
                    AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetWorkflow,
                    TrackingParameterName.TrackingWorkflowInstances), instanceId,
                    AdoDbType.Cursor);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        trackingWorkflowInstance = buildTrackingWorkflowInstance(
                            dataReader, TrackingCommandName.GetWorkflow);
                    }
                    else
                        trackingWorkflowInstance = null;
                }
            }

            return trackingWorkflowInstance != null;
        }

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
        public IList<TrackingWorkflowInstance> GetInvokedWorkflows(Guid instanceId)
        {
            List<TrackingWorkflowInstance> trackingWorkflowInstances = new List<TrackingWorkflowInstance>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetInvokedWorkflows), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetInvokedWorkflows,
                    TrackingParameterName.CallerInstanceId), instanceId,
                    AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetInvokedWorkflows,
                    TrackingParameterName.TrackingWorkflowInstances), instanceId,
                    AdoDbType.Cursor);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        trackingWorkflowInstances.Add(buildTrackingWorkflowInstance(
                            dataReader, TrackingCommandName.GetInvokedWorkflows));
                    }
                }
            }

            return trackingWorkflowInstances;
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
            if (query == null)
                throw new ArgumentNullException("query");

            List<TrackingWorkflowInstance> trackingWorkflowInstances = new List<TrackingWorkflowInstance>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetWorkflows), CommandType.StoredProcedure))
            {
                buildQueryParameters(dbCommand, query);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetWorkflows,
                    TrackingParameterName.TrackingWorkflowInstances), 
                    AdoDbType.Cursor, ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        trackingWorkflowInstances.Add(buildTrackingWorkflowInstance(
                            dataReader, TrackingCommandName.GetWorkflows));
                    }
                }
            }

            return trackingWorkflowInstances;
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
            return new SearchResults<TrackingWorkflowInstance>();
        }

        private void buildQueryParameters(DbCommand dbCommand, TrackingQuery query)
        {
            // inspect the query passed to us and add any populated 
            // parameters to the passed database command
            if (query.StatusDateRange != null)
            {
                if (query.StatusDateRange.StartDate.HasValue)
                {
                    AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.GetWorkflows, TrackingParameterName.StartDate),
                        query.StatusDateRange.StartDate.Value, AdoDbType.DateTime);
                }

                if (query.StatusDateRange.EndDate.HasValue)
                {
                    AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.GetWorkflows, TrackingParameterName.EndDate),
                        query.StatusDateRange.EndDate.Value, AdoDbType.DateTime);
                }
            }

            if (query.Statuses.Count > 0)
            {
                // create a comma-delimited representation of the specified query statuses
                String [] statusArray = new String [query.Statuses.Count];
                for (Int32 i = 0 ; i < query.Statuses.Count ; i++)
                    statusArray [i] = ((Int32) query.Statuses [i]).ToString();

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetWorkflows, TrackingParameterName.WorkflowInstanceStatusId),
                    String.Join(",", statusArray), AdoDbType.String);
            }

            //TODO implement this
            if (query.Types.Count > 0)
            {
            }
        }

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
        public IList<ActivityTrackingRecord> GetActivityEvents(Object internalId, DateTime recordsSince)
        {
            return new List<ActivityTrackingRecord>();
        }

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
        public IList<UserTrackingRecord> GetUserEvents(Object internalId, DateTime recordsSince)
        {
            return new List<UserTrackingRecord>();
        }

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
        public IList<WorkflowTrackingRecord> GetWorkflowEvents(Object internalId, DateTime recordsSince)
        {
            return new List<WorkflowTrackingRecord>();
        }

        /// <summary>
        /// Helper method used to build a <see cref="TrackingWorkflowInstance" />
        /// from the specified <see cref="IDataRecord" />.
        /// </summary>
        /// <param name="dataRecord">
        /// An <see cref="IDataRecord" />.
        /// </param>
        /// <returns>
        /// An instance of <see cref="TrackingWorkflowInstance" />.
        /// </returns>
        private TrackingWorkflowInstance buildTrackingWorkflowInstance(IDataRecord dataRecord, TrackingCommandName commandName)
        {
            String assemblyName = ValueReader.GetString(dataRecord,
                _nameResolver.ResolveParameterName(commandName,
                TrackingParameterName.AssemblyFullName));

            String typeName = ValueReader.GetString(dataRecord,
                _nameResolver.ResolveParameterName(commandName,
                TrackingParameterName.TypeFullName));

            return new TrackingWorkflowInstance(
                WorkflowInstanceHelper.DeserialiseActivity(ValueReader.GetString(
                    dataRecord, _nameResolver.ResolveParameterName(commandName,
                    TrackingParameterName.ActivityXoml))),
                ValueReader.GetGuid(dataRecord, _nameResolver.ResolveParameterName(
                    commandName, TrackingParameterName.InstanceId)),
                ValueReader.GetDateTime(dataRecord, _nameResolver.ResolveParameterName(
                    commandName, TrackingParameterName.InitialisedDateTime)),
                ValueReader.GetValue(dataRecord, _nameResolver.ResolveParameterName(
                    commandName, TrackingParameterName.WorkflowInstanceId)),
                ValueReader.GetGuid(dataRecord, _nameResolver.ResolveParameterName(
                    commandName, TrackingParameterName.CallerInstanceId)),
                ValueReader.GetEnum<WorkflowStatus>(dataRecord,
                    _nameResolver.ResolveParameterName(commandName,
                    TrackingParameterName.WorkflowInstanceStatusId)),
                    Type.GetType(String.Format("{0}, {1}", assemblyName, typeName), false));
        }
    }
}
