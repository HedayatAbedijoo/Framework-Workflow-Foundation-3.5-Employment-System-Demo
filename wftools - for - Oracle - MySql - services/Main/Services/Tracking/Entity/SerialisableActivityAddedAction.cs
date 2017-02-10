using System;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Represents the addition of an activity to a workflow.
    /// </summary>
    public class SerialisableActivityAddedAction : SerialisableActivityChangeAction
    {
        /// <summary>
        /// Constructs a new <see cref="SerialisableActivityAddedAction" /> using
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
        public SerialisableActivityAddedAction(Type activityType, 
            String qualifiedName, String parentQualifiedName, Int32 order, 
            String activityXoml) : base(activityType, qualifiedName, 
            parentQualifiedName, order, activityXoml) { }
    }
}
