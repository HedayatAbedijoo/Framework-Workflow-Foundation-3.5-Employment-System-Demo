using System.Workflow.Runtime.Hosting;
using System.Workflow.ComponentModel;
using System;
using System.Collections.Generic;
using System.Workflow.Runtime;
using System.Runtime.Remoting.Messaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using HA.Workflow.Services.Interfaces;
namespace HA.Workflow.Services.Services
{
    public class SubWorkflowService : WorkflowRuntimeService
    {
        protected override void OnStarted()
        {
            base.OnStarted();
            this.Runtime.WorkflowCompleted += WorkflowCompleted;
            this.Runtime.WorkflowTerminated += WorkflowTerminated;
            this.Runtime.WorkflowLoaded += WorkflowLoaded;
            this.Runtime.WorkflowUnloaded += new EventHandler<WorkflowEventArgs>(Runtime_WorkflowUnloaded);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
        protected override void Stop()
        {
            base.Stop();
        }
        void Runtime_WorkflowUnloaded(object sender, WorkflowEventArgs e)
        {
            if (ParentId != Guid.Empty && ParentId != e.WorkflowInstance.InstanceId)
            {
                SubWorkflowArgs arg = new SubWorkflowArgs(e.WorkflowInstance.InstanceId, e.WorkflowInstance.InstanceId, ParentId, null);
                this.OnSubWorkflowStart(arg);
                ParentId = Guid.Empty;
            }
        }
        void WorkflowCompleted(object o, WorkflowCompletedEventArgs e)
        {
            if (_childWorkflows.ContainsKey(e.WorkflowInstance.InstanceId)) //بعد از ایجاد زیر ورک فلو‌ها و بدون بسته شدن اپلیکیشن لیست کیو‌های زیر ورک فلو‌ها درون لیست باقی می‌ماند
            {
                Guid parentWorkflowId = Enqueue(e.WorkflowInstance.InstanceId, _childWorkflows[e.WorkflowInstance.InstanceId], e.OutputParameters);
                SubWorkflowArgs arg = new SubWorkflowArgs(e.WorkflowInstance.InstanceId, e.WorkflowInstance.InstanceId, parentWorkflowId, null);
                this.OnSubWorkflowFinished(arg);
            }
            else
                this.OnWorkflowCompelte(e);
        }
        void WorkflowTerminated(object o, WorkflowTerminatedEventArgs e)
        {
            if (_childWorkflows.ContainsKey(e.WorkflowInstance.InstanceId))
            {
                Guid parentWorkflowId = Enqueue(e.WorkflowInstance.InstanceId, _childWorkflows[e.WorkflowInstance.InstanceId], new Exception("Called Workflow Terminated", e.Exception));
            }
        }
        Guid Enqueue(Guid callee, ChildData cd, object prms)
        {
            Guid caller = cd.Parent;
            WorkflowInstance c = this.Runtime.GetWorkflow(caller);
            IComparable qn = cd.QueueName;
            c.EnqueueItem(qn, prms, null, null);
            _childWorkflows.Remove(callee);

            return caller;
        }
        void WorkflowLoaded(object o, WorkflowEventArgs e)
        {
            Guid callee = e.WorkflowInstance.InstanceId;
            //avoids having to query metadata if already loaded
            if (!_childWorkflows.ContainsKey(callee))
            {
                Activity a = e.WorkflowInstance.GetWorkflowDefinition();
                Guid caller = GetCaller(a);
                if (caller != Guid.Empty)
                    _childWorkflows.Add(callee, new ChildData(caller, GetQueueName(a)));
            }
        }
        IComparable GetQueueName(Activity a)
        {
            IComparable qn = a.GetValue(CallingWorkflowQueueNameProperty) as IComparable;
            return qn;

        }
        Guid GetCaller(Activity a)
        {
            Guid ret = Guid.Empty;
            object caller = a.GetValue(CallingWorkflowIDProperty);

            if (caller != null)
                ret = (Guid)caller;
            return ret;
        }
        class ChildData
        {
            public ChildData(Guid p, IComparable qn)
            {
                this.Parent = p;
                this.QueueName = qn;
            }
            internal Guid Parent;
            internal IComparable QueueName;
        }
        Dictionary<Guid, ChildData> _childWorkflows = new Dictionary<Guid, ChildData>();
        public static DependencyProperty CallingWorkflowIDProperty = System.Workflow.ComponentModel.DependencyProperty.RegisterAttached("CallingWorkflowID", typeof(Guid), typeof(SubWorkflowService));
        public static DependencyProperty CallingWorkflowQueueNameProperty = System.Workflow.ComponentModel.DependencyProperty.RegisterAttached("CallingWorkflowQueueName", typeof(IComparable), typeof(SubWorkflowService));

        public void StartWorkflow(Type workflowType, Dictionary<string, object> inparms, Guid caller, IComparable qn, ActivityExecutionContext context)
        {
            this.ParentId = caller;

            WorkflowRuntime wr = this.Runtime;
            CallContext.SetData(CallingWorkflowIDProperty.Name, caller);
            CallContext.SetData(CallingWorkflowQueueNameProperty.Name, qn);


            //wr.WorkflowCreated += new EventHandler<WorkflowEventArgs>(wr_WorkflowCreated);
            WorkflowInstance wi = wr.CreateWorkflow(workflowType, inparms);
            // wr.WorkflowCompleted += new EventHandler<WorkflowCompletedEventArgs>(wr_WorkflowCompleted);
            _childWorkflows.Add(wi.InstanceId, new ChildData(caller, qn));

            //SubWorkflowArgs arg = new SubWorkflowArgs(wi.InstanceId, wi.InstanceId, caller, null);
            //this.OnStartSubWorkflow(arg);

            wi.Start();


            //// Call Workflow Service when the SubWorkflow Started
            //WorkflowServices workflowServicde = context.GetService<WorkflowServices>();
            SubWorkflowArgs arg = new SubWorkflowArgs(wi.InstanceId, wi.InstanceId, caller, null);
            this.OnSubWorkflowInstanceStart(arg);
            //////////////////////
            ManualWorkflowSchedulerService ss = wr.GetService<ManualWorkflowSchedulerService>();

            if (ss != null)
                ss.RunWorkflow(wi.InstanceId);

        }

        public Guid ParentId { get; set; }

        public void OnFinishCallworkflowActivity(Guid instanceId)
        {
            WorkflowInstance instance = this.Runtime.GetWorkflow(instanceId);
            ReadOnlyCollection<WorkflowQueueInfo> qeue = instance.GetWorkflowQueueData();
            bool flag = true;
            foreach (WorkflowQueueInfo item in qeue)
            {
                //string regExp = instanceId + ":" + @"\w*:\w*";
                //if (Regex.IsMatch(item.QueueName.ToString(), regExp))
                if (item.QueueName.ToString().Contains("ThisIsSubWorkflowSupportForkJoin") && item.QueueName.ToString().Contains(instanceId.ToString()))
                    flag = false;
                //		QueueName	"2dbe6430-4ef3-432e-af2c-954d2a5cfa88:Subworkflow2:callWorkflowActivity1"	System.IComparable {string}
            }

            if (flag)
            {
                SubWorkflowArgs arg = new SubWorkflowArgs(Guid.Empty, Guid.Empty, instanceId, null);
                this.OnAllSubWorkflowFinished(arg);
            }
        }

        public void SetChildWorkflow(ReadOnlyCollection<WorkflowQueueInfo> queueInfo, Guid childWorkflowId, Guid parentWorkflowId, string childWorkflowName)
        {
            if (!_childWorkflows.ContainsKey(childWorkflowId))
            {
                foreach (WorkflowQueueInfo item in queueInfo)
                {
                    if (item.QueueName.ToString().Contains("ThisIsSubWorkflowSupportForkJoin") && item.QueueName.ToString().Contains(childWorkflowName))
                        _childWorkflows.Add(childWorkflowId, new ChildData(parentWorkflowId, item.QueueName));
                }
            }
        }
        public bool IsContainChildWorkflow(Guid workflowIdChild, Guid workflowIdParent)
        {
            if (_childWorkflows.ContainsKey(workflowIdChild) && _childWorkflows[workflowIdChild].QueueName.ToString().Contains(workflowIdParent.ToString()))
                return true;
            else
                return false;
        }

        #region ISubWorkflow Members

        public event EventHandler<SubWorkflowArgs> SubWorkflowStart;
        public void OnSubWorkflowStart(SubWorkflowArgs Arg)
        {
            if (SubWorkflowStart != null)
            {
                SubWorkflowStart(null, Arg);
            }
        }

        public event EventHandler<SubWorkflowArgs> SubWorkflowInstanceStart;
        public void OnSubWorkflowInstanceStart(SubWorkflowArgs Arg)
        {
            if (SubWorkflowInstanceStart != null)
            {
                SubWorkflowInstanceStart(null, Arg);
            }
        }

        public event EventHandler<SubWorkflowArgs> AllSubWorkflowFinished;
        public void OnAllSubWorkflowFinished(SubWorkflowArgs Arg)
        {
            if (AllSubWorkflowFinished != null)
            {
                AllSubWorkflowFinished(null, Arg);
            }
        }

        public event EventHandler<SubWorkflowArgs> SubWorkflowFinished;
        public void OnSubWorkflowFinished(SubWorkflowArgs Arg)
        {
            if (SubWorkflowFinished != null)
            {
                SubWorkflowFinished(null, Arg);
            }
        }

        public event EventHandler<WorkflowCompletedEventArgs> WorkflowComplete;
        public void OnWorkflowCompelte(WorkflowCompletedEventArgs Args)
        {
            if (WorkflowComplete != null)
                WorkflowComplete(null, Args);
        }


        /// If this events have Event handler
        public bool HasSubWorkflowStart
        {
            get
            {
                if (SubWorkflowStart != null) return true; else return false;
            }
        }
        public bool HasAllSubWorkflowFinished
        {
            get
            {
                if (AllSubWorkflowFinished != null) return true; else return false;
            }
        }
        public bool HasSubWorkflowFinished
        {
            get
            {
                if (SubWorkflowFinished != null) return true; else return false;
            }
        }
        public bool HasWorkflowComplete
        {
            get
            {
                if (WorkflowComplete != null)
                    return true;
                else
                    return false;
            }
        }


        #endregion
    }


}