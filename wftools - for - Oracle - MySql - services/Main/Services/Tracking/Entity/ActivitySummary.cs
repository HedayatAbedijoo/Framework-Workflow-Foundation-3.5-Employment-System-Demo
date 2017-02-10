using System;
using System.Collections.Generic;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents a summary of an activity in a workflow instance that is used by 
    /// the <see cref="GenericTrackingService" /> to persist 
    /// tracking information somewhere useful.
    /// </summary>
    public class ActivitySummary
    {
        /// <summary>
        /// Constructs a new <see cref="ActivitySummary" /> using the specified parameters.
        /// </summary>
        /// <param name="type">
        /// <see cref="Type" /> of the activity.
        /// </param>
        /// <param name="qualifiedName">
        /// Full qualified name of the activity.
        /// </param>
        public ActivitySummary(Type type, String qualifiedName)
        {
            _type = type;
            _qualifiedName = qualifiedName;
        }

        private Type _type;
        /// <summary>
        /// Gets/sets the <see cref="Type" /> of the activity.
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private String _qualifiedName;
        /// <summary>
        /// Gets/sets the fully qualified name of this activity in the workflow instance.
        /// </summary>
        public String QualifiedName
        {
            get { return _qualifiedName; }
            set { _qualifiedName = value; }
        }

        private ActivitySummary parentActivity;
        /// <summary>
        /// Gets/sets the parent <see cref="ActivitySummary" /> of this activity.
        /// </summary>
        public ActivitySummary ParentActivity
        {
            get { return parentActivity; }
            set { parentActivity = value; }
        }

        private readonly IList<ActivitySummary> childActivities = new List<ActivitySummary>();
        /// <summary>
        /// Gets/sets a list of children <see cref="ActivitySummary" /> objects 
        /// for this activity.
        /// </summary>
        public IList<ActivitySummary> ChildActivities
        {
            get { return childActivities; }
        }


    }
}
