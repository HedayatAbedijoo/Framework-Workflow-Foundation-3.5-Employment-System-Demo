using System;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Abstract base class for all activity change actions.
    /// </summary>
    public abstract class SerialisableActivityChangeAction : SerialisableWorkflowChangeAction
    {
        /// <summary>
        /// Constructs a new <see cref="SerialisableActivityChangeAction" /> using
        /// the specified parameters.
        /// </summary>
        /// <param name="activityType">
        /// <see cref="Type" /> of activity that was added.
        /// </param>
        /// <param name="qualifiedName">
        /// Fully qualified name of the activity that was added.
        /// </param>
        /// <param name="parentQualifiedName">
        /// Fully qualified name of the parent of the activity.
        /// </param>
        /// <param name="order">
        /// Order in which the activity was added.
        /// </param>
        /// <param name="activityXoml">
        /// XAML for the activity.
        /// </param>
        protected SerialisableActivityChangeAction(Type activityType,
            String qualifiedName, String parentQualifiedName, Int32 order,
            String activityXoml)
        {
            _activityType = activityType;
            _qualifiedName = qualifiedName;
            _parentQualifiedName = parentQualifiedName;
            _order = order;
            _activityXoml = activityXoml;
        }

        private Type _activityType;
        /// <summary>
        /// Gets/sets the <see cref="Type" /> of activity which was changed.
        /// </summary>
        public Type ActivityType
        {
            get { return _activityType; }
            set { _activityType = value; }
        }

        private string _qualifiedName;
        /// <summary>
        /// Gets/sets the fully qualified name of the activity in the workflow instance.
        /// </summary>
        public string QualifiedName
        {
            get { return _qualifiedName; }
            set { _qualifiedName = value; }
        }

        private string _parentQualifiedName;
        /// <summary>
        /// Gets/sets the fully qualified name of the parent activity in the workflow instance.
        /// </summary>
        public string ParentQualifiedName
        {
            get { return _parentQualifiedName; }
            set { _parentQualifiedName = value; }
        }

        private int _order;
        /// <summary>
        /// Gets/sets the order in which the activity was changed.
        /// </summary>
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        private string _activityXoml;
        /// <summary>
        /// Gets/sets the XAML for the activity that was changed.
        /// </summary>
        public string ActivityXoml
        {
            get { return _activityXoml; }
            set { _activityXoml = value; }
        }
    }
}