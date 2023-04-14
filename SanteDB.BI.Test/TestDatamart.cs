using NUnit.Framework;
using SanteDB.Core.Configuration;
using SanteDB.Core.Security;
using SanteDB.Core.Services;
using SanteDB.Core.TestFramework;
using SanteDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanteDB.BI.Services;
using SanteDB.BI.Model;
using System.CodeDom;
using SanteDB.BI.Datamart;
using System.Threading;
using DocumentFormat.OpenXml.Office2016.Excel;
using System.IO;
using SanteDB.BI.Util;
using SanteDB.BI.Exceptions;

namespace SanteDB.BI.Test
{
    [TestFixture]
    public class TestDatamart
    {
        public const string CORE_MART_ID = "org.santedb.bi.datamart.core";

        /// <summary>
        /// Delete the warehouse file
        /// </summary>
        private void DeleteWarehouseFile(String name)
        {
            var fp = Path.Combine(Path.GetDirectoryName(TestApplicationContext.TestAssembly.Location), $"{name}.sqlite");
            if (File.Exists(fp))
            {
                File.Delete(fp);
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            TestApplicationContext.TestAssembly = typeof(TestDatamart).Assembly;
            TestApplicationContext.Initialize(TestContext.CurrentContext.TestDirectory);

        }

        [Test]
        public void TestDoesHaveDatamarts()
        {
            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            Assert.AreEqual(1, repositoryService.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).Count());
        }

        [Test]
        public void TestDoesValidate()
        {
            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = repositoryService.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).FirstOrDefault();
            Assert.IsNotNull(coreMart);
            var errors = coreMart.Validate().ToList();
            Assert.IsFalse(errors.Any(o => o.Priority == Core.BusinessRules.DetectedIssuePriorityType.Error), "Core datamart file does not validate");
        }

        [Test]
        public void TestCanManageMetadata()
        {
            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = repositoryService.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).FirstOrDefault();
            Assert.IsNotNull(coreMart);
            var manager = ApplicationServiceContext.Current.GetService<IBiDatamartRepository>();
            Assert.IsNotNull(manager);
            using (AuthenticationContext.EnterSystemContext())
            {
                coreMart.Id = "example.test";
                var registered = manager.Register(coreMart);
                Assert.IsNotNull(registered.Key);
                Assert.AreEqual(coreMart.Id, registered.Id);
                Assert.AreEqual(coreMart.Name, registered.Name);
                Assert.AreEqual(coreMart.MetaData.Annotation.JsonBody, registered.Description);
                Assert.AreEqual(coreMart.MetaData.Version, registered.Version);

                // Validate we can query back out
                Assert.AreEqual(1, manager.Find(o => o.Id == coreMart.Id).Count());
                Assert.IsTrue(manager.Find(o => o.Id == coreMart.Id).Any());

                // Validate the data is accurate
                var queried = manager.Find(o => o.Id == coreMart.Id).First();
                Assert.AreEqual(registered.Key, queried.Key);

                Thread.Sleep(1000); // mimic some time to pass between updates

                // Update
                coreMart.MetaData.Version = "1.0";
                var updated = manager.Register(coreMart);
                Assert.AreEqual("1.0", updated.Version);
                Assert.AreEqual(1, manager.Find(o => o.Id == coreMart.Id).Count());

                // Un-Register
                manager.Unregister(updated);
                Assert.AreEqual(0, manager.Find(o => o.Id == coreMart.Id).Count());
                Assert.AreEqual(1, manager.Find(o => o.Id == coreMart.Id && o.ObsoletionTime != null).Count());

                // Can re-register again and the correct version is provided
                var reregistered = manager.Register(coreMart);
                Assert.AreNotEqual(reregistered.Key, registered.Key);
                Assert.AreEqual(1, manager.Find(o => o.Id == coreMart.Id).Count());
                Assert.AreEqual(1, manager.Find(o => o.Id == coreMart.Id && o.ObsoletionTime != null).Count());
                Assert.AreEqual(2, manager.Find(o => o.Id == coreMart.Id && o.ObsoletionTime == null || o.ObsoletionTime != null).Count());
                coreMart.Id = CORE_MART_ID;

            }
        }

        /// <summary>
        /// Test that we can create an execution context and a schema
        /// </summary>
        [Test]
        public void TestCanCreateExecutionContext()
        {
            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = repositoryService.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).FirstOrDefault();
            Assert.IsNotNull(coreMart);
            var manager = ApplicationServiceContext.Current.GetService<IBiDatamartRepository>();
            Assert.IsNotNull(manager);
            using (AuthenticationContext.EnterSystemContext())
            {
                var datamart = manager.Find(o => o.Id == coreMart.Id).FirstOrDefault();
                if (datamart == null)
                {
                    datamart = manager.Register(coreMart);
                }

                var cExecutions = datamart.Executions.Count();
                using (var executionContext = manager.GetExecutionContext(datamart, BiExecutionPurposeType.Discovery))
                {
                    Assert.AreEqual(executionContext.Datamart.Key, datamart.Key);
                    executionContext.SetOutcome(BiExecutionOutcomeType.Success);
                }
                Assert.AreEqual(cExecutions + 1, datamart.Executions.Count());
            }
        }

        /// <summary>
        /// Test that the data context integrator works
        /// </summary>
        [Test]
        public void TestCanInteractWithIntegrator()
        {

            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = repositoryService.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).FirstOrDefault();
            Assert.IsNotNull(coreMart);
            this.DeleteWarehouseFile("TestCanInteractWithIntegrator");
            coreMart.Produces.Name = "TestCanInteractWithIntegrator";
            var manager = ApplicationServiceContext.Current.GetService<IBiDatamartRepository>();
            Assert.IsNotNull(manager);
            using (AuthenticationContext.EnterSystemContext())
            {
                var datamart = manager.Find(o => o.Id == coreMart.Id).FirstOrDefault();
                if (datamart == null)
                {
                    datamart = manager.Register(coreMart);
                }

                using (var executionContext = manager.GetExecutionContext(datamart, BiExecutionPurposeType.DatabaseManagement | BiExecutionPurposeType.SchemaManagement))
                {
                    Assert.AreEqual(executionContext.Datamart.Key, datamart.Key);

                    var outputIntegrator = executionContext.GetIntegrator(coreMart.Produces);
                    Assert.IsNotNull(outputIntegrator);
                    Assert.IsFalse(outputIntegrator.DatabaseExists());

                    outputIntegrator.CreateDatabase();
                    Assert.IsTrue(outputIntegrator.DatabaseExists());

                    // Throw an exception because the connection isn't open
                    var noopSql = new BiSqlDefinition()
                    {
                        Invariants = new List<string>() { "sqlite" },
                        Sql = "SELECT 1"
                    };
                    Assert.Throws<InvalidOperationException>(() => outputIntegrator.ExecuteNonQuery(noopSql));

                    // Don't allow duplicate calls to open!
                    outputIntegrator.OpenRead();
                    Assert.Throws<InvalidOperationException>(() => outputIntegrator.CreateDatabase());
                    Assert.Throws<InvalidOperationException>(() => outputIntegrator.OpenWrite());
                    Assert.DoesNotThrow(() => outputIntegrator.ExecuteNonQuery(noopSql));
                    Assert.IsFalse(outputIntegrator.Exists(coreMart.SchemaObjects.First()));
                    outputIntegrator.Close();

                    // Open Write
                    outputIntegrator.OpenWrite();
                    Assert.IsTrue(outputIntegrator.NeedsMigration(coreMart.SchemaObjects.First()));
                    coreMart.SchemaObjects.ForEach(o => outputIntegrator.RecreateObject(o));
                    Assert.IsFalse(coreMart.SchemaObjects.Any(o => outputIntegrator.NeedsMigration(o)));
                    // Update the schema to add a new table and ensure we can migrate
                    coreMart.SchemaObjects.Add(new BiSchemaTableDefinition()
                    {
                        Name = "FOO_TBL",
                        Columns = new List<BiSchemaColumnDefinition>()
                    {
                        new BiSchemaColumnDefinition()
                        {
                            Name = "KEY",
                            IsKey = true,
                            Type = BiDataType.Uuid
                        },
                        new BiSchemaColumnDefinition()
                        {
                            Name = "VAL",
                            Type = BiDataType.String
                        }
                    }
                    });
                    Assert.IsTrue(coreMart.SchemaObjects.Any(o => outputIntegrator.NeedsMigration(o)));

                    // Ensure we can re-create the tables 
                    coreMart.SchemaObjects.ForEach(o => outputIntegrator.RecreateObject(o));

                    // Add a column to a table and ensure we re-create the tables
                    coreMart.SchemaObjects.First().Columns.Add(new BiSchemaColumnDefinition() { Name = "TEST", Type = BiDataType.Boolean });
                    Assert.IsTrue(coreMart.SchemaObjects.Any(o => outputIntegrator.NeedsMigration(o)));
                    coreMart.SchemaObjects.ForEach(o => outputIntegrator.RecreateObject(o));

                    outputIntegrator.Close();

                    executionContext.SetOutcome(BiExecutionOutcomeType.Success);
                }
            }
        }

        /// <summary>
        /// Test that a flow can be executed
        /// </summary>
        [Test]
        public void TestBiIntegrationManagement()
        {
            var metaRepository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = metaRepository.Query<BiDatamartDefinition>(o => o.Id == CORE_MART_ID).FirstOrDefault();
            Assert.IsNotNull(coreMart);

            this.DeleteWarehouseFile("TestBiIntegrationManagement");
            coreMart.Produces.Name = "TestBiIntegrationManagement";

            var repository = ApplicationServiceContext.Current.GetService<IBiDatamartRepository>();
            Assert.IsNotNull(repository);
            var manager = ApplicationServiceContext.Current.GetService<IBiDatamartManager>();
            Assert.IsNotNull(repository);

            // Construct a simple query to validate our warehouse and integration
            var queryDefinition = new BiQueryDefinition()
            {
                Id = "org.example.test.warehouse",
                DataSources = new List<BiDataSourceDefinition>()
                {
                    new BiDataSourceDefinition()
                    {
                        Ref = "#org.santedb.bi.dataSource.warehouse"
                    }
                },
                Name = "Example Test",
                QueryDefinitions = new List<BiSqlDefinition>()
                {
                    new BiSqlDefinition()
                    {
                        Invariants = new List<string>() { "sqlite" },
                        Sql = "SELECT * FROM VW_PAT_TBL"
                    }
                }
            };

            using (AuthenticationContext.EnterSystemContext())
            {
                // Resolve the query definition and try to run should throw
                try
                {
                    BiUtils.ResolveRefs(queryDefinition);
                    Assert.Fail();
                }
                catch
                {
                }

                // Manager should not allow us to create or refresh until the mart is registered
                Assert.Throws<BiException>(() => manager.Migrate(coreMart));

                // We should be able to register and then resolve refs
                repository.Register(coreMart);
                manager.Migrate(coreMart);
                var qdef = BiUtils.ResolveRefs(queryDefinition); // this should work now
                Assert.AreEqual(coreMart.Produces.Id, qdef.DataSources.First().Id);

                // Test we can actually run the query
                
                var dataSourceProvider = ApplicationServiceContext.Current.GetService<IServiceManager>().CreateInjected(qdef.DataSources.First().ProviderType) as IBiDataSource;
                var queryResult = dataSourceProvider.ExecuteQuery(qdef, new Dictionary<String, Object>(), null);
                Assert.AreEqual(0, queryResult.Records.Count());
            }
        }
    }
}
