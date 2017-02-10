using System;
using System.Diagnostics.CodeAnalysis;
using System.Workflow.Activities;
using System.Workflow.Runtime;

namespace WFTools.Services.Persistence
{
    /// <summary>
    /// Represents a pending workflow task that is committed in a single transaction.
    /// </summary>
    public class PendingWorkItem
    {
        private Boolean _isBlocked;
        /// <summary>
        /// Gets/sets a value indicating whether the workflow instance is blocked.
        /// </summary>
        public Boolean IsBlocked
        {
            get { return _isBlocked; }
            set { _isBlocked = value; }
        }

        private String _info;
        /// <summary>
        /// Gets/sets additional information about the workflow instance.
        /// </summary>
        public String Info
        {
            get { return _info; }
            set { _info = value; }
        }

        private Guid _instanceId;
        /// <summary>
        /// Gets/sets the workflow instance's unique identifier.
        /// </summary>
        public Guid InstanceId
        {
            get { return _instanceId; }
            set { _instanceId = value; }
        }

        private DateTime? _nextTimer;
        /// <summary>
        /// Gets/sets a <see cref="DateTime" /> indicating when the next
        /// timer based <see cref="IEventActivity" /> is due to fire for
        /// the workflow instance.
        /// </summary>
        public DateTime? NextTimer
        {
            get { return _nextTimer; }
            set { _nextTimer = value; }
        }

        private Byte [] _serialisedActivity;
        /// <summary>
        /// Gets/sets a serialised representation of the workflow instance.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "byte [] contains serialised data, cannot be represented as a collection")]
        public Byte [] SerialisedActivity
        {
            get { return _serialisedActivity; }
            set { _serialisedActivity = value; }
        }

        private Guid _stateId;
        /// <summary>
        /// Gets/sets the identifier of the state that the workflow instance is currently in.
        /// </summary>
        public Guid StateId
        {
            get { return _stateId; }
            set { _stateId = value; }
        }

        private WorkflowStatus _status;
        /// <summary>
        /// Gets/sets the actual status of the workflow instance.
        /// </summary>
        public WorkflowStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private ItemType _type;
        /// <summary>
        /// Gets/sets a value that indicates what to do with this pending work item.
        /// </summary>
        public ItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Boolean _unlock;
        /// <summary>
        /// Gets/sets a value indicating whether to unlock a workflow instance.
        /// </summary>
        public Boolean Unlock
        {
            get { return _unlock; }
            set { _unlock = value; }
        }

        /// <summary>
        /// Gets/sets the available types of operation used by PendingWorkItem.
        /// </summary>
        public enum ItemType
        {
            /// <summary>
            /// Pending work item is for a workflow instance.
            /// </summary>
            Instance,
            /// <summary>
            /// Pending work item is for a completed scope.
            /// </summary>
            CompletedScope,
            /// <summary>
            /// Pending work item represents a workflow unlock operation.
            /// </summary>
            UnlockWorkflow
        }
    }
}