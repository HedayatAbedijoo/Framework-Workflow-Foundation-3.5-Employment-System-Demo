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
            this.btnCreateSequentialWorkflow = new System.Windows.Forms.Button();
            this.lstTraceOutput = new System.Windows.Forms.ListBox();
            this.lblTraceOutput = new System.Windows.Forms.Label();
            this.cboConnectionString = new System.Windows.Forms.ComboBox();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreateSequentialWorkflow
            // 
            this.btnCreateSequentialWorkflow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateSequentialWorkflow.Location = new System.Drawing.Point(370, 3);
            this.btnCreateSequentialWorkflow.Name = "btnCreateSequentialWorkflow";
            this.btnCreateSequentialWorkflow.Size = new System.Drawing.Size(169, 23);
            this.btnCreateSequentialWorkflow.TabIndex = 0;
            this.btnCreateSequentialWorkflow.Text = "Create Sequential WorkFlow";
            this.btnCreateSequentialWorkflow.UseVisualStyleBackColor = true;
            this.btnCreateSequentialWorkflow.Click += new System.EventHandler(this.btnCreateSequentialWorkflow_Click);
            // 
            // lstTraceOutput
            // 
            this.lstTraceOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTraceOutput.FormattingEnabled = true;
            this.lstTraceOutput.HorizontalScrollbar = true;
            this.lstTraceOutput.Location = new System.Drawing.Point(12, 82);
            this.lstTraceOutput.Name = "lstTraceOutput";
            this.lstTraceOutput.Size = new System.Drawing.Size(527, 251);
            this.lstTraceOutput.TabIndex = 1;
            // 
            // lblTraceOutput
            // 
            this.lblTraceOutput.AutoSize = true;
            this.lblTraceOutput.Location = new System.Drawing.Point(12, 66);
            this.lblTraceOutput.Name = "lblTraceOutput";
            this.lblTraceOutput.Size = new System.Drawing.Size(73, 13);
            this.lblTraceOutput.TabIndex = 2;
            this.lblTraceOutput.Text = "Trace Output:";
            // 
            // cboConnectionString
            // 
            this.cboConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConnectionString.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnectionString.FormattingEnabled = true;
            this.cboConnectionString.Location = new System.Drawing.Point(112, 5);
            this.cboConnectionString.Name = "cboConnectionString";
            this.cboConnectionString.Size = new System.Drawing.Size(252, 21);
            this.cboConnectionString.TabIndex = 3;
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(12, 8);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(94, 13);
            this.lblConnectionString.TabIndex = 4;
            this.lblConnectionString.Text = "Connection String:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 345);
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.cboConnectionString);
            this.Controls.Add(this.lblTraceOutput);
            this.Controls.Add(this.lstTraceOutput);
            this.Controls.Add(this.btnCreateSequentialWorkflow);
            this.Name = "MainForm";
            this.Text = "Windows WorkFlow Test";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateSequentialWorkflow;
        private System.Windows.Forms.ListBox lstTraceOutput;
        private System.Windows.Forms.Label lblTraceOutput;
        private System.Windows.Forms.ComboBox cboConnectionString;
        private System.Windows.Forms.Label lblConnectionString;
    }
}

