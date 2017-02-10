using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Tracking;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Provides access to tracking data associated with a particular workflow 
    /// instance in the tracking store.
    /// </summary>
    public class TrackingWorkflowInstance : INotifyPropertyChanged
    {
        /// <summary>
        /// Construct a new <see cref="TrackingWorkflowInstance" /> with information
        /// to enable it to populate itself with data.
        /// </summary>
        /// <param name="definition">
        /// Root <see cref="Activity" /> for the workflow instance.
        /// </param>
        /// <param name="id">
        /// <see cref="Guid" /> that represents the unique identifier of 
        /// the workflow instance.
        /// </param>
        /// <param name="invokingWorkflowId">
        /// </param>
        /// <param name="initialised">
        /// <see cref="DateTime" /> indicating when the first 
        /// <see cref="TrackingChannel" /> was requested for the workflow
        /// instance.
        /// </param>
        /// <param name="internalId">
        /// <see cref="Object" /> that represents the internal
        /// identifier of the workflow instance within the tracking store.
        /// </param>
        /// <param name="status">
        /// </param>
        /// <param name="type">
        /// <see cref="WorkflowStatus" /> of the workflow instance.
        /// </param>
        public TrackingWorkflowInstance(Activity definition, Guid id, 
            DateTime initialised, Object internalId, Guid? invokingWorkflowId, 
            WorkflowStatus status, Type type)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            if (internalId == null)
                throw new ArgumentNullException("internalId");

            if (type == null)
                throw new ArgumentNullException("type");

            _definition = definition;
            _id = id;
            _initialised = initialised;
            _internalId = internalId;
            _invokingWorkflowId = invokingWorkflowId;
            _status = status;
            _type = type;
        }

        private GenericTrackingQueryManager _queryManager;
        /// <summary>
        /// Sets the <see cref="GenericTrackingQueryManager" /> that can be used
        /// to query the tracking store.
        /// </summary>
        internal GenericTrackingQueryManager QueryManager
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _queryManager = value;
            }
        }

        /// <summary>
        /// Indicates the last time that the contents of this object were 
        /// </summary>
        private DateTime lastRefreshed = DateTime.MinValue;

        /// <summary>
        /// Updates this <see cref="TrackingWorkflowInstance" /> with the 
        /// latest data from the tracking store.
        /// </summary>
        public void Refresh()
        {
            if (_queryManager == null)
                throw new InvalidOperationException("Cannot refresh without a QueryManager.");

            // utilise the internal identifier and a timestamp of when a
            // refresh of the TrackingWorkflowInstance last occurred to
            // add the most recent entries to contained collections and 
            // property values. If anything changes we raise the PropertyChanged 
            // event in order to update any bound UI with latest data
            ITrackingQueryResourceAccessor resourceAccessor = null;
            try
            {
                // stop the timer while we refresh data
                if (refreshTimer != null)
                    refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);

                // retrieve an ITrackingQueryResourceAccessor to query the data store
                resourceAccessor = _queryManager.CreateAccessor();

                // query for latest workflow instance details
                TrackingWorkflowInstance newTrackingWorkflowInstance = resourceAccessor.GetWorkflow(_id);
                if (newTrackingWorkflowInstance != null)
                {
                    if (_definition != newTrackingWorkflowInstance._definition)
                    {
                        _definition = newTrackingWorkflowInstance._definition;
                        raisePropertyChanged("Definition");
                    }

                    if (_status != newTrackingWorkflowInstance._status)
                    {
                        _status = newTrackingWorkflowInstance._status;
                        raisePropertyChanged("Status");
                    }
                }

                // query latest invoking workflow information
                if (_invokingWorkflowId.HasValue)
                {
                    TrackingWorkflowInstance newInvokingWorkflow = resourceAccessor.GetWorkflow(_invokingWorkflowId.Value);
                    if (newInvokingWorkflow != null)
                    {
                        invokingWorkflow = newInvokingWorkflow;
                        raisePropertyChanged("InvokingWorkflow");
                    }
                }

                // query for latest invoked workflow information
                if (invokedWorkflows == null)
                    invokedWorkflows = new List<TrackingWorkflowInstance>();

                IList<TrackingWorkflowInstance> newInvokedWorkflows = resourceAccessor.GetInvokedWorkflows(_id);
                if (newInvokedWorkflows.Count > 0)
                {
                    invokedWorkflows.AddRange(newInvokedWorkflows);
                    raisePropertyChanged("InvokedWorkflows");
                }

                // query for latest activity events
                if (activityEvents == null)
                    activityEvents = new List<ActivityTrackingRecord>();

                IList<ActivityTrackingRecord> newActivityEvents = resourceAccessor.GetActivityEvents(_internalId, lastRefreshed);
                if (newActivityEvents.Count > 0)
                {
                    activityEvents.AddRange(newActivityEvents);
                    raisePropertyChanged("ActivityEvents");
                }

                // query for latest user events
                if (userEvents == null)
                    userEvents = new List<UserTrackingRecord>();

                IList<UserTrackingRecord> newUserEvents = resourceAccessor.GetUserEvents(_internalId, lastRefreshed);
                if (newUserEvents.Count > 0)
                {
                    userEvents.AddRange(newUserEvents);
                    raisePropertyChanged("UserEvents");
                }

                // query for latest workflow events
                if (workflowEvents == null)
                    workflowEvents = new List<WorkflowTrackingRecord>();

                IList<WorkflowTrackingRecord> newWorkflowEvents = resourceAccessor.GetWorkflowEvents(_internalId, lastRefreshed);
                if (newWorkflowEvents.Count > 0)
                {
                    workflowEvents.AddRange(newWorkflowEvents);
                    raisePropertyChanged("WorkflowEvents");
                }

                lastRefreshed = DateTime.UtcNow;
            }
            finally
            {
                // dispose of the resource accessor
                if (resourceAccessor != null)
                    resourceAccessor.Dispose();

                // restart the timer
                if (refreshTimer != null)
                    refreshTimer.Change(refreshInterval, refreshInterval);
            }
        }

        private void raisePropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// <see cref="Timer" /> used to auto-refresh data store dependent
        /// properties.
        /// </summary>
        private Timer refreshTimer;

        private TimeSpan refreshInterval;
        /// <summary>
        /// Gets or sets a <see cref="TimeSpan"/> indicating the interval
        /// at which <see cref="Refresh" /> is called automatically.
        /// </summary>
        /// <remarks>
        /// By default this property is set to <see cref="TimeSpan.Zero" /> 
        /// indicating that <see cref="Refresh"/> is never called automatically.
        /// </remarks>
        public TimeSpan RefreshInterval
        {
            get { return refreshInterval; }
            set
            {
                if (value != refreshInterval)
                {
                    
                    if (refreshTimer != null)
                    {
                        // disable any active timers
                        refreshTimer.Dispose();
                        refreshTimer = null;
                    }

                    if (value != TimeSpan.Zero)
                    {
                        // configure a new timer to call Refresh
                        refreshTimer = new Timer(delegate { Refresh(); },
                            null, value, value);
                    }

                    refreshInterval = value;
                }
            }
        }

        private List<ActivityTrackingRecord> activityEvents;
        /// <summary>
        /// Gets the list of <see cref="ActivityTrackingRecord" /> objects associated
        /// with the workflow instance.
        /// </summary>
        public IList<ActivityTrackingRecord> ActivityEvents
        {
            get
            {
                if (activityEvents == null)
                    Refresh();

                return activityEvents.AsReadOnly();
            }
        }

        private Activity _definition;
        /// <summary>
        /// Gets the root <see cref="Activity" /> for the workflow instance.
        /// </summary>
        public Activity Definition
        {
            get { return _definition; }
        }

        private readonly Guid _id;
        /// <summary>
        /// Gets a <see cref="Guid" /> that represents the unique
        /// identifier of the workflow instance.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        private readonly DateTime _initialised;
        /// <summary>
        /// Gets a <see cref="DateTime" /> indicating when the first 
        /// <see cref="TrackingChannel" /> was requested for the workflow
        /// instance.
        /// </summary>
        public DateTime Initialised
        {
            get { return _initialised; }
        }

        private readonly Object _internalId;
        /// <summary>
        /// Gets an <see cref="Object" /> that represents the internal
        /// identifier of the workflow instance within the tracking store.
        /// </summary>
        public Object InternalId
        {
            get { return _internalId; }
        }

        private List<TrackingWorkflowInstance> invokedWorkflows;
        /// <summary>
        /// Gets a list of <see cref="TrackingWorkflowInstance" /> objects representing
        /// the tracking data for the workflows invoked by the workflow instance.
        /// </summary>
        public IList<TrackingWorkflowInstance> InvokedWorkflows
        {
            get
            {
                if (invokedWorkflows == null)
                    Refresh();

                return invokedWorkflows.AsReadOnly();
            }
        }

        /// <summary>
        /// <see cref="Guid" /> representing the unique identifier
        /// of the workflow that invoked the workflow instance.
        /// </summary>
        private Guid? _invokingWorkflowId;

        private TrackingWorkflowInstance invokingWorkflow;
        /// <summary>
        /// Gets a <see cref="TrackingWorkflowInstance" /> that represents
        /// the tracking data of the workflow that invoked the workflow instance.
        /// </summary>
        public TrackingWorkflowInstance InvokingWorkflow
        {
            get
            {
                if (_invokingWorkflowId.HasValue && invokingWorkflow == null)
                    Refresh();

                return invokingWorkflow;
            }
        }

        private WorkflowStatus _status;
        /// <summary>
        /// Gets the current <see cref="WorkflowStatus" /> of the workflow instance.
        /// </summary>
        public WorkflowStatus Status
        {
            get { return _status; }
        }

        private readonly Type _type;
        /// <summary>
        /// Gets the <see cref="Type" /> of the workflow instance.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        private List<UserTrackingRecord> userEvents;
        /// <summary>
        /// Gets the list of <see cref="UserTrackingRecord" /> objects associated
        /// with the workflow instance.
        /// </summary>
        public IList<UserTrackingRecord> UserEvents
        {
            get
            {
                if (userEvents == null)
                    Refresh();

                return userEvents.AsReadOnly();
            }
        }

        private List<WorkflowTrackingRecord> workflowEvents;
        /// <summary>
        /// Gets the list of <see cref="WorkflowTrackingRecord" /> objects associated
        /// with the workflow instance.
        /// </summary>
        public IList<WorkflowTrackingRecord> WorkflowEvents
        {
            get
            {
                if (workflowEvents == null)
                    Refresh();

                return workflowEvents.AsReadOnly();
            }
        }

        ///<summary>
        ///Occurs when a property value changes.
        ///</summary>
        ///
        public event PropertyChangedEventHandler PropertyChanged;
    }
}