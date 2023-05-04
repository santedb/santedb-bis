using RestSrvr;
using SanteDB.Core.Security.Audit;
using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.i18n;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using SanteDB.Core.Model.Audit;
using DocumentFormat.OpenXml.InkML;
using SanteDB.Core.Exceptions;
using SanteDB.BI.Util;
using DocumentFormat.OpenXml.Presentation;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// Default datamart manager
    /// </summary>
    public class DefaultDatamartManager : IBiDatamartManager
    {
        private readonly IBiDatamartRepository m_datamartRegistry;
        private readonly ILocalizationService m_localization;
        private readonly IBiMetadataRepository m_metadataRepository;
        private readonly IPolicyEnforcementService m_pepService;
        private readonly IAuditService m_auditService;
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(DefaultDatamartManager));

        /// <summary>
        /// DI constructor
        /// </summary>
        public DefaultDatamartManager(IBiDatamartRepository datamartRegistry, ILocalizationService localizationService, IBiMetadataRepository metadataRepository, IPolicyEnforcementService pepService, IAuditService auditService)
        {
            this.m_datamartRegistry = datamartRegistry;
            this.m_localization = localizationService;
            this.m_metadataRepository = metadataRepository;
            this.m_pepService = pepService;
            this.m_auditService = auditService;
        }

        /// <inheritdoc/>
        public string ServiceName => "Default Datamart Manager";

        /// <summary>
        /// Diagnostic event received
        /// </summary>
        public event EventHandler DiagnosticEventReceived;


        /// <summary>
        /// Get a registered datamart from the registry
        /// </summary>
        private IDataFlowExecutionContext GetDataFlowExecutionContext(BiDatamartDefinition datamartDefinition, DataFlowExecutionPurposeType purpose)
        {

            if (datamartDefinition == null)
            {
                throw new ArgumentNullException(nameof(datamartDefinition));
            }

            var validateIssues = datamartDefinition.Validate();
            if (validateIssues.Any(i => i.Priority == Core.BusinessRules.DetectedIssuePriorityType.Error))
            {
                throw new DetectedIssueException(validateIssues);
            }

            // Get the registration entry
            var datamart = this.m_datamartRegistry.Find(o => o.Id == datamartDefinition.Id).FirstOrDefault();
            if (datamart == null)
            {
                throw new KeyNotFoundException(String.Format(ErrorMessages.DEPENDENT_CONFIGURATION_MISSING, datamartDefinition.Id));
            }
            return this.m_datamartRegistry.GetExecutionContext(datamart, purpose);

        }

        /// <inheritdoc/>
        public void Destroy(BiDatamartDefinition datamartDefinition)
        {

            var audit = this.m_auditService.Audit()
                .WithAction(Core.Model.Audit.ActionType.Execute)
                .WithEventIdentifier(Core.Model.Audit.EventIdentifierType.ApplicationActivity)
                .WithEventType("DeInitDatamart", "http://santedb.org/conceptset/SecurityAuditCode#BI", "Destroy Datamart")
                .WithHttpInformation(RestOperationContext.Current?.IncomingRequest)
                .WithLocalDestination()
                .WithPrincipal()
                .WithRemoteSource(RemoteEndpointUtil.Current.GetRemoteClient());

            try
            {
                this.m_pepService.Demand(PermissionPolicyIdentifiers.AdministerWarehouse);
                using (var context = this.GetDataFlowExecutionContext(datamartDefinition, DataFlowExecutionPurposeType.DatabaseManagement | DataFlowExecutionPurposeType.SchemaManagement))
                {
                    try
                    {
                        using (var integrator = context.GetIntegrator(datamartDefinition.Produces))
                        {
                            this.m_tracer.TraceInfo("Dropping datamart schema {0}", datamartDefinition.Id);
                            integrator.DropDatabase();

                            audit.WithAuditableObjects(new AuditableObject()
                            {
                                IDTypeCode = AuditableObjectIdType.Custom,
                                CustomIdTypeCode = new AuditCode(nameof(BiDataSourceDefinition), "http://santedb.org/bi"),
                                LifecycleType = AuditableObjectLifecycle.PermanentErasure,
                                NameData = datamartDefinition.Produces.Name,
                                ObjectId = datamartDefinition.Produces.Id,
                                Role = AuditableObjectRole.DataRepository,
                                Type = AuditableObjectType.SystemObject
                            });
                        }
                        context.SetOutcome(DataFlowExecutionOutcomeType.Success);
                    }
                    catch(Exception e)
                    {
                        context.Log(System.Diagnostics.Tracing.EventLevel.Error, $"Destroy {datamartDefinition} failed with {e.ToHumanReadableString()}");
                        context.SetOutcome(DataFlowExecutionOutcomeType.Fail);
                    }
                }
                this.m_metadataRepository.Remove<BiDataSourceDefinition>(datamartDefinition.Produces.Id);
                audit.WithOutcome(OutcomeIndicator.Success).Send();

            }
            catch (Exception e)
            {
                audit.WithOutcome(OutcomeIndicator.MinorFail).Send();
                throw new BiException(this.m_localization.GetString(ErrorMessageStrings.DATAMART_DROP_ERROR, new { id = datamartDefinition.Id }), datamartDefinition, e);
            }
        }

        /// <inheritdoc/>
        public void Migrate(BiDatamartDefinition datamartDefinition)
        {

            var audit = this.m_auditService.Audit()
                .WithAction(Core.Model.Audit.ActionType.Execute)
                .WithEventIdentifier(Core.Model.Audit.EventIdentifierType.ApplicationActivity)
                .WithEventType("InitDataMart", "http://santedb.org/conceptset/SecurityAuditCode#BI", "Migrate Data Mart")
                .WithHttpInformation(RestOperationContext.Current?.IncomingRequest)
                .WithLocalDestination()
                .WithPrincipal()
                .WithRemoteSource(RemoteEndpointUtil.Current.GetRemoteClient());

            // get a context and destroy the datamart
            try
            {
                this.m_pepService.Demand(PermissionPolicyIdentifiers.AdministerWarehouse);
                this.m_metadataRepository.Remove<BiDataSourceDefinition>(datamartDefinition.Produces.Id);

                using (var context = this.GetDataFlowExecutionContext(datamartDefinition, DataFlowExecutionPurposeType.SchemaManagement | DataFlowExecutionPurposeType.DatabaseManagement))
                {
                    try
                    {
                        using (var integrator = context.GetIntegrator(datamartDefinition.Produces))
                        {
                            datamartDefinition = BiUtils.ResolveRefs(datamartDefinition);
                            this.MigrateInternal(context, integrator, datamartDefinition, audit);
                            this.m_metadataRepository.Insert(integrator.DataSource);

                        }

                        audit.WithOutcome(OutcomeIndicator.Success).Send();
                        context.SetOutcome(DataFlowExecutionOutcomeType.Success);
                    }
                    catch (Exception e)
                    {
                        context.SetOutcome(DataFlowExecutionOutcomeType.Fail);
                        context.Log(System.Diagnostics.Tracing.EventLevel.Error, $"Migrate {datamartDefinition} failed with {e.ToHumanReadableString()}");

                    }
                }
            }
            catch (Exception e)
            {
                audit.WithOutcome(OutcomeIndicator.MinorFail).Send();
                throw new BiException(this.m_localization.GetString(ErrorMessageStrings.DATAMART_CREATE_ERROR, new { id = datamartDefinition.Id }), datamartDefinition, e);
            }
        }

        /// <summary>
        /// Create the database if needed
        /// </summary>
        private void CreateDatabaseInternal(IDataFlowExecutionContext context, IDataIntegrator integrator, BiDatamartDefinition datamartDefinition, IAuditBuilder audit)
        {
            if (!integrator.DatabaseExists())
            {
                this.m_tracer.TraceInfo("Creating {0}...", datamartDefinition.Name);
                context.Log(System.Diagnostics.Tracing.EventLevel.Informational, "Create Database");
                integrator.CreateDatabase();
                audit.WithAuditableObjects(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.Custom,
                    CustomIdTypeCode = new AuditCode(nameof(BiDataSourceDefinition), "http://santedb.org/bi"),
                    LifecycleType = AuditableObjectLifecycle.Creation,
                    NameData = datamartDefinition.Produces.Name,
                    ObjectId = datamartDefinition.Produces.Id,
                    Role = AuditableObjectRole.DataRepository,
                    Type = AuditableObjectType.SystemObject
                });
            }

        }
        /// <summary>
        /// Performs the appropriate logic to migrate
        /// </summary>
        private void MigrateInternal(IDataFlowExecutionContext context, IDataIntegrator integrator, BiDatamartDefinition datamartDefinition, IAuditBuilder audit)
        {

            this.CreateDatabaseInternal(context, integrator, datamartDefinition, audit);
            integrator.OpenWrite();

            var toMigrate = datamartDefinition.SchemaObjects.Where(o => integrator.NeedsMigration(o));
            if (toMigrate.Any())
            {
                foreach (var itm in toMigrate)
                {
                    this.m_tracer.TraceInfo("Migrating {0}...", itm.Name);
                    integrator.RecreateObject(itm);
                    context.Log(System.Diagnostics.Tracing.EventLevel.Informational, $"Migrate {itm.Name}");
                    audit.WithAuditableObjects(new AuditableObject()
                    {
                        IDTypeCode = AuditableObjectIdType.Custom,
                        CustomIdTypeCode = new AuditCode(itm.GetType().Name, "http://santedb.org/bi"),
                        LifecycleType = AuditableObjectLifecycle.Creation,
                        NameData = itm.Name,
                        ObjectId = $"{datamartDefinition.Id}/schema/{itm.Name}",
                        Role = AuditableObjectRole.Table,
                        Type = AuditableObjectType.SystemObject
                    });
                }
            }
        }

        /// <inheritdoc/>
        public IDataFlowDiagnosticSession Refresh(BiDatamartDefinition datamartDefinition, bool diagnostics)
        {

            var audit = this.m_auditService.Audit()
                .WithAction(Core.Model.Audit.ActionType.Execute)
                .WithEventIdentifier(Core.Model.Audit.EventIdentifierType.ApplicationActivity)
                .WithEventType("RefreshDataMart", "http://santedb.org/conceptset/SecurityAuditCode#BI", "Refresh Data Mart")
                .WithHttpInformation(RestOperationContext.Current?.IncomingRequest)
                .WithLocalDestination()
                .WithPrincipal()
                .WithRemoteSource(RemoteEndpointUtil.Current.GetRemoteClient());

            // get a context and destroy the datamart
            try
            {

                // Remove this datamart from the registration since we don't want other people messing it up
                this.m_pepService.Demand(PermissionPolicyIdentifiers.WriteWarehouseData);

                var purpose = DataFlowExecutionPurposeType.Refresh | DataFlowExecutionPurposeType.DatabaseManagement | DataFlowExecutionPurposeType.SchemaManagement;
                if (diagnostics)
                {
                    purpose |= DataFlowExecutionPurposeType.Diagnostics;
                }

                using (var context = this.GetDataFlowExecutionContext(datamartDefinition, purpose))
                {

                    using (var integrator = context.GetIntegrator(datamartDefinition.Produces))
                    {
                        try
                        {

                            datamartDefinition = BiUtils.ResolveRefs(datamartDefinition);

                            if (context.DiagnosticSession != null)
                            {
                                context.DiagnosticSession.ActionStarted += (o, e) =>
                                {
                                    this.DiagnosticEventReceived?.Invoke(o, e);
                                    e.Action.SampleCollected += this.RelaySampleCollected;
                                };
                                context.DiagnosticSession.ActionEnded += (o, e) =>
                                {
                                    this.DiagnosticEventReceived?.Invoke(o, e);
                                    e.Action.SampleCollected -= this.RelaySampleCollected;
                                };
                            }

                            this.MigrateInternal(context, integrator, datamartDefinition, audit);
                            this.m_metadataRepository.Insert(integrator.DataSource);

                            // Call the main flow
                            var entryObject = datamartDefinition.EntryFlow?.Resolved ?? datamartDefinition.DataFlows.Find(o => o.Name == "main");
                            if (entryObject is BiDataFlowStep dfd)
                            {
                                var executionScope = new DataFlowScope("$", context);
                                executionScope.DeclareConstant(BiConstants.AuditDataFlowParameterName, audit);
                                executionScope.DeclareConstant(BiConstants.PrincipalDataFlowParameterName, AuthenticationContext.Current.Principal.Identity.Name);
                                executionScope.DeclareConstant(BiConstants.StartTimeDataFlowParameterName, DateTimeOffset.Now);
                                executionScope.DeclareConstant(BiConstants.DataMartDataFlowParameterName, datamartDefinition.Id);
                                dfd.Execute(executionScope).Count();
                            }
                            else
                            {
                                throw new InvalidOperationException(String.Format(ErrorMessages.MISSING_ENTRY_POINT, "main"));
                            }

                            audit.WithOutcome(OutcomeIndicator.Success).Send();
                            context.SetOutcome(DataFlowExecutionOutcomeType.Success);
                        }
                        catch(Exception e)
                        {
                            context.Log(System.Diagnostics.Tracing.EventLevel.Error, $"Refresh {datamartDefinition} failed with {e.ToHumanReadableString()}");
                            context.SetOutcome(DataFlowExecutionOutcomeType.Fail);
                            throw;
                        }
                        finally
                        {
                            this.m_metadataRepository.Insert(integrator.DataSource);
                        }
                        return context.DiagnosticSession;
                    }
                }
            }
            catch (Exception e)
            {
                audit.WithOutcome(OutcomeIndicator.MinorFail).Send();
                throw new BiException(this.m_localization.GetString(ErrorMessageStrings.DATAMART_CREATE_ERROR, new { id = datamartDefinition.Id }), datamartDefinition, e);
            }
        }

        /// <summary>
        /// Relay an event for a sample collection
        /// </summary>
        private void RelaySampleCollected(object sender, DiagnosticSampleEventArgs e) => this.DiagnosticEventReceived?.Invoke(sender, e);
    }
}
