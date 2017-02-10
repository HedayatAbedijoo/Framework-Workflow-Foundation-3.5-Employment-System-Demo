namespace WFTools.Samples.Windows
{
    partial class OptionsDialog
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
            this.cboTrackingService = new System.Windows.Forms.ComboBox();
            this.cboPersistenceService = new System.Windows.Forms.ComboBox();
            this.gbDatabaseSettings = new System.Windows.Forms.GroupBox();
            this.chkUseLocalTransactions = new System.Windows.Forms.CheckBox();
            this.lblTrackingService = new System.Windows.Forms.Label();
            this.lblPersistenceService = new System.Windows.Forms.Label();
            this.gbWorkflowOptions = new System.Windows.Forms.GroupBox();
            this.chkModifySequentialWorkflow = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbDatabaseSettings.SuspendLayout();
            this.gbWorkflowOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboTrackingService
            // 
            this.cboTrackingService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTrackingService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTrackingService.FormattingEnabled = true;
            this.cboTrackingService.Location = new System.Drawing.Point(116, 46);
            this.cboTrackingService.Name = "cboTrackingService";
            this.cboTrackingService.Size = new System.Drawing.Size(316, 21);
            this.cboTrackingService.TabIndex = 0;
            // 
            // cboPersistenceService
            // 
            this.cboPersistenceService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPersistenceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPersistenceService.FormattingEnabled = true;
            this.cboPersistenceService.Location = new System.Drawing.Point(116, 19);
            this.cboPersistenceService.Name = "cboPersistenceService";
            this.cboPersistenceService.Size = new System.Drawing.Size(316, 21);
            this.cboPersistenceService.TabIndex = 1;
            // 
            // gbDatabaseSettings
            // 
            this.gbDatabaseSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDatabaseSettings.Controls.Add(this.chkUseLocalTransactions);
            this.gbDatabaseSettings.Controls.Add(this.lblTrackingService);
            this.gbDatabaseSettings.Controls.Add(this.cboTrackingService);
            this.gbDatabaseSettings.Controls.Add(this.lblPersistenceService);
            this.gbDatabaseSettings.Controls.Add(this.cboPersistenceService);
            this.gbDatabaseSettings.Location = new System.Drawing.Point(12, 12);
            this.gbDatabaseSettings.Name = "gbDatabaseSettings";
            this.gbDatabaseSettings.Size = new System.Drawing.Size(438, 105);
            this.gbDatabaseSettings.TabIndex = 4;
            this.gbDatabaseSettings.TabStop = false;
            this.gbDatabaseSettings.Text = "&Database";
            // 
            // chkUseLocalTransactions
            // 
            this.chkUseLocalTransactions.AutoSize = true;
            this.chkUseLocalTransactions.Location = new System.Drawing.Point(116, 73);
            this.chkUseLocalTransactions.Name = "chkUseLocalTransactions";
            this.chkUseLocalTransactions.Size = new System.Drawing.Size(138, 17);
            this.chkUseLocalTransactions.TabIndex = 5;
            this.chkUseLocalTransactions.Text = "Use &Local Transactions";
            this.chkUseLocalTransactions.UseVisualStyleBackColor = true;
            // 
            // lblTrackingService
            // 
            this.lblTrackingService.AutoSize = true;
            this.lblTrackingService.Location = new System.Drawing.Point(6, 49);
            this.lblTrackingService.Name = "lblTrackingService";
            this.lblTrackingService.Size = new System.Drawing.Size(91, 13);
            this.lblTrackingService.TabIndex = 4;
            this.lblTrackingService.Text = "&Tracking Service:";
            // 
            // lblPersistenceService
            // 
            this.lblPersistenceService.AutoSize = true;
            this.lblPersistenceService.Location = new System.Drawing.Point(6, 22);
            this.lblPersistenceService.Name = "lblPersistenceService";
            this.lblPersistenceService.Size = new System.Drawing.Size(104, 13);
            this.lblPersistenceService.TabIndex = 3;
            this.lblPersistenceService.Text = "&Persistence Service:";
            // 
            // gbWorkflowOptions
            // 
            this.gbWorkflowOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkflowOptions.Controls.Add(this.chkModifySequentialWorkflow);
            this.gbWorkflowOptions.Location = new System.Drawing.Point(12, 123);
            this.gbWorkflowOptions.Name = "gbWorkflowOptions";
            this.gbWorkflowOptions.Size = new System.Drawing.Size(438, 51);
            this.gbWorkflowOptions.TabIndex = 5;
            this.gbWorkflowOptions.TabStop = false;
            this.gbWorkflowOptions.Text = "&Workflow";
            // 
            // chkModifySequentialWorkflow
            // 
            this.chkModifySequentialWorkflow.AutoSize = true;
            this.chkModifySequentialWorkflow.Location = new System.Drawing.Point(9, 20);
            this.chkModifySequentialWorkflow.Name = "chkModifySequentialWorkflow";
            this.chkModifySequentialWorkflow.Size = new System.Drawing.Size(215, 17);
            this.chkModifySequentialWorkflow.TabIndex = 0;
            this.chkModifySequentialWorkflow.Text = "&Modify Sequential Workflow on Creation";
            this.chkModifySequentialWorkflow.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(375, 188);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(294, 188);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(462, 223);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbWorkflowOptions);
            this.Controls.Add(this.gbDatabaseSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.Text = "Options...";
            this.Load += new System.EventHandler(this.OptionsDialog_Load);
            this.gbDatabaseSettings.ResumeLayout(false);
            this.gbDatabaseSettings.PerformLayout();
            this.gbWorkflowOptions.ResumeLayout(false);
            this.gbWorkflowOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTrackingService;
        private System.Windows.Forms.ComboBox cboPersistenceService;
        private System.Windows.Forms.GroupBox gbDatabaseSettings;
        private System.Windows.Forms.Label lblTrackingService;
        private System.Windows.Forms.Label lblPersistenceService;
        private System.Windows.Forms.CheckBox chkUseLocalTransactions;
        private System.Windows.Forms.GroupBox gbWorkflowOptions;
        private System.Windows.Forms.CheckBox chkModifySequentialWorkflow;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}