using System;
using System.Diagnostics.CodeAnalysis;
using WFTools.Services.Tracking.Ado;

namespace WFTools.Services.MySql.Tracking
{
    /// <summary>
    /// MySql-specific implementation of <see cref="ITrackingNameResolver" />.
    /// </summary>
    public class MySqlTrackingNameResolver : ITrackingNameResolver
    {
        /// <summary>
        /// Resolve <see cref="TrackingCommandName" /> to their database-specific command text.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="TrackingCommandName" /> indicating which command needs to be resolved.
        /// </param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Uses a large switch statement.")]
        public string ResolveCommandName(TrackingCommandName commandName)
        {
            switch (commandName)
            {
                case TrackingCommandName.DeleteInstanceTrackingProfile:
                    return "DeleteInstanceTrackingProfile";
                case TrackingCommandName.DeleteTrackingProfile:
                    return "DeleteTrackingProfile";
                case TrackingCommandName.GetCurrentDefaultTrackingProfile:
                    return "GetCurrentDefaultTrackingProfile";
                case TrackingCommandName.GetDefaultTrackingProfile:
                    return "GetDefaultTrackingProfile";
                case TrackingCommandName.GetInstanceTrackingProfile:
                    return "GetInstanceTrackingProfile";
                case TrackingCommandName.GetTrackingProfile:
                    return "GetTrackingProfile";
                case TrackingCommandName.GetTrackingProfileChanges:
                    return "GetTrackingProfileChanges";
                case TrackingCommandName.InsertActivities:
                    return "InsertActivities";
                case TrackingCommandName.InsertActivityAddedActions:
                    return "InsertActivityAddedActions";
                case TrackingCommandName.InsertActivityRemovedActions:
                    return "InsertActivityRemovedActions";
                case TrackingCommandName.InsertActivityTrackingRecords:
                    return "InsertActivityTrackingRecords";
                case TrackingCommandName.InsertEventAnnotations:
                    return "InsertEventAnnotations";
                case TrackingCommandName.InsertTrackingDataItems:
                    return "InsertTrackingDataItems";
                case TrackingCommandName.InsertTrackingDataAnnotations:
                    return "InsertTrackingDataAnnotations";
                case TrackingCommandName.InsertUserTrackingRecords:
                    return "InsertUserTrackingRecords";
                case TrackingCommandName.InsertWorkflow:
                    return "InsertWorkflow";
                case TrackingCommandName.InsertWorkflowInstance:
                    return "InsertWorkflowInstance";
                case TrackingCommandName.InsertWorkflowTrackingRecords:
                    return "InsertWorkflowTrackingRecords";
                case TrackingCommandName.UpdateDefaultTrackingProfile:
                    return "UpdateDefaultTrackingProfile";
                case TrackingCommandName.UpdateInstanceTrackingProfile:
                    return "UpdateInstanceTrackingProfile";
                case TrackingCommandName.UpdateTrackingProfile:
                    return "UpdateTrackingProfile";
            }

            throw new NotSupportedException(RM.Get_Error_CommandNamesNotSupported());
        }

        /// <summary>
        /// Resolve <see cref="TrackingParameterName" /> to their database-specific parameter name.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="TrackingCommandName" /> indicating which command the parameter
        /// name needs to be resolved for.
        /// </param>
        /// <param name="parameterName">
        /// A <see cref="TrackingParameterName" /> indicating which parameter needs to be resolved.
        /// </param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Uses a large switch statement.")]
        public string ResolveParameterName(TrackingCommandName commandName, TrackingParameterName parameterName)
        {
            switch (parameterName)
            {
                case TrackingParameterName.ActivityXml:
                    return "p_ACTIVITY_XML";

                case TrackingParameterName.ActivityXoml:

                    if (commandName == TrackingCommandName.InsertActivityAddedActions)
                        return "p_ADDED_ACTIVITY_ACTION";
                    else if (commandName == TrackingCommandName.InsertActivityRemovedActions)
                        return "p_REMOVED_ACTIVITY_ACTION";

                    break;

                case TrackingParameterName.ActivityInstanceId:
                    return "p_ACTIVITY_INSTANCE_ID";

                case TrackingParameterName.ActivityStatusId:
                    return "p_ACTIVITY_STATUS_ID";

                case TrackingParameterName.ActivityStatusEventId:
                    return "p_ACTIVITY_STATUS_EVENT_ID";

                case TrackingParameterName.Annotation:
                    return "p_ANNOTATION";

                case TrackingParameterName.AssemblyFullName:
                    return "p_ASSEMBLY_FULL_NAME";

                case TrackingParameterName.CallerContextGuid:
                    return "p_CALLER_CONTEXT_GUID";

                case TrackingParameterName.CallerInstanceId:
                    return "p_CALLER_INSTANCE_ID";

                case TrackingParameterName.CallerParentContextGuid:
                    return "p_CALLER_PARENT_CONTEXT_GUID";

                case TrackingParameterName.CallPath:
                    return "p_CALL_PATH";

                case TrackingParameterName.ContextGuid:
                    return "p_CONTEXT_GUID";

                case TrackingParameterName.CreateDefault:
                    return "p_CREATE_DEFAULT";

                case TrackingParameterName.DataBlob :
                    return "p_DATA_BLOB";

                case TrackingParameterName.DataString :
                    return "p_DATA_STR";

                case TrackingParameterName.DataNonSerialisable :
                    return "p_DATA_NON_SERIALISABLE";

                case TrackingParameterName.EventArgs:
                    return "p_EVENT_ARG";

                case TrackingParameterName.EventArgsAssemblyFullName:
                    return "p_EVENT_ARG_ASSEMBLY_NAME";

                case TrackingParameterName.EventArgsTypeFullName:
                    return "p_EVENT_ARG_TYPE_NAME";

                case TrackingParameterName.EventDateTime:
                    return "p_EVENT_DATE_TIME";

                case TrackingParameterName.EventId:
                    return "p_EVENT_ID";

                case TrackingParameterName.EventOrder:
                    return "p_EVENT_ORDER";

                case TrackingParameterName.EventType:
                    return "p_EVENT_TYPE";

                case TrackingParameterName.Exists:
                    return "p_EXISTS";

                case TrackingParameterName.FieldName:
                    return "p_FIELD_NAME";

                case TrackingParameterName.InitialisedDateTime:
                    return "p_INITIALISED_DATE_TIME";

                case TrackingParameterName.InstanceId:
                    return "p_INSTANCE_ID";

                case TrackingParameterName.IsInstanceType:
                    return "p_IS_INSTANCE_TYPE";

                case TrackingParameterName.LastCheck:
                    return "p_LAST_CHECK";

                case TrackingParameterName.NextCheck:
                    return "p_NEXT_CHECK";

                case TrackingParameterName.Order:
                    return "p_ORDER";

                case TrackingParameterName.ParentContextGuid:
                    return "p_PARENT_CONTEXT_GUID";

                case TrackingParameterName.ParentQualifiedName:
                    return "p_PARENT_QUALIFIED_NAME";

                case TrackingParameterName.QualifiedName:
                    return "p_QUALIFIED_NAME";

                case TrackingParameterName.TrackingDataItemId :
                    return "p_TRACKING_DATA_ITEM_ID";

                case TrackingParameterName.TrackingProfile:
                    return "p_TRACKING_PROFILE_XML";

                case TrackingParameterName.TypeFullName:
                    return "p_TYPE_FULL_NAME";

                case TrackingParameterName.Version:
                    return "p_VERSION";

                case TrackingParameterName.UserDataAssemblyFullName:
                    return "p_USER_DATA_ASSEMBLY_NAME";

                case TrackingParameterName.UserDataBlob:
                    return "p_USER_DATA_BLOB";

                case TrackingParameterName.UserDataKey:
                    return "p_USER_DATA_KEY";

                case TrackingParameterName.UserDataNonSerialisable:
                    return "p_USER_DATA_NON_SERIALISABLE";

                case TrackingParameterName.UserDataString:
                    return "p_USER_DATA_STR";

                case TrackingParameterName.UserDataTypeFullName:
                    return "p_USER_DATA_TYPE_NAME";

                case TrackingParameterName.UserEventId:
                    return "p_USER_EVENT_ID";

                case TrackingParameterName.WorkflowDefinition:
                    return "p_WORKFLOW_DEFINITION";

                case TrackingParameterName.WorkflowTypeId:
                    return "p_WORKFLOW_TYPE_ID";

                case TrackingParameterName.WorkflowInstanceEventId:
                    return "p_WORKFLOW_INSTANCE_EVENT_ID";

                case TrackingParameterName.WorkflowInstanceId:
                    return "p_WORKFLOW_INSTANCE_ID";

                case TrackingParameterName.WorkflowInstanceStatusId:
                    return "p_WORKFLOW_INSTANCE_STATUS";
            }

            throw new NotSupportedException(RM.Get_Error_CommandNameParameterNameNotSupported());
        }

        /// <summary>
        /// Resolve a <see cref="TrackingParameterName" /> to its database-specific parameter name
        /// including any additional batching information.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="TrackingCommandName" /> indicating which command the parameter
        /// name needs to be resolved for.
        /// </param>
        /// <param name="parameterName">
        /// A <see cref="TrackingParameterName" /> indicating which parameter needs to be resolved.
        /// </param>
        /// <param name="parameterBatch">
        /// <see cref="Int32" /> representing which parameter batch we're dealing with.
        /// </param>
        /// <returns>
        /// </returns>
        public string ResolveParameterName(TrackingCommandName commandName, TrackingParameterName parameterName, Int32 parameterBatch)
        {
            return String.Format("{0}_{1}", ResolveParameterName(
                commandName, parameterName), parameterBatch + 1);
        }
    }
}