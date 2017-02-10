using System;
using System.Windows.Forms;
using System.Workflow.Runtime;
using WFTools.Samples.WorkFlow;
using WFTools.Samples.WorkFlow.Services;

namespace WFTools.Samples.Windows
{
    public partial class StateMachineWorkflowForm : Form
    {
        public StateMachineWorkflowForm()
        {
            InitializeComponent();
        }

        private WorkflowInstance _workflowInstance;

        private void btnComplete_Click(object sender, EventArgs e)
        {
            try
            {
                btnComplete.Enabled = false;
                StateMachineService stateMachineService = SampleWorkflowRuntime.Current.GetService<StateMachineService>();
                stateMachineService.OnCompleted(_workflowInstance.InstanceId);
                SampleWorkflowRuntime.Current.RunWorkflow(_workflowInstance.InstanceId);
                btnCreate.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: {0}", ex.ToString());

                btnCreate.Enabled = false;
                btnComplete.Enabled = true;
            }


            
            btnComplete.Enabled = false;
            btnCreate.Enabled = true;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            _workflowInstance = SampleWorkflowRuntime.Current.CreateStateMachineWorkflow();
            _workflowInstance.Start();

            try
            {
                btnCreate.Enabled = false;
                SampleWorkflowRuntime.Current.RunWorkflow(_workflowInstance.InstanceId);
                btnComplete.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: {0}", ex.ToString());

                _workflowInstance = null;
                btnCreate.Enabled = true;
                btnComplete.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}