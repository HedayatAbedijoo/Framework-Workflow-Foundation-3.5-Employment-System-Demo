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
using HA.Workflow.Services.Controls.ActivityConditionHA;

namespace Workflow
{
    partial class WorkflowEmployment
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
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha1 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha2 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha3 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha4 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha5 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha6 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind7 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA activityconditionha7 = new HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA();
            System.Workflow.ComponentModel.ActivityBind activitybind8 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind9 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind10 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind11 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind12 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind13 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo1 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo2 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            System.Workflow.ComponentModel.ActivityBind activitybind14 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind15 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo3 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo4 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo5 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            System.Workflow.ComponentModel.ActivityBind activitybind16 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind17 = new System.Workflow.ComponentModel.ActivityBind();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo6 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo eventinfo7 = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfo();
            this.setStateActivity6 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity5 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity9 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity3 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.ifElseBranchActivityCustom6 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom5 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom4 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom7 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom3 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom2 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom1 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.setStateActivity8 = new System.Workflow.Activities.SetStateActivity();
            this.goNext5 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo5 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.setStateActivity7 = new System.Workflow.Activities.SetStateActivity();
            this.goNext4 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo4 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.subWorkflowActivity1 = new HA.Workflow.Services.Controls.SubWorkflow.SubWorkflowActivity();
            this.ifElseActivityCustom3 = new Workflow.Activities.IfElse.IfElseActivityCustom();
            this.goNext3 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo1 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.ifElseActivityCustom2 = new Workflow.Activities.IfElse.IfElseActivityCustom();
            this.goNext2 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo3 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.ifElseActivityCustom1 = new Workflow.Activities.IfElse.IfElseActivityCustom();
            this.goNext1 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo2 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.eventDrivenActivity5 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity6 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity4 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity4 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity3 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity3 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity2 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity2 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity1 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
            this.RejectApplicant = new System.Workflow.Activities.StateActivity();
            this.Finish = new System.Workflow.Activities.StateActivity();
            this.ConfirmEmployement = new System.Workflow.Activities.StateActivity();
            this.Interview = new System.Workflow.Activities.StateActivity();
            this.CheckDocuments = new System.Workflow.Activities.StateActivity();
            this.RegisterApplicant = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity6
            // 
            this.setStateActivity6.Description = "رد درخواست";
            this.setStateActivity6.Name = "setStateActivity6";
            this.setStateActivity6.TargetStateName = "RejectApplicant";
            // 
            // setStateActivity5
            // 
            this.setStateActivity5.Description = "تایید متقاضی";
            this.setStateActivity5.Name = "setStateActivity5";
            this.setStateActivity5.TargetStateName = "ConfirmEmployement";
            // 
            // setStateActivity4
            // 
            this.setStateActivity4.Description = "رد درخواست";
            this.setStateActivity4.Name = "setStateActivity4";
            this.setStateActivity4.TargetStateName = "RejectApplicant";
            // 
            // setStateActivity9
            // 
            this.setStateActivity9.Description = "قبول درخواست";
            this.setStateActivity9.Name = "setStateActivity9";
            this.setStateActivity9.TargetStateName = "RegisterApplicant";
            // 
            // setStateActivity3
            // 
            this.setStateActivity3.Description = "برو به مصاحبه";
            this.setStateActivity3.Name = "setStateActivity3";
            this.setStateActivity3.TargetStateName = "Interview";
            // 
            // setStateActivity2
            // 
            this.setStateActivity2.Description = "برو به رد درخواست";
            this.setStateActivity2.Name = "setStateActivity2";
            this.setStateActivity2.TargetStateName = "RejectApplicant";
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Description = "برو به بررسی مستندات";
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "CheckDocuments";
            // 
            // ifElseBranchActivityCustom6
            // 
            this.ifElseBranchActivityCustom6.Activities.Add(this.setStateActivity6);
            activitybind1.Name = "WorkflowEmployment";
            activitybind1.Path = "RecievedEventDataEventsArgs";
            activityconditionha1.Rule = "InterviewUnSucceed";
            activityconditionha1.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.ConditionalTransit;
            activityconditionha1.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.ifElseBranchActivityCustom6.Condition = activityconditionha1;
            this.ifElseBranchActivityCustom6.Name = "ifElseBranchActivityCustom6";
            // 
            // ifElseBranchActivityCustom5
            // 
            this.ifElseBranchActivityCustom5.Activities.Add(this.setStateActivity5);
            activitybind2.Name = "WorkflowEmployment";
            activitybind2.Path = "RecievedEventDataEventsArgs";
            activityconditionha2.Rule = "InterviewSucceed";
            activityconditionha2.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.ConditionalTransit;
            activityconditionha2.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.ifElseBranchActivityCustom5.Condition = activityconditionha2;
            this.ifElseBranchActivityCustom5.Description = "GoToInterview";
            this.ifElseBranchActivityCustom5.Name = "ifElseBranchActivityCustom5";
            // 
            // ifElseBranchActivityCustom4
            // 
            this.ifElseBranchActivityCustom4.Activities.Add(this.setStateActivity4);
            activitybind3.Name = "WorkflowEmployment";
            activitybind3.Path = "RecievedEventDataEventsArgs";
            activityconditionha3.Rule = "";
            activityconditionha3.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha3.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.ifElseBranchActivityCustom4.Condition = activityconditionha3;
            this.ifElseBranchActivityCustom4.Name = "ifElseBranchActivityCustom4";
            // 
            // ifElseBranchActivityCustom7
            // 
            this.ifElseBranchActivityCustom7.Activities.Add(this.setStateActivity9);
            activitybind4.Name = "WorkflowEmployment";
            activitybind4.Path = "RecievedEventDataEventsArgs";
            activityconditionha4.Rule = "";
            activityconditionha4.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha4.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            this.ifElseBranchActivityCustom7.Condition = activityconditionha4;
            this.ifElseBranchActivityCustom7.Name = "ifElseBranchActivityCustom7";
            // 
            // ifElseBranchActivityCustom3
            // 
            this.ifElseBranchActivityCustom3.Activities.Add(this.setStateActivity3);
            activitybind5.Name = "WorkflowEmployment";
            activitybind5.Path = "RecievedEventDataEventsArgs";
            activityconditionha5.Rule = "";
            activityconditionha5.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha5.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            this.ifElseBranchActivityCustom3.Condition = activityconditionha5;
            this.ifElseBranchActivityCustom3.Description = "GoToInterview";
            this.ifElseBranchActivityCustom3.Name = "ifElseBranchActivityCustom3";
            // 
            // ifElseBranchActivityCustom2
            // 
            this.ifElseBranchActivityCustom2.Activities.Add(this.setStateActivity2);
            activitybind6.Name = "WorkflowEmployment";
            activitybind6.Path = "RecievedEventDataEventsArgs";
            activityconditionha6.Rule = "";
            activityconditionha6.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha6.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            this.ifElseBranchActivityCustom2.Condition = activityconditionha6;
            this.ifElseBranchActivityCustom2.Name = "ifElseBranchActivityCustom2";
            // 
            // ifElseBranchActivityCustom1
            // 
            this.ifElseBranchActivityCustom1.Activities.Add(this.setStateActivity1);
            activitybind7.Name = "WorkflowEmployment";
            activitybind7.Path = "RecievedEventDataEventsArgs";
            activityconditionha7.Rule = "";
            activityconditionha7.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha7.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind7)));
            this.ifElseBranchActivityCustom1.Condition = activityconditionha7;
            this.ifElseBranchActivityCustom1.Name = "ifElseBranchActivityCustom1";
            // 
            // setStateActivity8
            // 
            this.setStateActivity8.Description = "اتمام درخواست";
            this.setStateActivity8.Name = "setStateActivity8";
            this.setStateActivity8.TargetStateName = "Finish";
            // 
            // goNext5
            // 
            activitybind8.Name = "WorkflowEmployment";
            activitybind8.Path = "RecievedEventDataEventsArgs";
            this.goNext5.Name = "goNext5";
            this.goNext5.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind8)));
            // 
            // onPageToGoTo5
            // 
            activitybind9.Name = "WorkflowEmployment";
            activitybind9.Path = "PageToGoToEventArgs";
            this.onPageToGoTo5.Name = "onPageToGoTo5";
            this.onPageToGoTo5.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo5.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind9)));
            // 
            // setStateActivity7
            // 
            this.setStateActivity7.Description = "اتمام درخواست";
            this.setStateActivity7.Name = "setStateActivity7";
            this.setStateActivity7.TargetStateName = "Finish";
            // 
            // goNext4
            // 
            activitybind10.Name = "WorkflowEmployment";
            activitybind10.Path = "RecievedEventDataEventsArgs";
            this.goNext4.Name = "goNext4";
            this.goNext4.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind10)));
            // 
            // onPageToGoTo4
            // 
            activitybind11.Name = "WorkflowEmployment";
            activitybind11.Path = "PageToGoToEventArgs";
            this.onPageToGoTo4.Name = "onPageToGoTo4";
            this.onPageToGoTo4.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo4.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind11)));
            // 
            // subWorkflowActivity1
            // 
            this.subWorkflowActivity1.Name = "subWorkflowActivity1";
            this.subWorkflowActivity1.Type = typeof(Workflow.wfApplicantSucceed);
            // 
            // ifElseActivityCustom3
            // 
            this.ifElseActivityCustom3.Activities.Add(this.ifElseBranchActivityCustom5);
            this.ifElseActivityCustom3.Activities.Add(this.ifElseBranchActivityCustom6);
            this.ifElseActivityCustom3.Name = "ifElseActivityCustom3";
            // 
            // goNext3
            // 
            activitybind12.Name = "WorkflowEmployment";
            activitybind12.Path = "RecievedEventDataEventsArgs";
            this.goNext3.Name = "goNext3";
            this.goNext3.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind12)));
            // 
            // onPageToGoTo1
            // 
            activitybind13.Name = "WorkflowEmployment";
            activitybind13.Path = "PageToGoToEventArgs";
            eventinfo1.EventAlias = "رد درخواست";
            eventinfo1.EventName = "RejectApplicant";
            eventinfo2.EventAlias = "قبول درخواست";
            eventinfo2.EventName = "ConfirmApplicant";
            this.onPageToGoTo1.Name = "onPageToGoTo1";
            this.onPageToGoTo1.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo1.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind13)));
            // 
            // ifElseActivityCustom2
            // 
            this.ifElseActivityCustom2.Activities.Add(this.ifElseBranchActivityCustom3);
            this.ifElseActivityCustom2.Activities.Add(this.ifElseBranchActivityCustom7);
            this.ifElseActivityCustom2.Activities.Add(this.ifElseBranchActivityCustom4);
            this.ifElseActivityCustom2.Name = "ifElseActivityCustom2";
            // 
            // goNext2
            // 
            activitybind14.Name = "WorkflowEmployment";
            activitybind14.Path = "RecievedEventDataEventsArgs";
            this.goNext2.Name = "goNext2";
            this.goNext2.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind14)));
            // 
            // onPageToGoTo3
            // 
            activitybind15.Name = "WorkflowEmployment";
            activitybind15.Path = "PageToGoToEventArgs";
            eventinfo3.EventAlias = "رد درخواست";
            eventinfo3.EventName = "RejectApplicant";
            eventinfo4.EventAlias = "مصاحبه و تست";
            eventinfo4.EventName = "GoToInterview";
            eventinfo5.EventAlias = "برگشت به مرحله ثبت نام";
            eventinfo5.EventName = "GoToRegister";
            this.onPageToGoTo3.Name = "onPageToGoTo3";
            this.onPageToGoTo3.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo3.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind15)));
            // 
            // ifElseActivityCustom1
            // 
            this.ifElseActivityCustom1.Activities.Add(this.ifElseBranchActivityCustom1);
            this.ifElseActivityCustom1.Activities.Add(this.ifElseBranchActivityCustom2);
            this.ifElseActivityCustom1.Name = "ifElseActivityCustom1";
            // 
            // goNext1
            // 
            activitybind16.Name = "WorkflowEmployment";
            activitybind16.Path = "RecievedEventDataEventsArgs";
            this.goNext1.Name = "goNext1";
            this.goNext1.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind16)));
            // 
            // onPageToGoTo2
            // 
            activitybind17.Name = "WorkflowEmployment";
            activitybind17.Path = "PageToGoToEventArgs";
            eventinfo6.EventAlias = "رد درخواست";
            eventinfo6.EventName = "RejectApplicant";
            eventinfo7.EventAlias = "بررسی مدارک";
            eventinfo7.EventName = "CheckDocuments";
            this.onPageToGoTo2.Name = "onPageToGoTo2";
            this.onPageToGoTo2.MethodInvoking += new System.EventHandler(this.InitializeOutgoingMessage);
            this.onPageToGoTo2.SetBinding(HA.Workflow.Services.Controls.Navigation.OnPageToGoTo.ArgsProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind17)));
            // 
            // eventDrivenActivity5
            // 
            this.eventDrivenActivity5.Activities.Add(this.goNext5);
            this.eventDrivenActivity5.Activities.Add(this.setStateActivity8);
            this.eventDrivenActivity5.Name = "eventDrivenActivity5";
            // 
            // stateInitializationActivity6
            // 
            this.stateInitializationActivity6.Activities.Add(this.onPageToGoTo5);
            this.stateInitializationActivity6.Name = "stateInitializationActivity6";
            // 
            // eventDrivenActivity4
            // 
            this.eventDrivenActivity4.Activities.Add(this.goNext4);
            this.eventDrivenActivity4.Activities.Add(this.setStateActivity7);
            this.eventDrivenActivity4.Name = "eventDrivenActivity4";
            // 
            // stateInitializationActivity4
            // 
            this.stateInitializationActivity4.Activities.Add(this.subWorkflowActivity1);
            this.stateInitializationActivity4.Activities.Add(this.onPageToGoTo4);
            this.stateInitializationActivity4.Name = "stateInitializationActivity4";
            // 
            // eventDrivenActivity3
            // 
            this.eventDrivenActivity3.Activities.Add(this.goNext3);
            this.eventDrivenActivity3.Activities.Add(this.ifElseActivityCustom3);
            this.eventDrivenActivity3.Name = "eventDrivenActivity3";
            // 
            // stateInitializationActivity3
            // 
            this.stateInitializationActivity3.Activities.Add(this.onPageToGoTo1);
            this.stateInitializationActivity3.Name = "stateInitializationActivity3";
            // 
            // eventDrivenActivity2
            // 
            this.eventDrivenActivity2.Activities.Add(this.goNext2);
            this.eventDrivenActivity2.Activities.Add(this.ifElseActivityCustom2);
            this.eventDrivenActivity2.Name = "eventDrivenActivity2";
            // 
            // stateInitializationActivity2
            // 
            this.stateInitializationActivity2.Activities.Add(this.onPageToGoTo3);
            this.stateInitializationActivity2.Name = "stateInitializationActivity2";
            // 
            // eventDrivenActivity1
            // 
            this.eventDrivenActivity1.Activities.Add(this.goNext1);
            this.eventDrivenActivity1.Activities.Add(this.ifElseActivityCustom1);
            this.eventDrivenActivity1.Name = "eventDrivenActivity1";
            // 
            // stateInitializationActivity1
            // 
            this.stateInitializationActivity1.Activities.Add(this.onPageToGoTo2);
            this.stateInitializationActivity1.Name = "stateInitializationActivity1";
            // 
            // RejectApplicant
            // 
            this.RejectApplicant.Activities.Add(this.stateInitializationActivity6);
            this.RejectApplicant.Activities.Add(this.eventDrivenActivity5);
            this.RejectApplicant.Name = "RejectApplicant";
            // 
            // Finish
            // 
            this.Finish.Name = "Finish";
            // 
            // ConfirmEmployement
            // 
            this.ConfirmEmployement.Activities.Add(this.stateInitializationActivity4);
            this.ConfirmEmployement.Activities.Add(this.eventDrivenActivity4);
            this.ConfirmEmployement.Name = "ConfirmEmployement";
            // 
            // Interview
            // 
            this.Interview.Activities.Add(this.stateInitializationActivity3);
            this.Interview.Activities.Add(this.eventDrivenActivity3);
            this.Interview.Name = "Interview";
            // 
            // CheckDocuments
            // 
            this.CheckDocuments.Activities.Add(this.stateInitializationActivity2);
            this.CheckDocuments.Activities.Add(this.eventDrivenActivity2);
            this.CheckDocuments.Name = "CheckDocuments";
            // 
            // RegisterApplicant
            // 
            this.RegisterApplicant.Activities.Add(this.stateInitializationActivity1);
            this.RegisterApplicant.Activities.Add(this.eventDrivenActivity1);
            this.RegisterApplicant.Name = "RegisterApplicant";
            // 
            // WorkflowEmployment
            // 
            this.Activities.Add(this.RegisterApplicant);
            this.Activities.Add(this.CheckDocuments);
            this.Activities.Add(this.Interview);
            this.Activities.Add(this.ConfirmEmployement);
            this.Activities.Add(this.Finish);
            this.Activities.Add(this.RejectApplicant);
            this.CompletedStateName = "Finish";
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "RegisterApplicant";
            this.Name = "WorkflowEmployment";
            this.PageToGoToEventArgs = null;
            this.RecievedEventDataEventsArgs = null;
            this.CanModifyActivities = false;

        }

        #endregion

        private StateInitializationActivity stateInitializationActivity4;
        private StateInitializationActivity stateInitializationActivity3;
        private StateInitializationActivity stateInitializationActivity2;
        private StateInitializationActivity stateInitializationActivity1;
        private StateActivity ConfirmEmployement;
        private StateActivity Interview;
        private StateActivity CheckDocuments;
        private StateActivity Finish;
        private StateActivity RejectApplicant;
        private StateInitializationActivity stateInitializationActivity6;
        private EventDrivenActivity eventDrivenActivity1;
        private EventDrivenActivity eventDrivenActivity2;
        private EventDrivenActivity eventDrivenActivity3;
        private EventDrivenActivity eventDrivenActivity4;
        private EventDrivenActivity eventDrivenActivity5;
        private SetStateActivity setStateActivity1;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext1;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom2;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom1;
        private Workflow.Activities.IfElse.IfElseActivityCustom ifElseActivityCustom1;
        private SetStateActivity setStateActivity2;
        private SetStateActivity setStateActivity4;
        private SetStateActivity setStateActivity3;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom4;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom3;
        private Workflow.Activities.IfElse.IfElseActivityCustom ifElseActivityCustom2;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext2;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo1;
        private SetStateActivity setStateActivity6;
        private SetStateActivity setStateActivity5;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom6;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom5;
        private Workflow.Activities.IfElse.IfElseActivityCustom ifElseActivityCustom3;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext3;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext4;
        private SetStateActivity setStateActivity7;
        private HA.Workflow.Services.Controls.Navigation.GoNext goNext5;
        private SetStateActivity setStateActivity8;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo2;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo3;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo5;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo4;
        private HA.Workflow.Services.Controls.SubWorkflow.SubWorkflowActivity subWorkflowActivity1;
        private SetStateActivity setStateActivity9;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom7;
        private StateActivity RegisterApplicant;











































































































    }
}
