using System;
using System.Collections.Generic;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Tracking;
using WFTools.Utilities;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Contains the criteria necessary for filtering the set of <see cref="TrackingWorkflowInstance"/> 
    /// objects returned by a call to <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery)" /> 
    /// or <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery,int,int)" />.
    /// </summary>
    public class TrackingQuery
    {
        private DateRange statusDateRange;
        /// <summary>
        /// Gets or sets a <see cref="DateRange" /> that specifies the range of dates that, 
        /// together with <see cref="Statuses" />, is used to constrain the 
        /// set of <see cref="TrackingWorkflowInstance" /> objects returned by a call to 
        /// <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery)" /> or
        /// <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery,int,int)" />.
        /// </summary>
        public DateRange StatusDateRange
        {
            get { return statusDateRange; }
            set { statusDateRange = value; }
        }

        private readonly List<WorkflowStatus> statuses = new List<WorkflowStatus>();
        /// <summary>
        /// Gets a list of <see cref="WorkflowStatus" /> values that can be used to constrain
        /// the set of <see cref="TrackingWorkflowInstance" /> objects returned by a call to 
        /// <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery)" /> or
        /// <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery,int,int)" />.
        /// </summary>
        public IList<WorkflowStatus> Statuses
        {
            get { return statuses; }
        }

        private readonly List<Type> types = new List<Type>();
        /// <summary>
        /// Gets a list of <see cref="Type" /> objects representing the workflow instance type 
        /// that can be used to constrain the set of <see cref="TrackingWorkflowInstance" /> 
        /// objects returned by a call to <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery)" /> 
        /// or <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery,int,int)" />.
        /// </summary>
        public IList<Type> Types
        {
            get { return types; }
        }

        private readonly List<TrackingDataItem> trackingDataItems = new List<TrackingDataItem>();
        /// <summary>
        /// Gets a list of <see cref="TrackingDataItem" /> objects which are used to 
        /// constrain the set of <see cref="TrackingWorkflowInstance" /> objects returned 
        /// by a call to <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery)" /> 
        /// or <see cref="GenericTrackingQueryManager.GetWorkflows(TrackingQuery,int,int)" />.
        /// </summary>
        public IList<TrackingDataItem> TrackingDataItems
        {
            get { return trackingDataItems; }
        }
    }
}