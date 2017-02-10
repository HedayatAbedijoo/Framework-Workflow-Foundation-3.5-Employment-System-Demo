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
    partial class Workflow1
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
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.ifElseBranchActivityCustom2 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseBranchActivityCustom1 = new Workflow.Activities.IfElse.IfElseBranchActivityCustom();
            this.ifElseActivityCustom1 = new Workflow.Activities.IfElse.IfElseActivityCustom();
            this.goNext1 = new HA.Workflow.Services.Controls.Navigation.GoNext();
            this.onPageToGoTo1 = new HA.Workflow.Services.Controls.Navigation.OnPageToGoTo();
            this.eventDrivenActivity1 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
            this.RegisterApplicant = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "";
            // 
            // ifElseBranchActivityCustom2
            // 
            this.ifElseBranchActivityCustom2.Name = "ifElseBranchActivityCustom2";
            // 
            // ifElseBranchActivityCustom1
            // 
            this.ifElseBranchActivityCustom1.Activities.Add(this.setStateActivity1);
            activitybind1.Name = "Workflow1";
            activitybind1.Path = "RecievedEventDataEventsArgs";
            activityconditionha1.Rule = "";
            activityconditionha1.TransitionCondition = HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.enmTransitionCondition.DirectTransit;
            activityconditionha1.SetBinding(HA.Workflow.Services.Controls.ActivityConditionHA.ActivityConditionHA.ItemProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.ifElseBranchActivityCustom1.Condition = activityconditionha1;
            this.ifElseBranchActivityCustom1.Name = "ifElseBranchActivityCustom1";
            // 
            // ifElseActivityCustom1
            // 
            this.ifElseActivityCustom1.Activities.Add(this.ifElseBranchActivityCustom1);
            this.ifElseActivityCustom1.Activities.Add(this.ifElseBranchActivityCustom2);
            this.ifElseActivityCustom1.Name = "ifElseActivityCustom1";
            // 
            // goNext1
            // 
            activitybind2.Name = "WorkflowEmployment";
            activitybind2.Path = "RecievedEventDataEventsArgs";
            this.goNext1.Name = "goNext1";
            this.goNext1.SetBinding(HA.Workflow.Services.Controls.Navigation.GoNext.EProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // onPageToGoTo1
            // 
            this.onPageToGoTo1.Args = null;
            this.onPageToGoTo1.Name = "onPageToGoTo1";
            // 
            // eventDrivenActivity1
            // 
            this.eventDrivenActivity1.Activities.Add(this.goNext1);
            this.eventDrivenActivity1.Activities.Add(this.ifElseActivityCustom1);
            this.eventDrivenActivity1.Name = "eventDrivenActivity1";
            // 
            // stateInitializationActivity1
            // 
            this.stateInitializationActivity1.Activities.Add(this.onPageToGoTo1);
            this.stateInitializationActivity1.Name = "stateInitializationActivity1";
            // 
            // RegisterApplicant
            // 
            this.RegisterApplicant.Activities.Add(this.stateInitializationActivity1);
            this.RegisterApplicant.Activities.Add(this.eventDrivenActivity1);
            this.RegisterApplicant.Name = "RegisterApplicant";
            // 
            // Workflow1
            // 
            this.Activities.Add(this.RegisterApplicant);
            this.CompletedStateName = null;
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "RegisterApplicant";
            this.Name = "Workflow1";
            this.PageToGoToEventArgs = null;
            this.RecievedEventDataEventsArgs = null;
            this.CanModifyActivities = false;

        }

        #endregion

        private HA.Workflow.Services.Controls.Navigation.GoNext goNext1;
        private EventDrivenActivity eventDrivenActivity1;
        private StateInitializationActivity stateInitializationActivity1;
        private HA.Workflow.Services.Controls.Navigation.OnPageToGoTo onPageToGoTo1;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom2;
        private Workflow.Activities.IfElse.IfElseBranchActivityCustom ifElseBranchActivityCustom1;
        private Workflow.Activities.IfElse.IfElseActivityCustom ifElseActivityCustom1;
        private SetStateActivity setStateActivity1;
        private StateActivity RegisterApplicant;












    }
}
