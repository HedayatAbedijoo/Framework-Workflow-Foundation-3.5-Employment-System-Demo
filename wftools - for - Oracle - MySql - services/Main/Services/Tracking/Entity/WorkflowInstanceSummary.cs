using System;
using System.Workflow.ComponentModel;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents a summary of workflow instance information that is used by 
    /// the <see cref="GenericTrackingService" /> to persist 
    /// tracking information somewhere useful.
    /// </summary>
    public class WorkflowInstanceSummary
    {
        /// <summary>
        /// Constructs a new <see cref="WorkflowInstanceSummary" />
        /// </summary>
        /// <param name="instanceId">
        /// Unique identifier of this workflow instance.
        /// </param>
        /// <param name="isXomlWorkflow">
        /// Indicates whether the workflow instance was constructed using XOML only.
        /// </param>
        /// <param name="xomlDocument">
        /// XOML document of this workflow instance.
        /// </param>
        /// <param name="contextGuid">
        /// Identifier of the context in which this workflow instance was called.</param>
        /// <param name="callerInstanceId">
        /// Identifier of the workflow that called this workflow.
        /// </param>
        /// <param name="callerContextGuid">
        /// Identifier of the context in which the calling workflow was called.
        /// </param>
        /// <param name="callerParentContextGuid">
        /// Identifier of the parent context in which the calling workflow was called.
        /// </param>
        /// <param name="callPath">
        /// Call path of the workflow.
        /// </param>
        public WorkflowInstanceSummary(Guid instanceId, Boolean isXomlWorkflow, 
            String xomlDocument, Guid contextGuid, Guid? callerInstanceId, 
            Guid? callerContextGuid, Guid? callerParentContextGuid, String callPath)
        {
            _instanceId = instanceId;
            _isXomlWorkflow = isXomlWorkflow;
            _xomlDocument = xomlDocument;
            _contextGuid = contextGuid;
            _callerInstanceId = callerInstanceId;
            _callerContextGuid = callerContextGuid;
            _callerParentContextGuid = callerParentContextGuid;
            _callPath = callPath;
        }

        private Object _internalId;
        /// <summary>
        /// Gets/sets a unique identifier assigned to this object by
        /// the underlying tracking store.
        /// </summary>
        public Object InternalId
        {
            get { return _internalId; }
            set { _internalId = value; }
        }

        private Guid _instanceId;
        /// <summary>
        /// Gets/sets the unique identifier of this workflow instance.
        /// </summary>
        public Guid InstanceId
        {
            get { return _instanceId; }
            set { _instanceId = value; }
        }

        private Guid _contextGuid;
        /// <summary>
        /// Gets/sets the context in which this workflow instance was called.
        /// </summary>
        public Guid ContextGuid
        {
            get { return _contextGuid; }
            set { _contextGuid = value; }
        }

        private Guid? _callerInstanceId;
        /// <summary>
        /// Gets/sets the unique identifier of the workflow instance that called
        /// this workflow instance.
        /// </summary>
        public Guid? CallerInstanceId
        {
            get { return _callerInstanceId; }
            set { _callerInstanceId = value; }
        }

        private String _callPath;
        /// <summary>
        /// Gets/sets a concatenated list of activity qualified names with 
        /// which this workflow instance has called through.
        /// </summary>
	    public String CallPath
	    {
		    get { return _callPath;}
		    set { _callPath = value;}
	    }

        private Guid? _callerContextGuid;
        /// <summary>
        /// Gets/sets a <see cref="Guid" /> representing the context 
        /// identifier of the workflow instance that called this workflow 
        /// instance.
        /// </summary>
        public Guid? CallerContextGuid
        {
            get { return _callerContextGuid; }
            set { _callerContextGuid = value; }
        }

        private Guid? _callerParentContextGuid;
        /// <summary>
        /// Gets/sets a <see cref="Guid" /> representing the context 
        /// identifier of the workflow instance that called the
        /// caller of this workflow instance.
        /// </summary>
        public Guid? CallerParentContextGuid
        {
            get { return _callerParentContextGuid; }
            set { _callerParentContextGuid = value; }
        }

        private WorkflowTypeSummary _workflowType;
        /// <summary>
        /// Gets/sets a <see cref="WorkflowTypeSummary" /> for this workflow 
        /// instance.
        /// </summary>
        public WorkflowTypeSummary WorkflowType
        {
            get { return _workflowType; }
            set { _workflowType = value; }
        }

        private ActivitySummary _rootActivity;
        /// <summary>
        /// Gets/sets an <see cref="ActivitySummary" /> for the root 
        /// <see cref="Activity" /> of this workflow instance.
        /// </summary>
        public ActivitySummary RootActivity
        {
            get { return _rootActivity; }
            set { _rootActivity = value; }
        }

        private Boolean _isXomlWorkflow;
        /// <summary>
        /// Gets/sets a value that indicates whether the workflow instance was 
        /// constructed using XOML only. If <c>true</c>, <see cref="WorkflowType" />
        /// will not accurately represent the 'real' type of the instance.
        /// </summary>
        public Boolean IsXomlWorkflow
        {
            get { return _isXomlWorkflow; }
            set { _isXomlWorkflow = value; }
        }

        private String _xomlDocument;
        /// <summary>
        /// Gets/sets the XOML document of this workflow instance.
        /// </summary>
        public String XomlDocument
        {
            get { return _xomlDocument; }
            set { _xomlDocument = value; }
        }
    }
}