using System;
using System.Runtime.Serialization;

namespace WFTools.Services.Persistence
{
    /// <summary>
    /// Exception thrown when something goes wrong in a persistence service.
    /// </summary>
    public class WorkflowPersistenceException : Exception
    {
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public WorkflowPersistenceException() : base()
		{
		}

		/// <summary>
		/// Construct with a message.
		/// </summary>
		public WorkflowPersistenceException(string message) : base(message)
		{
		}

		/// <summary>
		/// Construct with a message and inner exception.
		/// </summary>
		public WorkflowPersistenceException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Used during serialisation.
		/// </summary>
        protected WorkflowPersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
    }
}
