using System;
using System.Diagnostics;
using System.Workflow.Activities;

namespace WFTools.Samples.WorkFlow
{
	public partial class SequentialWorkFlow : SequentialWorkflowActivity
	{
	    private int loopCounter = 0;

        private void codeActivity1_ExecuteCode(object sender, EventArgs e)
        {
            Trace.WriteLine("Code Activity 1 executed...");
        }

        private void codeActivity2_ExecuteCode(object sender, EventArgs e)
        {
            Trace.WriteLine("Loop Activity executed...");

            loopCounter++;
        }

        private void codeActivity3_ExecuteCode(object sender, EventArgs e)
        {
            Trace.WriteLine("Code Activity 3 executed...");
        }
    }
}
