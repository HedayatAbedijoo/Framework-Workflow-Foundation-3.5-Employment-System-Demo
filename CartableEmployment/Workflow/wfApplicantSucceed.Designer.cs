using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Workflow
{
    partial class wfApplicantSucceed
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
            this.goNext2 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo2 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.goNext1 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo1 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.eventDrivenActivity2 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity2 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity1 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
            this.DeliverJobState = new System.Workflow.Activities.StateActivity();
            this.CompleteDocumentState = new System.Workflow.Activities.StateActivity();
            this.CallEmployeeState = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity2
            // 
            this.setStateActivity2.Name = "setStateActivity2";
            this.setStateActivity2.TargetStateName = "DeliverJobState";
            // 
            // goNext2
            // 
            this.goNext2.E = null;
            this.goNext2.Name = "goNext2";
            // 
            // onPageToGoTo2
            // 
            activitybind1.Name = "wfApplicantSucceed";
            activitybind1.Path = "PageToGoToEventArgs";
            this.onPageToGoTo2.Name = "onPageToGoTo2";
            this.onPageToGoTo2.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo2.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "CompleteDocumentState";
            // 
            // goNext1
            // 
            this.goNext1.E = null;
            this.goNext1.Name = "goNext1";
            // 
            // onPageToGoTo1
            // 
            activitybind2.Name = "wfApplicantSucceed";
            activitybind2.Path = "PageToGoToEventArgs";
            this.onPageToGoTo1.Name = "onPageToGoTo1";
            this.onPageToGoTo1.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo1.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // eventDrivenActivity2
            // 
            this.eventDrivenActivity2.Activities.Add(this.goNext2);
            this.eventDrivenActivity2.Activities.Add(this.setStateActivity2);
            this.eventDrivenActivity2.Name = "eventDrivenActivity2";
            // 
            // stateInitializationActivity2
            // 
            this.stateInitializationActivity2.Activities.Add(this.onPageToGoTo2);
            this.stateInitializationActivity2.Name = "stateInitializationActivity2";
            // 
            // eventDrivenActivity1
            // 
            this.eventDrivenActivity1.Activities.Add(this.goNext1);
            this.eventDrivenActivity1.Activities.Add(this.setStateActivity1);
            this.eventDrivenActivity1.Name = "eventDrivenActivity1";
            // 
            // stateInitializationActivity1
            // 
            this.stateInitializationActivity1.Activities.Add(this.onPageToGoTo1);
            this.stateInitializationActivity1.Name = "stateInitializationActivity1";
            // 
            // DeliverJobState
            // 
            this.DeliverJobState.Name = "DeliverJobState";
            // 
            // CompleteDocumentState
            // 
            this.CompleteDocumentState.Activities.Add(this.stateInitializationActivity2);
            this.CompleteDocumentState.Activities.Add(this.eventDrivenActivity2);
            this.CompleteDocumentState.Name = "CompleteDocumentState";
            // 
            // CallEmployeeState
            // 
            this.CallEmployeeState.Activities.Add(this.stateInitializationActivity1);
            this.CallEmployeeState.Activities.Add(this.eventDrivenActivity1);
            this.CallEmployeeState.Name = "CallEmployeeState";
            // 
            // wfApplicantSucceed
            // 
            this.Activities.Add(this.CallEmployeeState);
            this.Activities.Add(this.CompleteDocumentState);
            this.Activities.Add(this.DeliverJobState);
            this.CompletedStateName = "DeliverJobState";
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "CallEmployeeState";
            this.Name = "wfApplicantSucceed";
            this.PageToGoToEventArgs = null;
            this.RecievedEventDataEventsArgs = null;
            this.CanModifyActivities = false;

        }

        #endregion

        private StateActivity CompleteDocumentState;
        private StateActivity DeliverJobState;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo1;
        private StateInitializationActivity stateInitializationActivity2;
        private StateInitializationActivity stateInitializationActivity1;
        private SetStateActivity setStateActivity2;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext2;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo2;
        private SetStateActivity setStateActivity1;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext1;
        private EventDrivenActivity eventDrivenActivity2;
        private EventDrivenActivity eventDrivenActivity1;
        private StateActivity CallEmployeeState;









    }
}
