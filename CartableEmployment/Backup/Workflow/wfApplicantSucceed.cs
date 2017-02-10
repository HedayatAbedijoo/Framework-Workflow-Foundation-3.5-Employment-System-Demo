﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using HA.Workflow.Services.BaseClasses;

namespace Workflow
{
    public sealed partial class wfApplicantSucceed : StateMachineWorkflowBase
    {
        public wfApplicantSucceed()
        {
            InitializeComponent();
        }

        public override void InitializeOutgoingMessage(object sender, EventArgs e)
        {
            base.InitializeOutgoingMessage(sender, e);
        }

        public override HA.Workflow.Services.Interfaces.NavigationFlowEventArgs PageToGoToEventArgs
        {
            get
            {
                return base.PageToGoToEventArgs;
            }
            set
            {
                base.PageToGoToEventArgs = value;
            }
        }

        public override HA.Workflow.Services.Interfaces.StateBehaveArgs RecievedEventDataEventsArgs
        {
            get
            {
                return base.RecievedEventDataEventsArgs;
            }
            set
            {
                base.RecievedEventDataEventsArgs = value;
            }
        }
    }
}
