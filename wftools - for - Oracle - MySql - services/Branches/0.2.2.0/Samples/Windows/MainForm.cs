using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using WFTools.Samples.WorkFlow;
using WFTools.Utilities.Diagnostics;

namespace WFTools.Samples.Windows
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private delegate void TraceHandler(string message);

        private TraceHandler traceDelegate;

        private void MainForm_Load(object sender, EventArgs e)
        {
            // bind connection strings to the dropdown list
            cboConnectionString.DisplayMember = "Name";
            foreach (ConnectionStringSettings connectionStringSetting in ConfigurationManager.ConnectionStrings)
                cboConnectionString.Items.Add(connectionStringSetting);

            if (cboConnectionString.Items.Count > 0)
                cboConnectionString.SelectedIndex = 0;

            // configure delegate trace listener
            DelegateTraceListener traceListener = new DelegateTraceListener();
            traceDelegate = new TraceHandler(traceToMessageBox);
            traceListener.WriteDelegate += new DelegateTraceListener.WriteHandler(traceListener_WriteDelegate);
            traceListener.WriteLineDelegate += new DelegateTraceListener.WriteHandler(traceListener_WriteLineDelegate);
            Trace.Listeners.Add(traceListener);
        }

        private void traceListener_WriteLineDelegate(string message)
        {
            Invoke(traceDelegate, new object[] { message });
        }

        private void traceListener_WriteDelegate(string message)
        {
            Invoke(traceDelegate, new object[] { message });
        }

        private void traceToMessageBox(string message)
        {
            lstTraceOutput.Items.Insert(0, message);
        }

        private Dictionary<string, SampleWorkFlowRuntime> loadedWorkflowRuntimes = new Dictionary<string, SampleWorkFlowRuntime>();

        private void btnCreateSequentialWorkflow_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings connectionStringSettings = cboConnectionString.SelectedItem as ConnectionStringSettings;
            if (connectionStringSettings == null)
            {
                MessageBox.Show("No connection string selected.", "WFTools Samples",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            SampleWorkFlowRuntime workflowRuntime;
            if (this.loadedWorkflowRuntimes.ContainsKey(connectionStringSettings.Name))
                workflowRuntime = this.loadedWorkflowRuntimes[connectionStringSettings.Name];
            else
            {
                workflowRuntime = new SampleWorkFlowRuntime(connectionStringSettings);
                workflowRuntime.WorkflowTerminated += new EventHandler<WorkflowTerminatedEventArgs>(workflowRuntime_WorkflowTerminated);
                workflowRuntime.ServicesExceptionNotHandled += new EventHandler<ServicesExceptionNotHandledEventArgs>(workflowRuntime_ServicesExceptionNotHandled);
                this.loadedWorkflowRuntimes.Add(connectionStringSettings.Name, workflowRuntime);
            }

            // create a new sequential workflow
            WorkflowInstance workflowInstance = workflowRuntime.CreateSequentialWorkflow();
            workflowInstance.Start();

            ManualWorkflowSchedulerService schedulerService = workflowRuntime.GetService<ManualWorkflowSchedulerService>();
            schedulerService.RunWorkflow(workflowInstance.InstanceId);
        }

        private void workflowRuntime_WorkflowTerminated(object sender, WorkflowTerminatedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(string.Format("Workflow {0}:\r\n\r\n{1}",
                    e.WorkflowInstance.InstanceId, e.Exception));
            }
        }

        private void workflowRuntime_ServicesExceptionNotHandled(object sender, ServicesExceptionNotHandledEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(string.Format("Workflow {0}:\r\n\r\n{1}",
                    e.WorkflowInstanceId, e.Exception));
            }
        }
    }
}