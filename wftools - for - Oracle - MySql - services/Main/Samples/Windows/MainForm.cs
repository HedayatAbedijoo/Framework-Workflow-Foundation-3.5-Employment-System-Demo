using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;
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

        private delegate void TraceHandler();

        private void MainForm_Load(Object sender, EventArgs e)
        {
            // configure delegate trace listener
            DelegateTraceListener traceListener = new DelegateTraceListener();
            traceListener.WriteDelegate += traceListener_WriteDelegate;
            traceListener.WriteLineDelegate += traceListener_WriteDelegate;
            Trace.Listeners.Add(traceListener);

            // and the timer to flush messages to the message box
            traceDelegate = traceToMessageBox;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(Object sender, EventArgs e)
        {
            Invoke(traceDelegate);
        }

        private TraceHandler traceDelegate;
        private readonly Object traceMessagesLock = new Object();
        private readonly Stack<String> traceMessages = new Stack<String>();
        private Timer timer;

        private void traceListener_WriteDelegate(String message)
        {
            lock (traceMessagesLock)
            {
                traceMessages.Push(message);
            }
        }

        private void traceToMessageBox()
        {
            lock (traceMessagesLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (traceMessages.Count > 0)
                    stringBuilder.AppendFormat("{0}\r\n", traceMessages.Pop());

                stringBuilder.Append(txtTraceOutput.Text);
                // truncate text after 10000 characters, keeps performance good
                if (stringBuilder.Length > 10000)
                    stringBuilder.Remove(10000, stringBuilder.Length - 10000);

                txtTraceOutput.Text = stringBuilder.ToString();
            }
        }

        private static void workflowRuntime_WorkflowTerminated(Object sender, WorkflowTerminatedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("Workflow {0}:\r\n\r\n{1}",
                    e.WorkflowInstance.InstanceId, e.Exception));
            }
        }

        private static void workflowRuntime_ServicesExceptionNotHandled(Object sender, ServicesExceptionNotHandledEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("Workflow {0}:\r\n\r\n{1}",
                    e.WorkflowInstanceId, e.Exception));
            }
        }

        private void miCreateSequentialWorkflow_Click(object sender, EventArgs e)
        {
            // create a new sequential workflow
            WorkflowInstance workflowInstance = SampleWorkflowRuntime.Current.CreateSequentialWorkflow();

            if (_options.WorkflowSettings.ModifySequentialWorkflow)
            {
                Activity rootActivity = workflowInstance.GetWorkflowDefinition();
                WorkflowChanges workflowChanges = new WorkflowChanges(rootActivity);

                // modify the workflow
                Activity activityToRemove = workflowChanges.TransientWorkflow.GetActivityByName("codeActivity3");
                CompositeActivity parentActivity = activityToRemove.Parent;

                parentActivity.Activities.Remove(activityToRemove);

                CodeActivity codeActivity = new CodeActivity("TestChangeActivity");
                codeActivity.ExecuteCode +=
                    delegate { Trace.WriteLine("Test Change Activity executed..."); };

                parentActivity.Activities.Add(codeActivity);

                workflowInstance.ApplyWorkflowChanges(workflowChanges);
            }

            workflowInstance.Start();

            SampleWorkflowRuntime.Current.RunWorkflow(workflowInstance.InstanceId);
        }

        private void miCreateArticle_Click(object sender, EventArgs e)
        {
            new StateMachineWorkflowForm().ShowDialog();
        }

        /// <summary>
        /// Current options.
        /// </summary>
        private Options _options;

        private void btnOptions_Click(object sender, EventArgs e)
        {
            // display the options dialog
            OptionsDialog optionsDialog = new OptionsDialog(_options, SampleWorkflowRuntime.Current == null);

            if (optionsDialog.ShowDialog() == DialogResult.OK)
                _options = optionsDialog.Options;

            // enable or disable toolbar buttons based upon values
            if (SampleWorkflowRuntime.Current != null)
            {
                // disable the start button
                btnStartRuntime.Enabled = false;

                // enable the stop button
                btnStopRuntime.Enabled = true;
            }
            else
            {
                if (_options != null && _options.HasDatabaseSettings && _options.DatabaseSettings.HasPersistenceConnectionString)
                {
                    // we have valid database settings

                    // enable the start button, disable the stop button
                    btnStartRuntime.Enabled = true;
                    btnStopRuntime.Enabled = false;
                }
                else
                {
                    // we don't currently have valid database settings

                    // disable all buttons
                    btnStartRuntime.Enabled = false;
                    btnStopRuntime.Enabled = false;
                }
            }
        }

        private void btnStartRuntime_Click(object sender, EventArgs e)
        {
            SampleWorkflowRuntime.Start(_options.DatabaseSettings);
            SampleWorkflowRuntime.Current.WorkflowTerminated += workflowRuntime_WorkflowTerminated;
            SampleWorkflowRuntime.Current.ServicesExceptionNotHandled += workflowRuntime_ServicesExceptionNotHandled;

            btnCreate.Enabled = true;
            btnStartRuntime.Enabled = false;
            btnStopRuntime.Enabled = true;
        }

        private void btnStopRuntime_Click(object sender, EventArgs e)
        {
            SampleWorkflowRuntime.Stop();

            btnCreate.Enabled = false;
            btnStartRuntime.Enabled = true;
            btnStopRuntime.Enabled = false;
        }
    }
}