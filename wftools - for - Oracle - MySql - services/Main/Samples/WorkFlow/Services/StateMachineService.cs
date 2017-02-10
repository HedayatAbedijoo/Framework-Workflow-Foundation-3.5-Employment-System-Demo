using System;
using System.Workflow.Activities;

namespace WFTools.Samples.WorkFlow.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IStateMachineService" />.
    /// </summary>
    public class StateMachineService : IStateMachineService
    {
        /// <summary>
        /// Event raised when the workflow should be completed.
        /// </summary>
        public event EventHandler<ExternalDataEventArgs> Completed;

        /// <summary>
        /// Raises the completed event to the specified workflow instance.
        /// </summary>
        /// <param name="instanceId">
        /// Unique identifier of a workflow instance.
        /// </param>
        public void OnCompleted(Guid instanceId)
        {
            if (Completed != null)
                Completed(null, new ExternalDataEventArgs(instanceId));

        }
    }
}
