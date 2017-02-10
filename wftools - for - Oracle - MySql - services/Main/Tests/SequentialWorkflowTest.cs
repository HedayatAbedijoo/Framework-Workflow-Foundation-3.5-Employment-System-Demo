using System.Configuration;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WFTools.Samples.Windows;
using WFTools.Samples.WorkFlow;

namespace WFTools.Tests
{
    /// <summary>
    /// Creates a sequential workflow.
    /// </summary>
    [TestClass]
    public class SequentialWorkflowTest
    {
        [ClassInitialize]
        public static void Initialise(TestContext testContext)
        {
            SampleWorkflowRuntime.Start(buildDatabaseSettings());
        }

        private static DatabaseSettings buildDatabaseSettings()
        {
            DatabaseSettings databaseSettings = new DatabaseSettings();
            databaseSettings.PersistenceConnectionString = ConfigurationManager.ConnectionStrings["MySQL"];
            return databaseSettings;
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SampleWorkflowRuntime.Stop();
        }

        [TestMethod]
        public void CreateSequentialWorkflow()
        {
            WorkflowInstance workflowInstance = SampleWorkflowRuntime.Current.CreateSequentialWorkflow();
            workflowInstance.Start();

            SampleWorkflowRuntime.Current.RunWorkflow(workflowInstance.InstanceId);
        }
    }
}
