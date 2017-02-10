using System;
using System.Workflow.Activities;
using System.Workflow.Runtime;

namespace WFTools.Utilities.Workflow
{
    /// <summary>
    /// Event arguments used for error handling performed by a <see cref="ErrorHandlingService" />.
    /// </summary>
    [Serializable]
    public class ErrorHandlingEventArgs : ExternalDataEventArgs
    {
        public ErrorHandlingEventArgs(Guid instanceId, Exception exception) : base(instanceId)
        {
            _exception = exception;
            WaitForIdle = true;
        }

        private readonly Exception _exception;
        /// <summary>
        /// The exception raised.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }

    /// <summary>
    /// Abstract implementation of <see cref="IErrorHandlingService" /> that can serve
    /// as a base class for other more functional services.
    /// </summary>
    public abstract class ErrorHandlingService : IErrorHandlingService
    {
        //HACK 
        // to get around anonymous delegate issues... anonymous delegates
        // tend not to be marked as serialisable when produced by the compiler
        // and therefore should not be serialised into the database
        [NonSerialized]
        private EventHandler<ErrorHandlingEventArgs> error;
        /// <summary>
        /// Event raised when an error occurs while executing a workflow.
        /// </summary>
        public event EventHandler<ErrorHandlingEventArgs> Error
        {
            add { error += value; }
            remove { error -= value; }
        }

        /// <summary>
        /// Raises an exception to the hosting application.
        /// </summary>
        /// <param name="exception">
        /// The <see cref="Exception" /> to raise.
        /// </param>
        public void RaiseError(Exception exception)
        {
            if (error != null)
                error(this, new ErrorHandlingEventArgs(WorkflowEnvironment.WorkflowInstanceId, exception));
        }
    }
}
