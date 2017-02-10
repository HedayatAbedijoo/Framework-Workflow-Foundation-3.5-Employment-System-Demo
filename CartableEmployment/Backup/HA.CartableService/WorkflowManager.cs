using System;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Runtime.Hosting;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HA.Workflow.Services.Interfaces;
using HA.Workflow.Services.Services;
using System.Workflow.ComponentModel;
using HA.Workflow.Services.BaseClasses;
using Workflow;
using System.Threading;
/// <summary>
/// Wrapper class used by the asp.net host application to communicate
/// with the navigational workflow.
/// </summary>
namespace HA.CartableService
{
    public class WorkflowManager : IDisposable
    {
        public WorkflowManager(System.Web.HttpApplicationState application, System.Web.SessionState.HttpSessionState session, System.Web.HttpRequest request, System.Web.HttpResponse response)
        {
            // get reference to asp.net members
            this.Application = application;
            this.Session = session;
            this.Request = request;
            this.Response = response;

            _PageToGoTO = null;
            this.EventWatingDataSet = null;
            this.PreviousStateName = null;
        }

        #region Workflow Actions

        /// <summary>
        /// GoToNext State by Condition
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public string GoNext(string NextState)
        {
            // prepare external data exchange arguments
            OrderedDictionary parameters = new OrderedDictionary();
            parameters.Add("NextState", NextState);
            StateBehaveArgs args = new StateBehaveArgs(this.CurrentWorkflowInstanceId, parameters);
            // make local service call into Navigation Workflow
            _WorkflowNavigationServices.OnGoNext(args);

            // execute the workflow on the current thread and return the page to go to
            return GetPageToGoTo(args.InstanceId);
        }
        public string GoNext(object DataParameters)
        {
            // prepare external data exchange arguments
            OrderedDictionary parameters = new OrderedDictionary();
            parameters.Add("EntityBase", DataParameters);
            StateBehaveArgs args = new StateBehaveArgs(this.CurrentWorkflowInstanceId, parameters);
            // make local service call into Navigation Workflow

            _WorkflowNavigationServices.OnGoNext(args);

            // execute the workflow on the current thread and return the page to go to
            return GetPageToGoTo(args.InstanceId);
        }

        /// <summary>
        /// Rehydrate the workflow with the given instance id by raising the rehydrate 
        /// event inside the navigational workflow and then return the page to go to.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public string Rehydrate(Guid instanceId)
        {
            try
            {
                // put the workflow instance id in session 
                this.CurrentWorkflowInstanceId = instanceId;

                // prepare external data exchange arguments
                NavigationFlowEventArgs arg = new NavigationFlowEventArgs(instanceId, string.Empty, null);

                // make local service call into Navigation Workflow to retrieve step user left off at
                _WorkflowNavigationServices.OnRehydrated(arg);

                // execute the workflow on the current thread and return the page to go to
                return GetPageToGoTo(arg.InstanceId);
            }
            catch (Exception ex)
            {
                // the given workflow instance could not be rehydrated
                return GetStartPage();
            }
        }

        /// <summary>
        /// Creates a new workflow instance and returns the page name 
        /// of the initial state to go to (which is default.aspx).
        /// </summary>
        /// <returns></returns>
        public string StartNewWorkflow()
        {
            // create and start the navigational workflow instance

            WorkflowInstance instance = _WorkflowRuntime.CreateWorkflow(typeof(WorkflowEmployment));

            instance.Start();

            // execute the workflow synchronously on the current thread
            this.CurrentWorkflowInstanceId = instance.InstanceId;

            RunWorkflow(instance.InstanceId);

            // put the workflow instance id in session for this user
            this.CurrentWorkflowInstanceId = instance.InstanceId;

            // execute the workflow on the current thread and return the page to go to
            return this._PageToGoTO; //GetPageToGoTo(instance.InstanceId);
        }

        #endregion

        #region Fetch WorkflowRuntime And WorkflowServices Event
        EventHandler<WorkflowCompletedEventArgs> __completedHandler = null;
        EventHandler<NavigationFlowEventArgs> __PageToGoToReceived = null;
        EventHandler<SubWorkflowArgs> __SubworkflowStarted = null;
        EventHandler<SubWorkflowArgs> __AllSubWorkflowFinished = null;
        EventHandler<SubWorkflowArgs> __SubWorkflowFinished = null;
        EventHandler<SubWorkflowArgs> __SubWorkflowInstanceStart = null;

        private void InitWorkflowEvents()
        {
            AutoResetEvent waitHandle = new AutoResetEvent(false);


            __completedHandler = delegate(object sender, WorkflowCompletedEventArgs e)
            {
                if (this.CurrentWorkflowInstanceId == e.WorkflowInstance.InstanceId)
                {
                    this.WorkflowFinished(sender, e);
                    //_WorkflowRuntime.WorkflowCompleted -= __completedHandler;
                    waitHandle.Set();
                }
            };
            _WorkflowRuntime.WorkflowCompleted += __completedHandler;
            /////////////////////////////////////
            __PageToGoToReceived = delegate(object sender, NavigationFlowEventArgs e)
            {
                if (this.CurrentWorkflowInstanceId == e.InstanceId || this.SubWorkflowInstanceId == e.InstanceId || this.ParentWorkflowId == e.InstanceId)
                {
                    this.NavigationServices_PageToGoToReceived(sender, e);
                    //_WorkflowServices.PageToGoToReceived -= __PageToGoToReceived;
                    waitHandle.Set();
                }
            };
            _WorkflowNavigationServices.PageToGoToReceived += __PageToGoToReceived;
            /////////////////////////////////////
            __SubWorkflowInstanceStart = delegate(object sender, SubWorkflowArgs e)
            {
                if (this.CurrentWorkflowInstanceId == e.ParentWorkflowId)
                {
                    this.SubWorkflowInstanceId = e.InstanceId;
                    waitHandle.Set();
                }
            };
            _SubWorkflowService.SubWorkflowInstanceStart += __SubWorkflowInstanceStart;
            /////////////////////////////////////
            __SubworkflowStarted = delegate(object sender, SubWorkflowArgs e)
            {
                if (this.CurrentWorkflowInstanceId == e.ParentWorkflowId)
                {
                    this.SubWorkflowStart(sender, e);
                    //_CallWorkflowService.SubWorkflowStart -= __SubworkflowStarted;
                    waitHandle.Set();
                }
            };
            _SubWorkflowService.SubWorkflowStart += __SubworkflowStarted;
            /////////////////////////////////////
            __SubWorkflowFinished = delegate(object sender, SubWorkflowArgs e)
            {
                if (this.CurrentWorkflowInstanceId == e.InstanceId)
                {
                    this.SubWorkflowFinished(sender, e);
                    //  _CallWorkflowService.SubWorkflowFinished -= __SubWorkflowFinished;
                    waitHandle.Set();
                }
            };
            _SubWorkflowService.SubWorkflowFinished += __SubWorkflowFinished;

            /////////////////////////////////////
            __AllSubWorkflowFinished = delegate(object sender, SubWorkflowArgs e)
            {
                if (this.ParentWorkflowId == e.ParentWorkflowId)
                {
                    this.AllSubWorkflowFinished(sender, e);
                    waitHandle.Set();
                }
            };
            _SubWorkflowService.AllSubWorkflowFinished += __AllSubWorkflowFinished;

        }
        private void DisposeWorkflowEvents()
        {
            _WorkflowRuntime.WorkflowCompleted -= __completedHandler;
            _WorkflowNavigationServices.PageToGoToReceived -= __PageToGoToReceived;
            _SubWorkflowService.SubWorkflowStart -= __SubworkflowStarted;
            _SubWorkflowService.SubWorkflowFinished -= __SubWorkflowFinished;
            _SubWorkflowService.AllSubWorkflowFinished -= __AllSubWorkflowFinished;
            _SubWorkflowService.SubWorkflowInstanceStart -= __SubWorkflowInstanceStart;

        }



        void WorkflowFinished(object sender, WorkflowCompletedEventArgs e)
        {
            if (WorkflowCompleted != null)
                WorkflowCompleted(sender, e);
        }
        void SubWorkflowStart(object sender, SubWorkflowArgs e)
        {
            if (SubWorkflowStarted != null)
                SubWorkflowStarted(sender, e);
        }
        void SubWorkflowFinished(object sender, SubWorkflowArgs e)
        {
            // this.ParentWorkflowId = e.ParentWorkflowId;

            if (SubWorkflowCompleted != null)
                SubWorkflowCompleted(sender, e);

            RunWorkflow(e.ParentWorkflowId);
        }
        void AllSubWorkflowFinished(object sender, SubWorkflowArgs e)
        {
            if (AllSubWorkflowsCompleted != null)
                AllSubWorkflowsCompleted(sender, e);
            //this.CurrentWorkflowInstanceId = e.ParentWorkflowId;
        }
        void NavigationServices_PageToGoToReceived(object sender, NavigationFlowEventArgs e)
        {
            this.PreviousStateName = e.Parameters["PreviousStateName"] as string;
            this.EventWatingDataSet = e.Parameters["dsNextEvents"] as DataSet;
            _PageToGoTO = e.Parameters["CurentStateName"] as string;
            //  _PageToGoTO = e.PageToGoTo;
        }

        #endregion

        #region Properties

        public DataTable GetActionName()
        {

            if (this.CurrentWorkflowInstanceId == Guid.Empty)
                return null;
            StateMachineWorkflowInstance instance =
                new StateMachineWorkflowInstance(this.GetWorkflowRuntime(), this.CurrentWorkflowInstanceId);
            Dictionary<string, string> nextStates = GetAllTransitionState(instance);

            DataTable dtNextEvents = new DataTable();
            dtNextEvents.Columns.Add("EventName", typeof(string));
            dtNextEvents.Columns.Add("EventAlias", typeof(string));
            foreach (KeyValuePair<string, string> kvp in nextStates)
            {
                dtNextEvents.Rows.Add(kvp.Key, kvp.Value);

            }


            return dtNextEvents;

        }

        /// <summary>
        /// Gets or sets the workflow instance id currently in use.
        /// </summary>
        public Guid CurrentWorkflowInstanceId
        {
            get { return Session[WorkflowInstanceIdKey] == null ? Guid.Empty : (Guid)Session[WorkflowInstanceIdKey]; }
            set { Session[WorkflowInstanceIdKey] = value; }
        }

        public Guid SubWorkflowInstanceId
        {
            get { return Session[SubWorkflowInstanceIdKey] == null ? Guid.Empty : (Guid)Session[SubWorkflowInstanceIdKey]; }
            set { Session[SubWorkflowInstanceIdKey] = value; }
        }
        private WorkflowInstance CurrentWorkflowInstance
        {
            get { return Session[WorkflowInstanceIdKey] == null ? null : (WorkflowInstance)Session[WorkflowInstanceIdKey]; }

        }
        /// <summary>
        /// Return the starting page of all navigational workflows.
        /// </summary>
        /// <returns></returns>
        public string GetStartPage()
        {
            // this is a special function in that it knows where to look in the web.config to retrieve the initial page
            return StatesHelper.GetStartPage();
        }

        public Byte[] GetEventWaitingByteStream
        {
            get
            {
                return DataSet2ByteArray();
            }
        }

        private NavigationService _WorkflowNavigationServices
        {
            get { return this._WorkflowRuntime.GetService<NavigationService>(); }
        }
        private SubWorkflowService _SubWorkflowService
        {
            get { return this._WorkflowRuntime.GetService<SubWorkflowService>(); }
        }
        private WorkflowRuntime _WorkflowRuntime
        {
            get { return WorkflowRuntimeHelper.WorkflowRuntime; }
        }

        public DataSet EventWatingDataSet
        {
            get { return Session["EventWatingDataSet"] == null ? null : (DataSet)Session["EventWatingDataSet"]; }
            set { Session["EventWatingDataSet"] = value; }
        }
        public Guid ParentWorkflowId
        {
            get { return Session[ParentWorkflowInstanceIdKey] == null ? Guid.Empty : (Guid)Session[ParentWorkflowInstanceIdKey]; }
            set { Session[ParentWorkflowInstanceIdKey] = value; }
        }
        public string PreviousStateName { get; set; }
        public string _PageToGoTO
        {
            get { return Session["_PageToGoTO"] == null ? string.Empty : Session["_PageToGoTO"].ToString(); }
            set { Session["_PageToGoTO"] = value; }
        }


        #region Transfer Workflow Events to Outside
        public event EventHandler<SubWorkflowArgs> SubWorkflowStarted;

        public event EventHandler<SubWorkflowArgs> SubWorkflowCompleted;

        public event EventHandler<SubWorkflowArgs> AllSubWorkflowsCompleted;

        public event EventHandler<WorkflowCompletedEventArgs> WorkflowCompleted;
        #endregion
        #endregion

        #region Constants


        public const string ParentWorkflowInstanceIdKey = "ParentWorkflowInstanceIdKey";
        public const string SubWorkflowInstanceIdKey = "SubWorkflowInstanceId";
        public const string WorkflowInstanceIdKey = "WorkflowInstanceId";
        public const string WorkflowRuntimeKey = "WorkflowRuntime";
        public const string LocalServicesKey = "LocalServices";


        private readonly System.Web.HttpApplicationState Application;
        private readonly System.Web.SessionState.HttpSessionState Session;
        private readonly System.Web.HttpRequest Request;
        private readonly System.Web.HttpResponse Response;
        #endregion

        #region Private Method

        private void SetWorkflowQueueInfo(Guid instanceId, Guid ParentWorkflowId)
        {
            if (instanceId != Guid.Empty && ParentWorkflowId != Guid.Empty)
            {
                if (!this._SubWorkflowService.IsContainChildWorkflow(instanceId, ParentWorkflowId))
                {
                    WorkflowInstance parentInstance = this.GetWorkflowRuntime().GetWorkflow(ParentWorkflowId);
                    WorkflowInstance childInstance = this.GetWorkflowRuntime().GetWorkflow(instanceId);
                    Activity a = childInstance.GetWorkflowDefinition();
                    a.GetType();
                    ReadOnlyCollection<WorkflowQueueInfo> queue = parentInstance.GetWorkflowQueueData();
                    this._SubWorkflowService.SetChildWorkflow(queue, instanceId, ParentWorkflowId, a.Name);
                }
            }
        }

        /// <summary>
        /// Get a reference to the workflow runtime.
        /// </summary>
        /// <returns></returns>
        private WorkflowRuntime GetWorkflowRuntime()
        {
            //WorkflowRuntime workflowRuntime = Application[WorkflowRuntimeKey] as WorkflowRuntime;
            //return workflowRuntime;
            return WorkflowRuntimeHelper.WorkflowRuntime;

        }

        /// <summary>
        /// Get a reference to the manual workflow scheduler service.
        /// </summary>
        /// <returns></returns>
        private ManualWorkflowSchedulerService GetWorkflowSchedulerService()
        {
            ManualWorkflowSchedulerService scheduler = _WorkflowRuntime.GetService(typeof(ManualWorkflowSchedulerService)) as ManualWorkflowSchedulerService;
            return scheduler;
        }

        /// <summary>
        /// Run the workflow with the given instance id on the current thread. 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private bool RunWorkflow(Guid instanceId)
        {
            this.SetWorkflowQueueInfo(instanceId, this.ParentWorkflowId);
            ManualWorkflowSchedulerService scheduler = GetWorkflowSchedulerService();

            this.InitWorkflowEvents();
            bool result = scheduler.RunWorkflow(instanceId);
            this.DisposeWorkflowEvents();
            return result;
        }

        /// <summary>
        /// Runs the given workflow and returns the page to go to.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private string GetPageToGoTo(Guid instanceId)
        {
            // execute the workflow on the current asp.net thread
            RunWorkflow(instanceId);

            return _PageToGoTO;
        }

        public DataSet Byte2DataSet(byte[] bytDs)
        {
            if (bytDs == null) return null;
            DataSet outDs = new DataSet();

            MemoryStream inMs = new MemoryStream(bytDs);

            inMs.Seek(0, 0);

            using (DeflateStream zipStream = new DeflateStream(inMs, CompressionMode.Decompress, true))
            {

                byte[] outByt = bytDs; //ReadFullStream(zipStream);

                zipStream.Flush();

                zipStream.Close();

                MemoryStream outMs = new MemoryStream(outByt);

                outMs.Seek(0, 0);

                outDs.RemotingFormat = SerializationFormat.Binary;

                BinaryFormatter bf = new BinaryFormatter();



                outDs = (DataSet)bf.Deserialize(outMs, null);

            }

            return outDs;

        }
        private byte[] ReadFullStream(Stream stream)
        {

            byte[] buffer = new byte[4096];

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();

                    ms.Write(buffer, 0, read);
                }
            }
        }

        private byte[] DataSet2ByteArray()
        {
            MemoryStream ms = new MemoryStream();
            if (this.EventWatingDataSet == null)
                return null;
            else
            {
                new BinaryFormatter().Serialize(ms, this.EventWatingDataSet);
                return ms.ToArray();
            }
        }

        private Dictionary<string, string> GetAllTransitionState(StateMachineWorkflowInstance stateMachine)
        {
            //CompositeActivity act = stateMachine.CurrentState as CompositeActivity;

            Dictionary<string, string> transictionState = GetSetStateInfoRecursive(stateMachine.CurrentState);

            //foreach (Activity item in act.EnabledActivities)
            //{
            //    if (item is EventDrivenActivity)
            //    {
            //        EventDrivenActivity evn = item as EventDrivenActivity;
            //        foreach (Activity item2 in evn.EnabledActivities)
            //        {

            //        }
            //        if (item is SetStateActivity)
            //        {
            //            transictionState.Add(((SetStateActivity)item).TargetStateName, ((SetStateActivity)item).Description);
            //        }
            //    }
            //}

            //Collection<string> states = new Collection<string>();
            //foreach (string item in stateMachine.PossibleStateTransitions)
            //{

            //    states.Add(item);
            //}
            //Activity activ = stateMachine.StateMachineWorkflow.Activities[stateMachine.CurrentStateName];
            //CompositeActivity Comactiv = stateMachine.StateMachineWorkflow.Activities[stateMachine.CurrentStateName] as CompositeActivity;

            return transictionState;
        }

        private Dictionary<string, string> GetSetStateInfoRecursive(Activity act)
        {

            Dictionary<string, string> transictionState = new Dictionary<string, string>();
            CompositeActivity comAct = act as CompositeActivity;
            if (comAct != null)
            {
                foreach (Activity item in comAct.EnabledActivities)
                {
                    Dictionary<string, string> tmp = GetSetStateInfoRecursive(item);

                    foreach (KeyValuePair<string, string> kvp in tmp)
                    {
                        transictionState.Add(kvp.Key, kvp.Value);
                    }


                }
            }
            else
            {
                if (act is SetStateActivity)
                {
                    transictionState.Add(((SetStateActivity)act).TargetStateName, ((SetStateActivity)act).Description);
                }
            }
            return transictionState;
        }

        private Collection<string> GetNextEvents(StateMachineWorkflowInstance stateMachine)
        {
            Collection<string> events = new Collection<string>();


            Activity activ = stateMachine.StateMachineWorkflow.Activities[stateMachine.CurrentStateName];
            CompositeActivity act = stateMachine.StateMachineWorkflow.Activities[stateMachine.CurrentStateName] as CompositeActivity;

            CompositeActivity act2 = null;
            for (int i = 0; i < act.Activities.Count; i++)
            {
                act2 = act.Activities[i] as CompositeActivity;

                if (act2 is EventDrivenActivity)
                {
                    foreach (Activity act3 in act2.Activities)
                    {
                        if (act3 is HandleExternalEventActivity)
                        {
                            HandleExternalEventActivity handl = act3 as HandleExternalEventActivity;
                            events.Add(handl.EventName);

                        }
                    }
                }
            }

            return events;
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
