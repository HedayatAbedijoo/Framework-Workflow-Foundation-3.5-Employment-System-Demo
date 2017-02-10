using System;
using System.Workflow.Activities;

namespace WFTools.Samples.WorkFlow.Services
{
    /// <summary>
    /// The service interface used for communicating with the 
    /// sample state machine workflow.
    /// </summary>
    [ExternalDataExchange]
    public interface IStateMachineService
	{
        /// <summary>
        /// Event raised when the workflow should be completed.
        /// </summary>
        event EventHandler<ExternalDataEventArgs> Completed;
	}
}
