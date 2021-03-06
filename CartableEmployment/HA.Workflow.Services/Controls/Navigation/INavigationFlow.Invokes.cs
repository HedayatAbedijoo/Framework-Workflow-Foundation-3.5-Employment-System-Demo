//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HA.Workflow.Services.Controls.Navigation
{
    using System;
    using System.ComponentModel;
    using System.Workflow.Activities;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Design;
    using System.Workflow.ComponentModel.Compiler;
    using HA.Workflow.Services.Interfaces;


    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class OnPageToGoTo : CallExternalMethodActivity
    {

        public static DependencyProperty ArgsProperty = DependencyProperty.Register("Args", typeof(NavigationFlowEventArgs), typeof(OnPageToGoTo));
        HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfoCollection eventInfoCollection = new HA.Workflow.Services.BaseClasses.StateEventsWaiting.EventInfoCollection();
        public OnPageToGoTo()
        {
            base.InterfaceType = typeof(INavigationFlow);
            base.MethodName = "OnPageToGoTo";
        }

        [BrowsableAttribute(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public override System.Type InterfaceType
        {
            get
            {
                return base.InterfaceType;
            }
            set
            {
                throw new InvalidOperationException("Cannot set InterfaceType on a derived CallExternalMethodActivity.");
            }
        }

        [BrowsableAttribute(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public override string MethodName
        {
            get
            {
                return base.MethodName;
            }
            set
            {
                throw new InvalidOperationException("Cannot set MethodName on a derived CallExternalMethodActivity.");
            }
        }

        [ValidationOptionAttribute(ValidationOption.Required)]
        public NavigationFlowEventArgs Args
        {
            get
            {
                return ((NavigationFlowEventArgs)(this.GetValue(OnPageToGoTo.ArgsProperty)));
            }
            set
            {
                this.SetValue(OnPageToGoTo.ArgsProperty, value);
            }
        }

        protected override void OnMethodInvoking(System.EventArgs e)
        {
            this.ParameterBindings["Args"].Value = this.Args;
        }


    }

}
