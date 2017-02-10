using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using WFTools.Services.Tracking.Entity;
using System.Data;
using WFTools.Services.Common.Ado;
using System.Xml;
using System.Workflow.ComponentModel;

namespace WFTools.Services.Tracking.Ado
{
    //
    // Partial implementation of AdoTrackingResourceAccessor containing 
    // tracking service and channel methods
    //
    public partial class AdoTrackingResourceAccessor
    {
        /// <summary>
        /// The number of activity added actions to batch up when persisting.
        /// </summary>
        protected virtual Int32 ActivityAddedActionBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of activity removed actions to batch up when persisting.
        /// </summary>
        protected virtual Int32 ActivityRemovedActionBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of activity tracking records to batch up when persisting.
        /// </summary>
        protected virtual Int32 ActivityTrackingBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of event annotations to batch up when persisting.
        /// </summary>
        protected virtual Int32 EventAnnotationBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of tracking data item annotations to batch up when persisting.
        /// </summary>
        protected virtual Int32 TrackingDataItemAnnotationBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of tracking data items to batch up when persisting.
        /// </summary>
        protected virtual Int32 TrackingDataItemBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of user tracking records to batch up when persisting.
        /// </summary>
        protected virtual Int32 UserTrackingBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of workflow change tracking records to batch up when persisting.
        /// </summary>
        protected virtual Int32 WorkflowChangeBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// The number of workflow tracking records to batch up when persisting.
        /// </summary>
        protected virtual Int32 WorkflowTrackingBatchSize
        {
            get { return 5; }
        }

        /// <summary>
        /// Create a new workflow instance record or return the existing
        /// record if any exists.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// and any type information applicable to it.
        /// </param>
        public WorkflowInstanceSummary InsertOrGetWorkflowInstance(WorkflowInstanceSummary workflowInstanceSummary)
        {
            if (!InsertWorkflowType(workflowInstanceSummary))
                InsertActivities(workflowInstanceSummary);

            InsertWorkflowInstance(workflowInstanceSummary);

            return workflowInstanceSummary;
        }

        /// <summary>
        /// Insert a record for the workflow type.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <returns>
        /// A <see cref="Boolean" /> that indicates whether the
        /// workflow type was already inserted.
        /// </returns>
        protected virtual Boolean InsertWorkflowType(WorkflowInstanceSummary workflowInstanceSummary)
        {
            Boolean exists;

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertWorkflow), CommandType.StoredProcedure))
            {
                String assemblyFullName;
                String typeFullName;
                if (workflowInstanceSummary.IsXomlWorkflow)
                {
                    assemblyFullName = workflowInstanceSummary.InstanceId.ToString();
                    typeFullName = workflowInstanceSummary.InstanceId.ToString();
                }
                else
                {
                    assemblyFullName = workflowInstanceSummary.WorkflowType.Type.Assembly.FullName;
                    typeFullName = workflowInstanceSummary.WorkflowType.Type.FullName;
                }

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.TypeFullName),
                    typeFullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.AssemblyFullName),
                    assemblyFullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.IsInstanceType),
                    workflowInstanceSummary.IsXomlWorkflow, AdoDbType.Boolean);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.WorkflowDefinition),
                    workflowInstanceSummary.XomlDocument, AdoDbType.Text);

                String workflowTypeIdParameter = _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.WorkflowTypeId);

                AddParameter(dbCommand, workflowTypeIdParameter,
                    AdoDbType.Int32, ParameterDirection.Output);

                String existsParameter = _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflow,
                    TrackingParameterName.Exists);

                AddParameter(dbCommand, existsParameter,
                    AdoDbType.Boolean, ParameterDirection.Output);

                dbCommand.ExecuteNonQuery();

                workflowInstanceSummary.WorkflowType.InternalId =
                    _valueReader.GetInt32(dbCommand, workflowTypeIdParameter);

                exists = _valueReader.GetBoolean(dbCommand, existsParameter);
            }

            return exists;
        }

        /// <summary>
        /// Inserts the activities associated with a workflow type.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <remarks>
        /// This implementation expects the underlying database to support
        /// some form of XML parsing as it constructs an XML document and passes
        /// it to a command.
        /// </remarks>
        protected virtual void InsertActivities(WorkflowInstanceSummary workflowInstanceSummary)
        {
            String activityXml = buildActivityXml(workflowInstanceSummary.RootActivity);

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertActivities), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertActivities,
                    TrackingParameterName.WorkflowTypeId),
                    workflowInstanceSummary.WorkflowType.InternalId,
                    AdoDbType.Int32);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertActivities,
                    TrackingParameterName.ActivityXml),
                    activityXml, AdoDbType.Text);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Build an XML document containing the activities associated with a 
        /// workflow instance.
        /// </summary>
        /// <param name="rootActivity">
        /// <see cref="ActivitySummary" /> representing the root activity of the
        /// workflow instance.
        /// </param>
        /// <returns>
        /// A String representing the XML document generated from the root activity.
        /// </returns>
        private static String buildActivityXml(ActivitySummary rootActivity)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Activities");

                Queue<ActivitySummary> activityQueue = new Queue<ActivitySummary>();
                activityQueue.Enqueue(rootActivity);
                while (activityQueue.Count > 0)
                {
                    ActivitySummary currentActivity = activityQueue.Dequeue();
                    xmlWriter.WriteStartElement("Activity");
                    xmlWriter.WriteElementString("TypeFullName", currentActivity.Type.FullName);
                    xmlWriter.WriteElementString("AssemblyFullName", currentActivity.Type.Assembly.FullName);
                    xmlWriter.WriteElementString("QualifiedName", currentActivity.QualifiedName);

                    if (currentActivity.ParentActivity != null)
                        xmlWriter.WriteElementString("ParentQualifiedName", currentActivity.ParentActivity.QualifiedName);

                    xmlWriter.WriteEndElement();

                    foreach (ActivitySummary childActivity in currentActivity.ChildActivities)
                        activityQueue.Enqueue(childActivity);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Insert a record for the workflow instance.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance.
        /// </param>
        protected virtual void InsertWorkflowInstance(WorkflowInstanceSummary workflowInstanceSummary)
        {
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertWorkflowInstance), CommandType.StoredProcedure))
            {
                String assemblyFullName;
                String typeFullName;
                if (workflowInstanceSummary.IsXomlWorkflow)
                {
                    assemblyFullName = workflowInstanceSummary.InstanceId.ToString();
                    typeFullName = workflowInstanceSummary.InstanceId.ToString();
                }
                else
                {
                    assemblyFullName = workflowInstanceSummary.WorkflowType.Type.Assembly.FullName;
                    typeFullName = workflowInstanceSummary.WorkflowType.Type.FullName;
                }

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.InstanceId),
                    workflowInstanceSummary.InstanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.TypeFullName),
                    typeFullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.AssemblyFullName),
                    assemblyFullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.ContextGuid),
                    workflowInstanceSummary.ContextGuid, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.CallerInstanceId),
                    workflowInstanceSummary.CallerInstanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.CallPath),
                    workflowInstanceSummary.CallPath, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.CallerContextGuid),
                    workflowInstanceSummary.CallerContextGuid, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.CallerParentContextGuid),
                    workflowInstanceSummary.CallerParentContextGuid, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.InitialisedDateTime),
                    DateTime.UtcNow, AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowInstance,
                    TrackingParameterName.WorkflowInstanceId),
                    AdoDbType.Cursor, ParameterDirection.Output);

                using (DbDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        workflowInstanceSummary.InternalId = _valueReader.GetInt64(dataReader, 0);
                }
            }
        }

        /// <summary>
        /// Inserts a batch of activity tracking records, returning them
        /// as an <see cref="IDictionary{TKey,TValue}" /> indexed by their
        /// unique identifiers.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="activityTrackingRecords">
        /// An <see cref="IList{T}" /> containing a batch of activity tracking 
        /// records to be inserted into the tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IDictionary{TKey,TValue}" /> containing the inserted
        /// records and indexed by their unique identifiers.
        /// </returns>
        protected virtual IDictionary<Int64, SerialisableActivityTrackingRecord> InsertActivityTrackingRecordBatch(
            WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableActivityTrackingRecord> activityTrackingRecords)
        {
            if (activityTrackingRecords.Count == 0 || activityTrackingRecords.Count > ActivityTrackingBatchSize)
                throw new ArgumentOutOfRangeException("activityTrackingRecords");

            Dictionary<Int64, SerialisableActivityTrackingRecord> activityTrackingRecordsById =
                new Dictionary<Int64, SerialisableActivityTrackingRecord>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertActivityTrackingRecords), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertActivityTrackingRecords,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                String [] activityIdentifiers = new String [activityTrackingRecords.Count];
                for (Int32 i = 0 ; i < ActivityTrackingBatchSize ; i++)
                {
                    if (i < activityTrackingRecords.Count)
                    {
                        SerialisableActivityTrackingRecord activityTrackingRecord = activityTrackingRecords [i];

                        activityIdentifiers [i] = BuildActivityIdentifier(activityTrackingRecord);

                        // do we have an identifier for this activity already?
                        Int64? activityInstanceId = FindActivityInstanceInternalId(activityIdentifiers [i]);
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i),
                            activityInstanceId, AdoDbType.Int64, ParameterDirection.InputOutput);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.QualifiedName, i),
                            activityIdentifiers [i], AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ContextGuid, i),
                            activityTrackingRecord.ContextGuid, AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ParentContextGuid, i),
                            activityTrackingRecord.ParentContextGuid, AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityStatusId, i),
                            (Int16) activityTrackingRecord.ExecutionStatus, AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.EventDateTime, i),
                            activityTrackingRecord.EventDateTime, AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.EventOrder, i),
                            activityTrackingRecord.EventOrder, AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityStatusEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i),
                            null, AdoDbType.Int64, ParameterDirection.InputOutput);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.QualifiedName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ContextGuid, i), null,
                            AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ParentContextGuid, i), null,
                            AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityStatusId, i), null,
                            AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.EventDateTime, i), null,
                            AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.EventOrder, i), null,
                            AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityStatusEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                }

                dbCommand.ExecuteNonQuery();

                for (Int32 i = 0 ; i < activityTrackingRecords.Count ; i++)
                {
                    if (activityTrackingRecords [i].ExecutionStatus == ActivityExecutionStatus.Closed)
                    {
                        // the activity was completed, remove its identifier from 
                        // our in-memory cache of activity instance identifiers
                        RemoveActivityInstanceInternalId(activityIdentifiers [i]);
                    }
                    else
                    {
                        // create a mapping between the activity instance and 
                        // its unique identifier in the database
                        UpdateActivityInstanceInternalId(activityIdentifiers [i],
                            ValueReader.GetInt64(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i)));
                    }

                    activityTrackingRecordsById.Add(ValueReader.GetInt64(
                        dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityTrackingRecords,
                            TrackingParameterName.ActivityStatusEventId, i)),
                        activityTrackingRecords [i]);
                }
            }

            return activityTrackingRecordsById;
        }

        /// <summary>
        /// Insert a list of activity tracking records into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="activityTrackingRecords">
        /// A list of <see cref="SerialisableActivityTrackingRecord" /> objects
        /// that should be inserted into the tracking store.
        /// </param>
        public void InsertTrackingRecords(WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableActivityTrackingRecord> activityTrackingRecords)
        {
            List<KeyValuePair<Int64, String>> annotations = new List<KeyValuePair<Int64, String>>();
            List<KeyValuePair<Int64, SerialisableTrackingDataItem>> dataItems =
                new List<KeyValuePair<Int64, SerialisableTrackingDataItem>>();

            batch(activityTrackingRecords, ActivityTrackingBatchSize,
                delegate(IList<SerialisableActivityTrackingRecord> activityTrackingRecordBatch)
                {
                    IDictionary<Int64, SerialisableActivityTrackingRecord>
                        activityTrackingRecordsById = InsertActivityTrackingRecordBatch(
                            workflowInstanceSummary, activityTrackingRecordBatch);

                    foreach (Int64 id in activityTrackingRecordsById.Keys)
                    {
                        foreach (String annotation in activityTrackingRecordsById [id].Annotations)
                            annotations.Add(new KeyValuePair<Int64, String>(id, annotation));

                        foreach (SerialisableTrackingDataItem dataItem in activityTrackingRecordsById [id].Body)
                            dataItems.Add(new KeyValuePair<Int64, SerialisableTrackingDataItem>(id, dataItem));
                    }
                });

            insertEventAnnotations(workflowInstanceSummary, TrackingRecordType.Activity, annotations);
            insertTrackingDataItems(workflowInstanceSummary, TrackingRecordType.Activity, dataItems);
        }

        /// <summary>
        /// Inserts a batch of user tracking records, returning them
        /// as an <see cref="IDictionary{TKey,TValue}" /> indexed by their
        /// unique identifiers.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="userTrackingRecords">
        /// An <see cref="IList{T}" /> containing a batch of user tracking 
        /// records to be inserted into the tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IDictionary{TKey,TValue}" /> containing the inserted
        /// records and indexed by their unique identifiers.
        /// </returns>
        protected virtual IDictionary<Int64, SerialisableUserTrackingRecord> InsertUserTrackingRecordBatch(
            WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableUserTrackingRecord> userTrackingRecords)
        {
            if (userTrackingRecords.Count == 0 || userTrackingRecords.Count > UserTrackingBatchSize)
                throw new ArgumentOutOfRangeException("userTrackingRecords");

            Dictionary<Int64, SerialisableUserTrackingRecord> userTrackingRecordsById =
                new Dictionary<Int64, SerialisableUserTrackingRecord>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertUserTrackingRecords), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertUserTrackingRecords,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                String [] activityIdentifiers = new String [userTrackingRecords.Count];
                for (Int32 i = 0 ; i < UserTrackingBatchSize ; i++)
                {
                    if (i < userTrackingRecords.Count)
                    {
                        SerialisableUserTrackingRecord userTrackingRecord = userTrackingRecords [i];
                        activityIdentifiers [i] = BuildActivityIdentifier(userTrackingRecord);

                        // do we have an identifier for this activity already?
                        Int64? activityInstanceId = FindActivityInstanceInternalId(activityIdentifiers [i]);
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i),
                            activityInstanceId, AdoDbType.Int64,
                            ParameterDirection.InputOutput);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.QualifiedName, i),
                            userTrackingRecord.QualifiedName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ContextGuid, i),
                            userTrackingRecord.ContextGuid, AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ParentContextGuid, i),
                            userTrackingRecord.ParentContextGuid, AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.EventDateTime, i),
                            userTrackingRecord.EventDateTime, AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.EventOrder, i),
                            userTrackingRecord.EventOrder, AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataKey, i),
                            userTrackingRecord.UserDataKey, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataTypeFullName, i),
                            userTrackingRecord.UserData.Type.FullName,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataAssemblyFullName, i),
                            userTrackingRecord.UserData.Type.Assembly.FullName,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataString, i),
                            userTrackingRecord.UserData.StringData,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataBlob, i),
                            userTrackingRecord.UserData.SerialisedData,
                            AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataNonSerialisable, i),
                            userTrackingRecord.UserData.NonSerialisable,
                            AdoDbType.Boolean);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i),
                            null, AdoDbType.Int64, ParameterDirection.InputOutput);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.QualifiedName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ContextGuid, i), null,
                            AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ParentContextGuid, i), null,
                            AdoDbType.Guid);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.EventDateTime, i), null,
                            AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.EventOrder, i), null,
                            AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataKey, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataTypeFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataAssemblyFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataString, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataBlob, i),
                            null, AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserDataNonSerialisable, i),
                            null, AdoDbType.Boolean);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                }

                dbCommand.ExecuteNonQuery();

                for (Int32 i = 0 ; i < userTrackingRecords.Count ; i++)
                {
                    UpdateActivityInstanceInternalId(activityIdentifiers [i],
                        ValueReader.GetInt64(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.ActivityInstanceId, i)));

                    userTrackingRecordsById.Add(ValueReader.GetInt64(
                        dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertUserTrackingRecords,
                            TrackingParameterName.UserEventId, i)),
                        userTrackingRecords [i]);
                }
            }

            return userTrackingRecordsById;
        }

        /// <summary>
        /// Insert a list of user tracking records into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="userTrackingRecords">
        /// A list of <see cref="SerialisableUserTrackingRecord" /> objects
        /// that should be inserted into the tracking store.
        /// </param>
        public void InsertTrackingRecords(WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableUserTrackingRecord> userTrackingRecords)
        {
            List<KeyValuePair<Int64, String>> annotations = new List<KeyValuePair<Int64, String>>();
            List<KeyValuePair<Int64, SerialisableTrackingDataItem>> dataItems =
                new List<KeyValuePair<Int64, SerialisableTrackingDataItem>>();

            batch(userTrackingRecords, UserTrackingBatchSize,
                delegate(IList<SerialisableUserTrackingRecord> userTrackingRecordBatch)
                {
                    IDictionary<Int64, SerialisableUserTrackingRecord>
                        userTrackingRecordsById = InsertUserTrackingRecordBatch(
                            workflowInstanceSummary, userTrackingRecordBatch);

                    foreach (Int64 id in userTrackingRecordsById.Keys)
                    {
                        foreach (String annotation in userTrackingRecordsById [id].Annotations)
                            annotations.Add(new KeyValuePair<Int64, String>(id, annotation));

                        foreach (SerialisableTrackingDataItem dataItem in userTrackingRecordsById [id].Body)
                            dataItems.Add(new KeyValuePair<Int64, SerialisableTrackingDataItem>(id, dataItem));
                    }
                });

            insertEventAnnotations(workflowInstanceSummary, TrackingRecordType.User, annotations);
            insertTrackingDataItems(workflowInstanceSummary, TrackingRecordType.User, dataItems);
        }

        /// <summary>
        /// Inserts a batch of workflow tracking records, returning them
        /// as an <see cref="IDictionary{TKey,TValue}" /> indexed by their
        /// unique identifiers.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="workflowTrackingRecords">
        /// An <see cref="IList{T}" /> containing a batch of workflow tracking 
        /// records to be inserted into the tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IDictionary{TKey,TValue}" /> containing the inserted
        /// records and indexed by their unique identifiers.
        /// </returns>
        protected virtual IDictionary<Int64, SerialisableWorkflowTrackingRecord> InsertWorkflowTrackingRecordBatch(
            WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableWorkflowTrackingRecord> workflowTrackingRecords)
        {
            if (workflowTrackingRecords.Count == 0 || workflowTrackingRecords.Count > WorkflowTrackingBatchSize)
                throw new ArgumentOutOfRangeException("workflowTrackingRecords");

            Dictionary<Int64, SerialisableWorkflowTrackingRecord> workflowTrackingRecordsById =
                new Dictionary<Int64, SerialisableWorkflowTrackingRecord>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertWorkflowTrackingRecords), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowTrackingRecords,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                for (Int32 i = 0 ; i < WorkflowTrackingBatchSize ; i++)
                {
                    if (i < workflowTrackingRecords.Count)
                    {
                        SerialisableWorkflowTrackingRecord workflowTrackingRecord = workflowTrackingRecords [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceStatusId, i),
                            (Int16) workflowTrackingRecord.TrackingWorkflowEvent,
                            AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventDateTime, i),
                            workflowTrackingRecord.EventDateTime, AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventOrder, i),
                            workflowTrackingRecord.EventOrder, AdoDbType.Int32);

                        if (workflowTrackingRecord.EventArgs != null)
                        {
                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsTypeFullName, i),
                                workflowTrackingRecord.EventArgs.Type.FullName,
                                AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsAssemblyFullName, i),
                                workflowTrackingRecord.EventArgs.Type.Assembly.FullName,
                                AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgs, i),
                                workflowTrackingRecord.EventArgs.SerialisedData,
                                AdoDbType.Binary);
                        }
                        else
                        {
                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsTypeFullName, i),
                                null, AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsAssemblyFullName, i),
                                null, AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgs, i), null,
                                AdoDbType.Binary);
                        }

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceStatusId, i),
                            null, AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventDateTime, i), null,
                            AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventOrder, i), null,
                            AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgsTypeFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgsAssemblyFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgs, i), null,
                            AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                }

                dbCommand.ExecuteNonQuery();

                for (Int32 i = 0 ; i < workflowTrackingRecords.Count ; i++)
                {
                    workflowTrackingRecordsById.Add(ValueReader.GetInt64(
                        dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.InsertWorkflowTrackingRecords,
                        TrackingParameterName.WorkflowInstanceEventId, i)),
                        workflowTrackingRecords [i]);
                }
            }

            return workflowTrackingRecordsById;
        }

        /// <summary>
        /// Insert a list of workflow tracking records into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="workflowTrackingRecords">
        /// A list of <see cref="SerialisableWorkflowTrackingRecord" /> objects
        /// that should be inserted into the tracking store.
        /// </param>
        public void InsertTrackingRecords(WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableWorkflowTrackingRecord> workflowTrackingRecords)
        {
            List<KeyValuePair<Int64, String>> annotations = new List<KeyValuePair<Int64, String>>();

            batch(workflowTrackingRecords, WorkflowChangeBatchSize,
                delegate(IList<SerialisableWorkflowTrackingRecord> workflowTrackingRecordBatch)
                {
                    IDictionary<Int64, SerialisableWorkflowTrackingRecord>
                        workflowTrackingRecordsById = InsertWorkflowTrackingRecordBatch(
                            workflowInstanceSummary, workflowTrackingRecordBatch);

                    foreach (Int64 id in workflowTrackingRecordsById.Keys)
                    {
                        foreach (String annotation in workflowTrackingRecordsById [id].Annotations)
                            annotations.Add(new KeyValuePair<Int64, String>(id, annotation));
                    }
                });

            insertEventAnnotations(workflowInstanceSummary, TrackingRecordType.Workflow, annotations);
        }



        /// <summary>
        /// Insert a list of workflow change actions into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="actions">
        /// A list of <see cref="KeyValuePair{TKey,TValue}" /> objects
        /// containing the parent identifier and the child item to persist.
        /// </param>
        private void insertActions(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, SerialisableWorkflowChangeAction>> actions)
        {
            List<KeyValuePair<Int64, SerialisableActivityAddedAction>> addedActions =
                new List<KeyValuePair<Int64, SerialisableActivityAddedAction>>();
            List<KeyValuePair<Int64, SerialisableActivityRemovedAction>> removedActions =
                new List<KeyValuePair<Int64, SerialisableActivityRemovedAction>>();

            foreach (KeyValuePair<Int64, SerialisableWorkflowChangeAction> action in actions)
            {
                Type actionType = action.Value.GetType();
                if (actionType == typeof(SerialisableActivityAddedAction))
                {
                    addedActions.Add(
                        new KeyValuePair<Int64, SerialisableActivityAddedAction>(
                            action.Key, (SerialisableActivityAddedAction) action.Value));
                }
                else if (actionType == typeof(SerialisableActivityRemovedAction))
                {
                    removedActions.Add(
                        new KeyValuePair<Int64, SerialisableActivityRemovedAction>(
                            action.Key, (SerialisableActivityRemovedAction) action.Value));
                }
            }

            if (addedActions.Count > 0)
                insertActivityAddedActions(workflowInstanceSummary, addedActions);

            if (removedActions.Count > 0)
                insertActivityRemovedActions(workflowInstanceSummary, removedActions);

            // TODO support the rest of the action types
        }

        /// <summary>
        /// Insert a list of activity added actions into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="addedActions">
        /// A list of <see cref="KeyValuePair{TKey,TValue}" /> objects
        /// containing the parent identifier and the child item to persist.
        /// </param>
        private void insertActivityAddedActions(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, SerialisableActivityAddedAction>> addedActions)
        {
            batch(addedActions, ActivityTrackingBatchSize,
                delegate(IList<KeyValuePair<Int64, SerialisableActivityAddedAction>> addedActionBatch)
                {
                    InsertActivityAddedActionBatch(workflowInstanceSummary, addedActionBatch);
                });
        }

        /// <summary>
        /// Inserts a batch of workflow change tracking records, returning them
        /// as an <see cref="IDictionary{TKey,TValue}" /> indexed by their
        /// unique identifiers.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="workflowChangeRecords">
        /// An <see cref="IList{T}" /> containing a batch of workflow change 
        /// records to be inserted into the tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IDictionary{TKey,TValue}" /> containing the inserted
        /// records and indexed by their unique identifiers.
        /// </returns>
        protected virtual IDictionary<Int64, SerialisableWorkflowChangeRecord> InsertWorkflowChangeRecordBatch(
            WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableWorkflowChangeRecord> workflowChangeRecords)
        {
            if (workflowChangeRecords.Count == 0 || workflowChangeRecords.Count > WorkflowChangeBatchSize)
                throw new ArgumentOutOfRangeException("workflowChangeRecords");

            Dictionary<Int64, SerialisableWorkflowChangeRecord> workflowChangeRecordsById =
                new Dictionary<Int64, SerialisableWorkflowChangeRecord>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertWorkflowTrackingRecords), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertWorkflowTrackingRecords,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                for (Int32 i = 0 ; i < WorkflowChangeBatchSize ; i++)
                {
                    if (i < workflowChangeRecords.Count)
                    {
                        SerialisableWorkflowChangeRecord workflowChangeRecord = workflowChangeRecords [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceStatusId, i),
                            (Int16) workflowChangeRecord.TrackingWorkflowEvent,
                            AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventDateTime, i),
                            workflowChangeRecord.EventDateTime, AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventOrder, i),
                            workflowChangeRecord.EventOrder, AdoDbType.Int32);

                        if (workflowChangeRecord.EventArgs != null)
                        {
                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsTypeFullName, i),
                                workflowChangeRecord.EventArgs.Type.FullName,
                                AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsAssemblyFullName, i),
                                workflowChangeRecord.EventArgs.Type.Assembly.FullName,
                                AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgs, i),
                                workflowChangeRecord.EventArgs.SerialisedData,
                                AdoDbType.Binary);
                        }
                        else
                        {
                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsTypeFullName, i),
                                null, AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgsAssemblyFullName, i),
                                null, AdoDbType.String);

                            AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                                TrackingCommandName.InsertWorkflowTrackingRecords,
                                TrackingParameterName.EventArgs, i), null,
                                AdoDbType.Binary);
                        }

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceStatusId, i),
                            null, AdoDbType.Int16);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventDateTime, i), null,
                            AdoDbType.DateTime);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventOrder, i), null,
                            AdoDbType.Int32);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgsTypeFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgsAssemblyFullName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.EventArgs, i), null,
                            AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertWorkflowTrackingRecords,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                }

                dbCommand.ExecuteNonQuery();

                for (Int32 i = 0 ; i < workflowChangeRecords.Count ; i++)
                {
                    workflowChangeRecordsById.Add(ValueReader.GetInt64(
                        dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.InsertWorkflowTrackingRecords,
                        TrackingParameterName.WorkflowInstanceEventId, i)),
                        workflowChangeRecords [i]);
                }
            }

            return workflowChangeRecordsById;
        }

        /// <summary>
        /// Insert a list of workflow change tracking records into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="workflowChangeRecords">
        /// A list of <see cref="SerialisableWorkflowChangeRecord" /> objects
        /// that should be inserted into the tracking store.
        /// </param>
        public void InsertTrackingRecords(WorkflowInstanceSummary workflowInstanceSummary, IList<SerialisableWorkflowChangeRecord> workflowChangeRecords)
        {
            List<KeyValuePair<Int64, String>> annotations = new List<KeyValuePair<Int64, String>>();
            List<KeyValuePair<Int64, SerialisableWorkflowChangeAction>> actions = new List<KeyValuePair<long, SerialisableWorkflowChangeAction>>();

            batch(workflowChangeRecords, WorkflowChangeBatchSize,
                delegate(IList<SerialisableWorkflowChangeRecord> workflowChangeRecordBatch)
                {
                    IDictionary<Int64, SerialisableWorkflowChangeRecord>
                        workflowChangeRecordsById = InsertWorkflowChangeRecordBatch(
                            workflowInstanceSummary, workflowChangeRecordBatch);

                    foreach (Int64 id in workflowChangeRecordsById.Keys)
                    {
                        foreach (String annotation in workflowChangeRecordsById [id].Annotations)
                            annotations.Add(new KeyValuePair<Int64, String>(id, annotation));

                        foreach (SerialisableWorkflowChangeAction action in workflowChangeRecordsById [id].EventArgs.Changes)
                            actions.Add(new KeyValuePair<long, SerialisableWorkflowChangeAction>(id, action));
                    }
                });

            insertActions(workflowInstanceSummary, actions);
            insertEventAnnotations(workflowInstanceSummary, TrackingRecordType.Workflow, annotations);
        }

        /// <summary>
        /// Inserts a batch of activity added actions.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="addedActions">
        /// An <see cref="IList{T}" /> containing a batch of activity added
        /// actions to be inserted into the tracking store.
        /// </param>
        protected virtual void InsertActivityAddedActionBatch(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, SerialisableActivityAddedAction>> addedActions)
        {
            if (addedActions.Count == 0 || addedActions.Count > ActivityAddedActionBatchSize)
                throw new ArgumentOutOfRangeException("addedActions");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertActivityAddedActions), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertActivityAddedActions,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                for (Int32 i = 0 ; i < ActivityAddedActionBatchSize ; i++)
                {
                    if (i < addedActions.Count)
                    {
                        KeyValuePair<Int64, SerialisableActivityAddedAction> addedAction = addedActions [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            addedAction.Key, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.QualifiedName, i),
                            addedAction.Value.QualifiedName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.TypeFullName, i),
                            addedAction.Value.ActivityType.FullName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.AssemblyFullName, i),
                            addedAction.Value.ActivityType.Assembly.FullName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.ParentQualifiedName, i),
                            addedAction.Value.ParentQualifiedName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.ActivityXoml, i),
                            addedAction.Value.ActivityXoml, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.Order, i),
                            addedAction.Value.Order, AdoDbType.Int32);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            null, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.QualifiedName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.TypeFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.AssemblyFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.ParentQualifiedName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.ActivityXoml, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityAddedActions,
                            TrackingParameterName.Order, i), null,
                            AdoDbType.Int32);
                    }
                }

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Insert a list of activity added actions into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="removedActions">
        /// A list of <see cref="KeyValuePair{TKey,TValue}" /> objects
        /// containing the parent identifier and the child item to persist.
        /// </param>
        private void insertActivityRemovedActions(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, SerialisableActivityRemovedAction>> removedActions)
        {
            batch(removedActions, ActivityRemovedActionBatchSize,
                delegate(IList<KeyValuePair<Int64, SerialisableActivityRemovedAction>> removedActionBatch)
                {
                    InsertActivityRemovedActionBatch(workflowInstanceSummary,
                        removedActionBatch);
                });
        }

        /// <summary>
        /// Inserts a batch of activity removed actions.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="removedActions">
        /// An <see cref="IList{T}" /> containing a batch of activity removed
        /// actions to be inserted into the tracking store.
        /// </param>
        protected virtual void InsertActivityRemovedActionBatch(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, SerialisableActivityRemovedAction>> removedActions)
        {
            if (removedActions.Count == 0 || removedActions.Count > ActivityRemovedActionBatchSize)
                throw new ArgumentOutOfRangeException("removedActions");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertActivityRemovedActions), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertActivityRemovedActions,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                for (Int32 i = 0 ; i < ActivityRemovedActionBatchSize ; i++)
                {
                    if (i < removedActions.Count)
                    {
                        KeyValuePair<Int64, SerialisableActivityRemovedAction> removedAction = removedActions [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            removedAction.Key, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.QualifiedName, i),
                            removedAction.Value.QualifiedName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.TypeFullName, i),
                            removedAction.Value.ActivityType.FullName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.AssemblyFullName, i),
                            removedAction.Value.ActivityType.Assembly.FullName,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.ParentQualifiedName, i),
                            removedAction.Value.ParentQualifiedName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.ActivityXoml, i),
                            removedAction.Value.ActivityXoml, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.Order, i),
                            removedAction.Value.Order, AdoDbType.Int32);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.WorkflowInstanceEventId, i),
                            null, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.QualifiedName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.TypeFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.AssemblyFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.ParentQualifiedName, i),
                            null, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.ActivityXoml, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertActivityRemovedActions,
                            TrackingParameterName.Order, i), null,
                            AdoDbType.Int32);
                    }
                }

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Insert a list of event annotations into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="type">
        /// The type of tracking record to associate the annotations with.
        /// </param>
        /// <param name="annotations">
        /// A list of <see cref="KeyValuePair{TKey,TValue}" /> objects
        /// containing the key and object that should be inserted into the 
        /// tracking store.
        /// </param>
        private void insertEventAnnotations(WorkflowInstanceSummary workflowInstanceSummary, TrackingRecordType type, IList<KeyValuePair<Int64, String>> annotations)
        {
            batch(annotations, EventAnnotationBatchSize,
                delegate(IList<KeyValuePair<Int64, String>> annotationBatch)
                {
                    InsertEventAnnotationBatch(workflowInstanceSummary, type, annotationBatch);
                });
        }

        /// <summary>
        /// Inserts a batch of event annotations.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="type">
        /// The type of tracking record to associate the annotations with.
        /// </param>
        /// <param name="annotations">
        /// An <see cref="IList{T}" /> containing a batch of annotations
        /// to be inserted into the tracking store.
        /// </param>
        protected virtual void InsertEventAnnotationBatch(WorkflowInstanceSummary workflowInstanceSummary, TrackingRecordType type, IList<KeyValuePair<Int64, String>> annotations)
        {
            if (annotations.Count == 0 || annotations.Count > EventAnnotationBatchSize)
                throw new ArgumentOutOfRangeException("annotations");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertEventAnnotations), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertEventAnnotations,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                String eventType = null;
                switch (type)
                {
                    case TrackingRecordType.Activity:
                        eventType = "A";
                        break;
                    case TrackingRecordType.User:
                        eventType = "U";
                        break;
                    case TrackingRecordType.Workflow:
                        eventType = "W";
                        break;
                }

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertEventAnnotations,
                    TrackingParameterName.EventType), eventType,
                    AdoDbType.String);

                for (Int32 i = 0 ; i < EventAnnotationBatchSize ; i++)
                {
                    if (i < annotations.Count)
                    {
                        KeyValuePair<Int64, String> annotation = annotations [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertEventAnnotations,
                            TrackingParameterName.EventId, i), annotation.Key,
                            AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertEventAnnotations,
                            TrackingParameterName.Annotation, i),
                            annotation.Value, AdoDbType.String);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertEventAnnotations,
                            TrackingParameterName.EventId, i), null,
                            AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertEventAnnotations,
                            TrackingParameterName.Annotation, i), null,
                            AdoDbType.String);
                    }
                }

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Insert a list of tracking data items into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="type">
        /// The type of tracking record to associate the tracking data items with.
        /// </param>
        /// <param name="trackingDataItems">
        /// An <see cref="IList{T}" /> containing a batch of tracking data items
        /// to be inserted into the tracking store.
        /// </param>
        private void insertTrackingDataItems(WorkflowInstanceSummary workflowInstanceSummary, TrackingRecordType type, IList<KeyValuePair<Int64, SerialisableTrackingDataItem>> trackingDataItems)
        {
            List<KeyValuePair<Int64, String>> annotations = new List<KeyValuePair<Int64, String>>();

            batch(trackingDataItems, TrackingDataItemBatchSize,
                delegate(IList<KeyValuePair<Int64, SerialisableTrackingDataItem>> trackingDataItemBatch)
                {
                    IDictionary<Int64, SerialisableTrackingDataItem> trackingDataItemsById =
                        InsertTrackingDataItemBatch(workflowInstanceSummary, type, trackingDataItemBatch);

                    foreach (Int64 id in trackingDataItemsById.Keys)
                    {
                        foreach (String annotation in trackingDataItemsById [id].Annotations)
                            annotations.Add(new KeyValuePair<Int64, String>(id, annotation));
                    }
                });

            insertTrackingDataItemAnnotations(workflowInstanceSummary, annotations);
        }


        /// <summary>
        /// Inserts a batch of tracking data items.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="type">
        /// The type of tracking record to associate the tracking data items with.
        /// </param>
        /// <param name="trackingDataItems">
        /// An <see cref="IList{T}" /> containing a batch of tracking data items
        /// to be inserted into the tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IDictionary{TKey,TValue}" /> containing the inserted
        /// records and indexed by their unique identifiers.
        /// </returns>
        protected virtual IDictionary<Int64, SerialisableTrackingDataItem> InsertTrackingDataItemBatch(WorkflowInstanceSummary workflowInstanceSummary, TrackingRecordType type, IList<KeyValuePair<Int64, SerialisableTrackingDataItem>> trackingDataItems)
        {
            if (trackingDataItems.Count == 0 || trackingDataItems.Count > TrackingDataItemBatchSize)
                throw new ArgumentOutOfRangeException("trackingDataItems");

            Dictionary<Int64, SerialisableTrackingDataItem> trackingDataItemsById = new Dictionary<Int64, SerialisableTrackingDataItem>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertTrackingDataItems), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertTrackingDataItems,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                String eventType = null;
                switch (type)
                {
                    case TrackingRecordType.Activity:
                        eventType = "A";
                        break;
                    case TrackingRecordType.User:
                        eventType = "U";
                        break;
                    case TrackingRecordType.Workflow:
                        eventType = "W";
                        break;
                }

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertTrackingDataItems,
                    TrackingParameterName.EventType), eventType,
                    AdoDbType.String);

                for (Int32 i = 0 ; i < TrackingDataItemBatchSize ; i++)
                {
                    if (i < trackingDataItems.Count)
                    {
                        KeyValuePair<Int64, SerialisableTrackingDataItem> trackingDataItem = trackingDataItems [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.EventId, i),
                            trackingDataItem.Key, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.FieldName, i),
                            trackingDataItem.Value.FieldName, AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.TypeFullName, i),
                            trackingDataItem.Value.Data.Type.FullName,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.AssemblyFullName, i),
                            trackingDataItem.Value.Data.Type.Assembly.FullName,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataString, i),
                            trackingDataItem.Value.Data.StringData,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataBlob, i),
                            trackingDataItem.Value.Data.SerialisedData,
                            AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataNonSerialisable, i),
                            trackingDataItem.Value.Data.NonSerialisable,
                            AdoDbType.Boolean);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.TrackingDataItemId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.EventId, i), null,
                            AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.FieldName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.TypeFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.AssemblyFullName, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataString, i), null,
                            AdoDbType.String);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataBlob, i), null,
                            AdoDbType.Binary);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.DataNonSerialisable, i),
                            null, AdoDbType.Boolean);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataItems,
                            TrackingParameterName.TrackingDataItemId, i),
                            AdoDbType.Int64, ParameterDirection.Output);
                    }
                }

                dbCommand.ExecuteNonQuery();

                for (Int32 i = 0 ; i < trackingDataItems.Count ; i++)
                {
                    trackingDataItemsById.Add(ValueReader.GetInt64(
                        dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.InsertTrackingDataItems,
                        TrackingParameterName.TrackingDataItemId, i)),
                        trackingDataItems [i].Value);
                }
            }

            return trackingDataItemsById;
        }

        /// <summary>
        /// Insert a list of tracking data item annotations into the tracking store.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="annotations">
        /// A list of <see cref="KeyValuePair{TKey,TValue}" /> objects
        /// containing the parent identifier and the child item to persist.
        /// </param>
        private void insertTrackingDataItemAnnotations(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, String>> annotations)
        {
            batch(annotations, TrackingDataItemAnnotationBatchSize,
                delegate(IList<KeyValuePair<Int64, String>> annotationBatch)
                {
                    InsertTrackingDataItemAnnotationBatch(workflowInstanceSummary, annotationBatch);
                });
        }

        /// <summary>
        /// Inserts a batch of tracking data item annotations.
        /// </summary>
        /// <param name="workflowInstanceSummary">
        /// A <see cref="WorkflowInstanceSummary" /> representing a workflow instance
        /// along with all its type information.
        /// </param>
        /// <param name="annotations">
        /// An <see cref="IList{T}" /> containing a batch of annotations
        /// to be inserted into the tracking store.
        /// </param>
        protected virtual void InsertTrackingDataItemAnnotationBatch(WorkflowInstanceSummary workflowInstanceSummary, IList<KeyValuePair<Int64, String>> annotations)
        {
            if (annotations.Count == 0 || annotations.Count > TrackingDataItemAnnotationBatchSize)
                throw new ArgumentOutOfRangeException("annotations");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.InsertTrackingDataAnnotations), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.InsertTrackingDataAnnotations,
                    TrackingParameterName.WorkflowInstanceId),
                    workflowInstanceSummary.InternalId, AdoDbType.Int64);

                for (Int32 i = 0 ; i < EventAnnotationBatchSize ; i++)
                {
                    if (i < annotations.Count)
                    {
                        KeyValuePair<Int64, String> annotation = annotations [i];

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataAnnotations,
                            TrackingParameterName.TrackingDataItemId, i),
                            annotation.Key, AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataAnnotations,
                            TrackingParameterName.Annotation, i),
                            annotation.Value, AdoDbType.String);
                    }
                    else
                    {
                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataAnnotations,
                            TrackingParameterName.TrackingDataItemId, i), null,
                            AdoDbType.Int64);

                        AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                            TrackingCommandName.InsertTrackingDataAnnotations,
                            TrackingParameterName.Annotation, i), null,
                            AdoDbType.String);
                    }
                }

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delegate used to handle batches of items for persistence.
        /// </summary>
        private delegate void BatchHandler<T>(IList<T> objectBatch);

        /// <summary>
        /// Helper method to aid in batching up records ready for persistence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to batch.
        /// </typeparam>
        /// <param name="objects">
        /// An <see cref="IList{T}" /> containing the full list of objects to batch.
        /// </param>
        /// <param name="batchSize">
        /// The size to make each batch.
        /// </param>
        /// <param name="batchHandler">
        /// A delegate of type <see cref="BatchHandler{T}" /> used for dealing with
        /// each batch of objects.
        /// </param>
        private static void batch<T>(IList<T> objects, Int32 batchSize, BatchHandler<T> batchHandler)
        {
            List<T> objectBatch = new List<T>();

            Int32 listIndex = 0;
            while (listIndex < objects.Count)
            {
                objectBatch.Add(objects [listIndex++]);

                if (objectBatch.Count == batchSize || listIndex == objects.Count)
                {
                    batchHandler(objectBatch);

                    objectBatch.Clear();
                }
            }
        }

        /// <summary>
        /// Helper method used to construct a unique key for an 
        /// activity instance identifier.
        /// </summary>
        /// <param name="activityIdentifier"></param>
        /// <returns></returns>
        private static String getActivityIdentifierKey(String activityIdentifier)
        {
            return String.Format(CultureInfo.InvariantCulture,
                "ActivityInstanceIdentifier_{0}", activityIdentifier);
        }

        /// <summary>
        /// Given a unique identifier for an activity, find the internal identifier
        /// that uniquely identifies the activity instance to the database.
        /// </summary>
        /// <param name="activityIdentifier">
        /// Unique identifier of the activity.
        /// </param>
        /// <returns>
        /// An <see cref="Int64" /> value representing the internal identifier
        /// of the specified activity identifier. If no identifier is found
        /// then <c>null</c> is returned.
        /// </returns>
        protected Int64? FindActivityInstanceInternalId(String activityIdentifier)
        {
            if (_stateProvider != null)
                return _stateProvider.Get<Int64?>(getActivityIdentifierKey(activityIdentifier));

            return null;
        }

        /// <summary>
        /// Removes the association between a unique identifier for an activity 
        /// and its internal identifier.
        /// </summary>
        /// <param name="activityIdentifier">
        /// Unique identifier of the activity.
        /// </param>
        protected void RemoveActivityInstanceInternalId(String activityIdentifier)
        {
            if (_stateProvider != null)
            {
                String activityIdentifierKey = getActivityIdentifierKey(activityIdentifier);
                if (_stateProvider.Contains(activityIdentifierKey))
                    _stateProvider.Remove(activityIdentifierKey);
            }
        }

        /// <summary>
        /// Associates the unique identifier for an activity with its internal identifier.
        /// </summary>
        /// <param name="activityIdentifier">
        /// Unique identifier of the activity.
        /// </param>
        /// <param name="internalIdentifier">
        /// Internal identifier to associate.
        /// </param>
        protected void UpdateActivityInstanceInternalId(String activityIdentifier, Int64 internalIdentifier)
        {
            if (_stateProvider != null)
            {
                String activityIdentifierKey = getActivityIdentifierKey(activityIdentifier);
                if (_stateProvider.Contains(activityIdentifierKey))
                    _stateProvider.Update(activityIdentifierKey, internalIdentifier);
                else
                    _stateProvider.Add(activityIdentifierKey, internalIdentifier);
            }
        }

        /// <summary>
        /// Given an activity tracking record, create an identifier that uniquely
        /// identifies an activity instance.
        /// </summary>
        /// <param name="activityTrackingRecord">
        /// A <see cref="SerialisableActivityTrackingRecord" /> to build an identifier for.
        /// </param>
        /// <returns>
        /// <see cref="String" /> representing a unique identifier for the activity
        /// instance that the activity tracking record was generated for.
        /// </returns>
        protected String BuildActivityIdentifier(SerialisableActivityTrackingRecord activityTrackingRecord)
        {
            return buildActivityIdentifier(activityTrackingRecord.QualifiedName,
                activityTrackingRecord.ContextGuid,
                activityTrackingRecord.ParentContextGuid);
        }

        /// <summary>
        /// Given a user tracking record, create an identifier that uniquely
        /// identifies an activity instance.
        /// </summary>
        /// <param name="userTrackingRecord">
        /// A <see cref="SerialisableUserTrackingRecord" /> to build an identifier for.
        /// </param>
        /// <returns>
        /// <see cref="String" /> representing a unique identifier for the activity
        /// instance that the user tracking record was generated for.
        /// </returns>
        protected String BuildActivityIdentifier(SerialisableUserTrackingRecord userTrackingRecord)
        {
            return buildActivityIdentifier(userTrackingRecord.QualifiedName,
                userTrackingRecord.ContextGuid,
                userTrackingRecord.ParentContextGuid);
        }

        private static String buildActivityIdentifier(String qualifiedName, Guid contextGuid, Guid parentContextGuid)
        {
            using (MD5 cryptoProvider = new MD5CryptoServiceProvider())
            {
                byte [] bytes = new UnicodeEncoding().GetBytes(qualifiedName);
                String hashedQualifiedName = new Guid(cryptoProvider.ComputeHash(bytes)).ToString().Replace('-', '_');

                return String.Format(CultureInfo.InvariantCulture,
                    "{0}_{1}_{2}", hashedQualifiedName,
                    contextGuid.ToString().Replace('-', '_'),
                    parentContextGuid.ToString().Replace('-', '_'));
            }
        }
    }
}
