using System;
using System.Configuration;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WFTools.Services.Persistence.Ado;
using PersistenceException=WFTools.Services.Persistence.PersistenceException;

namespace WFTools.Tests
{
    /// <summary>
    /// Summary description for Persistence
    /// </summary>
    [TestClass]
    public class PersistenceTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private static WorkflowRuntime _workflowRuntime;

        [TestMethod]
        public void StartRuntime()
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Oracle - ODP.NET"];
            _workflowRuntime = new WorkflowRuntime();
            _workflowRuntime.AddService(new AdoPersistenceService(
                connectionString, true, TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)));

            Exception startException = null;
            _workflowRuntime.ServicesExceptionNotHandled +=
                delegate(Object sender, ServicesExceptionNotHandledEventArgs e)
                    {
                        startException = e.Exception;
                    };

            _workflowRuntime.StartRuntime();

            if (startException != null)
                throw startException;
        }

        [TestMethod]
        public void StopRuntime()
        {
            _workflowRuntime.StopRuntime();
            _workflowRuntime = null;
        }

        [TestMethod]
        [ExpectedException(typeof(PersistenceException))]
        public void RetrieveInvalidInstance()
        {
            _workflowRuntime.GetWorkflow(Guid.NewGuid());
        }
    }
}
