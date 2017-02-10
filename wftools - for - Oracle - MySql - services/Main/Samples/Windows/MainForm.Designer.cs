namespace WFTools.Samples.Windows
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblTraceOutput = new System.Windows.Forms.Label();
            this.txtTraceOutput = new System.Windows.Forms.TextBox();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.btnCreate = new System.Windows.Forms.ToolStripSplitButton();
            this.miCreateSequentialWorkflow = new System.Windows.Forms.ToolStripMenuItem();
            this.miCreateStateMachineWorkflow = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStartRuntime = new System.Windows.Forms.ToolStripButton();
            this.btnStopRuntime = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTraceOutput
            // 
            this.lblTraceOutput.AutoSize = true;
            this.lblTraceOutput.Location = new System.Drawing.Point(12, 37);
            this.lblTraceOutput.Name = "lblTraceOutput";
            this.lblTraceOutput.Size = new System.Drawing.Size(73, 13);
            this.lblTraceOutput.TabIndex = 2;
            this.lblTraceOutput.Text = "Trace Output:";
            // 
            // txtTraceOutput
            // 
            this.txtTraceOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTraceOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtTraceOutput.Location = new System.Drawing.Point(12, 53);
            this.txtTraceOutput.Multiline = true;
            this.txtTraceOutput.Name = "txtTraceOutput";
            this.txtTraceOutput.ReadOnly = true;
            this.txtTraceOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTraceOutput.Size = new System.Drawing.Size(527, 326);
            this.txtTraceOutput.TabIndex = 7;
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCreate,
            this.btnStartRuntime,
            this.btnStopRuntime,
            this.toolStripSeparator1,
            this.btnOptions});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(551, 25);
            this.toolStripMain.TabIndex = 11;
            this.toolStripMain.Text = "toolStripMain";
            // 
            // btnCreate
            // 
            this.btnCreate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCreate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCreateSequentialWorkflow,
            this.miCreateStateMachineWorkflow});
            this.btnCreate.Enabled = false;
            this.btnCreate.Image = ((System.Drawing.Image)(resources.GetObject("btnCreate.Image")));
            this.btnCreate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(32, 22);
            this.btnCreate.Text = "Create...";
            // 
            // miCreateSequentialWorkflow
            // 
            this.miCreateSequentialWorkflow.Name = "miCreateSequentialWorkflow";
            this.miCreateSequentialWorkflow.Size = new System.Drawing.Size(249, 22);
            this.miCreateSequentialWorkflow.Text = "Create Sequential Workflow...";
            this.miCreateSequentialWorkflow.Click += new System.EventHandler(this.miCreateSequentialWorkflow_Click);
            // 
            // miCreateStateMachineWorkflow
            // 
            this.miCreateStateMachineWorkflow.Name = "miCreateStateMachineWorkflow";
            this.miCreateStateMachineWorkflow.Size = new System.Drawing.Size(249, 22);
            this.miCreateStateMachineWorkflow.Text = "Create State Machine Workflow...";
            this.miCreateStateMachineWorkflow.Click += new System.EventHandler(this.miCreateArticle_Click);
            // 
            // btnStartRuntime
            // 
            this.btnStartRuntime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStartRuntime.Enabled = false;
            this.btnStartRuntime.Image = ((System.Drawing.Image)(resources.GetObject("btnStartRuntime.Image")));
            this.btnStartRuntime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStartRuntime.Name = "btnStartRuntime";
            this.btnStartRuntime.Size = new System.Drawing.Size(23, 22);
            this.btnStartRuntime.Text = "Start Runtime";
            this.btnStartRuntime.Click += new System.EventHandler(this.btnStartRuntime_Click);
            // 
            // btnStopRuntime
            // 
            this.btnStopRuntime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStopRuntime.Enabled = false;
            this.btnStopRuntime.Image = ((System.Drawing.Image)(resources.GetObject("btnStopRuntime.Image")));
            this.btnStopRuntime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStopRuntime.Name = "btnStopRuntime";
            this.btnStopRuntime.Size = new System.Drawing.Size(23, 22);
            this.btnStopRuntime.Text = "Stop Runtime";
            this.btnStopRuntime.Click += new System.EventHandler(this.btnStopRuntime_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOptions
            // 
            this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(23, 22);
            this.btnOptions.Text = "Options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 391);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.txtTraceOutput);
            this.Controls.Add(this.lblTraceOutput);
            this.Name = "MainForm";
            this.Text = "Windows WorkFlow Test";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTraceOutput;
        private System.Windows.Forms.TextBox txtTraceOutput;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripSplitButton btnCreate;
        private System.Windows.Forms.ToolStripMenuItem miCreateSequentialWorkflow;
        private System.Windows.Forms.ToolStripMenuItem miCreateStateMachineWorkflow;
        private System.Windows.Forms.ToolStripButton btnOptions;
        private System.Windows.Forms.ToolStripButton btnStartRuntime;
        private System.Windows.Forms.ToolStripButton btnStopRuntime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

