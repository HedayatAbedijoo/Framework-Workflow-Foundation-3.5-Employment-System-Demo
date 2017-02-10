using System;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents the removal of an activity from a workflow.
    /// </summary>
    public class SerialisableActivityRemovedAction : SerialisableActivityChangeAction
    {
        /// <summary>
        /// Constructs a new <see cref="SerialisableActivityRemovedAction" /> using
        /// the specified parameters.
        /// </summary>
        /// <param name="activityType">
        /// <see cref="Type" /> of activity that was removed.
        /// </param>
        /// <param name="qualifiedName">
        /// Fully qualified name of the activity that was removed.
        /// </param>
        /// <param name="parentQualifiedName">
        /// Fully qualified name of the parent of the activity.
        /// </param>
        /// <param name="order">
        /// Order in which the activity was removed.
        /// </param>
        /// <param name="activityXoml">
        /// XAML for the activity.
        /// </param>
        public SerialisableActivityRemovedAction(Type activityType,
            String qualifiedName, String parentQualifiedName, Int32 order, 
            String activityXoml) : base(activityType, qualifiedName, 
            parentQualifiedName, order, activityXoml) { }
    }
}
