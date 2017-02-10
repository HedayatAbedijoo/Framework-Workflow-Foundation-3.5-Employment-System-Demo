using System;
using System.Configuration;
using System.Windows.Forms;

namespace WFTools.Samples.Windows
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog(Options options, Boolean enableDatabaseSettings)
        {
            this.options = options;
            this.enableDatabaseSettings = enableDatabaseSettings;

            InitializeComponent();
        }

        private readonly Boolean enableDatabaseSettings;

        private void btnOK_Click(object sender, EventArgs e)
        {
            // re-populate options
            this.options = new Options();

            // database settings
            this.options.DatabaseSettings = new DatabaseSettings();
            if (cboPersistenceService.SelectedItem != null)
                this.options.DatabaseSettings.PersistenceConnectionString = cboPersistenceService.SelectedItem as ConnectionStringSettings;
            if (cboTrackingService.SelectedItem != null)
                this.options.DatabaseSettings.TrackingConnectionString = cboTrackingService.SelectedItem as ConnectionStringSettings;
            this.options.DatabaseSettings.UseLocalTransactions = chkUseLocalTransactions.Checked;

            this.options.WorkflowSettings = new WorkflowSettings();
            this.options.WorkflowSettings.ModifySequentialWorkflow = chkModifySequentialWorkflow.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private Options options;
        /// <summary>
        /// Gets the <see cref="Options" /> specified within this dialog.
        /// </summary>
        public Options Options
        {
            get { return options; }
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {
            cboPersistenceService.DisplayMember = "Name";
            cboTrackingService.DisplayMember = "Name";

            // bind connection strings to dropdown lists
            foreach (ConnectionStringSettings connectionStringSetting in ConfigurationManager.ConnectionStrings)
            {
                cboPersistenceService.Items.Add(connectionStringSetting);
                cboTrackingService.Items.Add(connectionStringSetting);
            }

            cboTrackingService.Items.Insert(0, "None");

            if (cboPersistenceService.Items.Count > 0)
                cboPersistenceService.SelectedIndex = 0;

            if (cboTrackingService.Items.Count > 0)
                cboTrackingService.SelectedIndex = 0;

            // populate the form with selected data
            if (this.options != null)
            {
                if (this.options.HasDatabaseSettings)
                {
                    if (this.options.DatabaseSettings.HasPersistenceConnectionString)
                        cboPersistenceService.SelectedItem = this.options.DatabaseSettings.PersistenceConnectionString;

                    if (this.options.DatabaseSettings.HasTrackingConnectionString)
                        cboPersistenceService.SelectedItem = this.options.DatabaseSettings.TrackingConnectionString;

                    chkUseLocalTransactions.Checked = this.options.DatabaseSettings.UseLocalTransactions;
                }

                if (this.options.HasWorkflowSettings)
                    chkModifySequentialWorkflow.Checked = this.options.WorkflowSettings.ModifySequentialWorkflow;
            }

            this.gbDatabaseSettings.Enabled = this.enableDatabaseSettings;
        }
    }
}