using System;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents a summary of workflow type information that is used by 
    /// the <see cref="GenericTrackingService" /> to persist 
    /// tracking information somewhere useful.
    /// </summary>
    public class WorkflowTypeSummary
    {
        ///<summary>
        /// Constructs a new <see cref="WorkflowTypeSummary" /> using the specified type.
        ///</summary>
        ///<param name="type">
        /// <see cref="Type" /> of the workflow.
        /// </param>
        public WorkflowTypeSummary(Type type)
        {
            _type = type;
        }

        private Object _internalId;
        /// <summary>
        /// Gets/sets the unique identifier assigned to this object by an
        /// the underlying persistence provider.
        /// </summary>
        public Object InternalId
        {
            get { return _internalId; }
            set { _internalId = value; }
        }

        private Type _type;
        /// <summary>
        /// Gets/sets the <see cref="Type" /> of the workflow.
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
