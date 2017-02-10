using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Collections.Specialized;

namespace HA.Workflow.Services.Interfaces
{
    [ExternalDataExchange]
    interface ISubWorkflow
    {
        event EventHandler<SubWorkflowArgs> StartSubWorkflow;
        event EventHandler<SubWorkflowArgs> EndSubWorkflow;
    }

    [Serializable]
    public class SubWorkflowArgs : ExternalDataEventArgs
    {
        public IOrderedDictionary Parameters { get; set; }
        public Guid SubWorkflowId { get; set; }
        public Guid ParentWorkflowId { get; set; }
        public SubWorkflowArgs(Guid instanceId, Guid SubWorkflowId, Guid ParentWorkflowId, IOrderedDictionary parameters)
            : base(instanceId)
        {
            this.Parameters = parameters;
            this.SubWorkflowId = SubWorkflowId;
            this.ParentWorkflowId = ParentWorkflowId;
        }

    }
}
