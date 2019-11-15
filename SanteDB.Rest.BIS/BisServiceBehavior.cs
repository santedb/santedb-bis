using RestSrvr.Attributes;
using RestSrvr.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SanteDB.Core.Model;
using SanteDB.Rest.Common.Attributes;
using SanteDB.Core.Security;
using System.Linq.Expressions;
using SanteDB.Core.Model.Query;
using RestSrvr;
using System.Collections;
using SanteDB.BI.Util;
using SanteDB.Core.Services;
using System.Globalization;
using SanteDB.Core.Security.Audit;
using SanteDB.Core.Auditing;
using System.Text.RegularExpressions;
using SanteDB.BI;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Default implementation of the BIS service contract
    /// </summary>
    [ServiceBehavior(Name = "BIS", InstanceMode = ServiceInstanceMode.Singleton)]
    public class BisServiceBehavior : IBisServiceContract
    {

        // Context parameters
        protected Dictionary<String, Func<Object>> m_contextParams = new Dictionary<string, Func<object>>()
        {
            { "Context.UserName", () => AuthenticationContext.Current.Principal.Identity.Name },
            { "Context.UserId", () => ApplicationServiceContext.Current.GetService<ISecurityRepositoryService>().GetUser(AuthenticationContext.Current.Principal.Identity)?.Key },
            { "Context.UserEntityId", () => ApplicationServiceContext.Current.GetService<ISecurityRepositoryService>().GetUserEntity(AuthenticationContext.Current.Principal.Identity)?.Key },
            { "Context.ProviderId", () => ApplicationServiceContext.Current.GetService<ISecurityRepositoryService>().GetProviderEntity(AuthenticationContext.Current.Principal.Identity)?.Key },
            { "Context.Language", () => CultureInfo.CurrentCulture.TwoLetterISOLanguageName }
        };

        // Default tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(BisServiceBehavior));

        // Metadata repository
        private IBiMetadataRepository m_metadataRepository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();

        /// <summary>
        /// Get resource type
        /// </summary>
        private Type GetResourceType (String resourceTypeName)
        {
            return typeof(BiDefinition).Assembly.ExportedTypes.FirstOrDefault(o => o.GetCustomAttribute<XmlRootAttribute>()?.ElementName == resourceTypeName);
        }

        /// <summary>
        /// Create the specified BIS metadata object
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public BiDefinition Create(string resourceType, BiDefinition body)
        {
            try
            {
                // Ensure that endpoint agrees with the body definition
                var rt = this.GetResourceType(resourceType);
                if (body.GetType() != rt)
                    throw new FaultException(400, "Invalid resource type");
                return this.m_metadataRepository.Insert(body);
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Create: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Delete the specified resource type
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public BiDefinition Delete(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                return this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Remove),
                    new Type[] { rt },
                    new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BiDefinition;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Delete: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Get the specified resource type
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.ReadMetadata)]
        public BiDefinition Get(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                var retVal = this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Get),
                    new Type[] { rt },
                    new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BiDefinition;

                // Resolve any refs in the object
                if (retVal == null)
                    throw new KeyNotFoundException(id);
                else
                {
                    return BiUtils.ResolveRefs(retVal);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Get: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Get service options
        /// </summary>
        public ServiceOptions Options()
        {
            return new ServiceOptions()
            {
                InterfaceVersion = typeof(BisServiceBehavior).Assembly.GetName().Version.ToString(),
                Resources = typeof(IBisServiceContract).GetCustomAttributes<ServiceKnownResourceAttribute>().Select(o => new ServiceResourceOptions(
                    o.Type.GetCustomAttribute<XmlRootAttribute>().ElementName,
                    o.Type,
                    new List<ServiceResourceCapability>()
                    {
                        new ServiceResourceCapability(ResourceCapabilityType.Create, new String[] { PermissionPolicyIdentifiers.UnrestrictedMetadata }),
                        new ServiceResourceCapability(ResourceCapabilityType.Update, new String[] { PermissionPolicyIdentifiers.UnrestrictedMetadata }),
                        new ServiceResourceCapability(ResourceCapabilityType.Delete, new String[] { PermissionPolicyIdentifiers.UnrestrictedMetadata }),
                        new ServiceResourceCapability(ResourceCapabilityType.Search, new String[] { PermissionPolicyIdentifiers.ReadMetadata}),
                        new ServiceResourceCapability(ResourceCapabilityType.Get, new String[] { PermissionPolicyIdentifiers.ReadMetadata })
                    }
                    )).ToList()
            };
        }

        /// <summary>
        /// Execute a ping
        /// </summary>
        public void Ping()
        {
        }

        /// <summary>
        /// Hydrate the query
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        private BisResultContext HydrateQuery(String queryId)
        {
            AuditData audit = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.Success, EventIdentifierType.Query, AuditUtil.CreateAuditActionCode(EventTypeCodes.Query));
            try
            {
                // First we want to grab the appropriate source for this ID
                var viewDef = this.m_metadataRepository.Get<BiViewDefinition>(queryId);
                if (viewDef == null)
                {
                    var queryDef = this.m_metadataRepository.Get<BiQueryDefinition>(queryId);
                    if (queryDef == null)
                        throw new KeyNotFoundException(queryId);
                    viewDef = new BiViewDefinition()
                    {
                        Query = queryDef
                    };
                }

                viewDef = SanteDB.BI.Util.BiUtils.ResolveRefs(viewDef) as BiViewDefinition;
                var dsource = viewDef.Query?.DataSources.FirstOrDefault(o => o.Name == "main") ?? viewDef.Query?.DataSources.FirstOrDefault();
                if (dsource == null)
                    throw new KeyNotFoundException("Query does not contain a data source");

                IBiDataSource providerImplementation = null;
                if (dsource.ProviderType != null)
                    providerImplementation = Activator.CreateInstance(dsource.ProviderType) as IBiDataSource;
                else
                    providerImplementation = ApplicationServiceContext.Current.GetService<IBiDataSource>(); // Global default

                // Parameters
                Dictionary<String, object> parameters = new Dictionary<string, object>();
                foreach (var kv in RestOperationContext.Current.IncomingRequest.QueryString.AllKeys)
                    parameters.Add(kv, RestOperationContext.Current.IncomingRequest.QueryString[kv]);
                // Populate data about the query
                audit.AuditableObjects.Add(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.ReportNumber,
                    LifecycleType = AuditableObjectLifecycle.Report,
                    ObjectId = queryId,
                    QueryData = RestOperationContext.Current.IncomingRequest.Url.Query,
                    Role = AuditableObjectRole.Query,
                    Type = AuditableObjectType.SystemObject
                });

                // Context parameters
                foreach (var kv in this.m_contextParams)
                    if (!parameters.ContainsKey(kv.Key))
                        parameters.Add(kv.Key, kv.Value());

                // Aggregations and groups?
                if (RestOperationContext.Current.IncomingRequest.QueryString["_groupBy"] != null)
                {
                    var aggRx = new Regex(@"(\w*)\((.*?)\)");
                    viewDef.AggregationDefinitions = new List<BiAggregationDefinition>()
                    {
                        new BiAggregationDefinition()
                        {
                            Groupings = RestOperationContext.Current.IncomingRequest.QueryString.GetValues("_groupBy").Select(o=>new BiSqlColumnReference()
                            {
                                ColumnSelector = o,
                                Name = o
                            }).ToList(),
                            Columns = RestOperationContext.Current.IncomingRequest.QueryString.GetValues("_select").Select(o=>{
                                var match = aggRx.Match(o);
                                if(!match.Success)
                                    throw new InvalidOperationException("Aggregation function must be in format AGGREGATOR(COLUMN)");
                                return new BiAggregateSqlColumnReference()
                                {
                                    Aggregation = (BiAggregateFunction)Enum.Parse(typeof(BiAggregateFunction), match.Groups[1].Value),
                                    ColumnSelector = match.Groups[2].Value,
                                    Name = match.Groups[2].Value
                                };
                            }).ToList()
                        }
                    };
                }

                int offset = 0, count = 100;
                if (!Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString["_offset"] ?? "0", out offset))
                    throw new FormatException("_offset is not in the correct format");
                if (!Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString["_count"] ?? "100", out count))
                    throw new FormatException("_count is not in the correct format");


                    var queryData = providerImplementation.ExecuteView(viewDef, parameters, offset, count);
                return queryData;
            }
            catch (KeyNotFoundException)
            {
                audit.Outcome = OutcomeIndicator.MinorFail;
                throw;
            }
            catch (Exception e)
            {
                audit.Outcome = OutcomeIndicator.MinorFail;
                this.m_tracer.TraceError("Error rendering query: {0}", e);
                throw new FaultException(500, $"Error rendering query {queryId}", e);
            }
            finally
            {
                AuditUtil.AddLocalDeviceActor(audit);
                AuditUtil.AddRemoteDeviceActor(audit);
                AuditUtil.AddUserActor(audit);
                AuditUtil.SendAudit(audit);
            }
        }
        /// <summary>
        /// Render the specified query
        /// </summary>
        /// Policies controlled by query implementation
        [Demand(PermissionPolicyIdentifiers.Login)]
        public IEnumerable<dynamic> RenderQuery(string id)
        {
            var retVal = this.HydrateQuery(id);
            return retVal.Dataset;
        }

        /// <summary>
        /// Search for BIS definitions
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.ReadMetadata)]
        public BiDefinitionCollection Search(string resourceType)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                var expression = typeof(QueryExpressionParser).GetGenericMethod(nameof(QueryExpressionParser.BuildLinqExpression),
                    new Type[] { rt },
                    new Type[] { typeof(NameValueCollection) }
                ).Invoke(null, new Object[] { NameValueCollection.ParseQueryString(RestOperationContext.Current.IncomingRequest.Url.Query) });

                int offset = Int32.Parse(RestOperationContext.Current.IncomingRequest.QueryString["_offset"] ?? "0"),
                    count = Int32.Parse(RestOperationContext.Current.IncomingRequest.QueryString["_count"] ?? "100");
                // Execute the query
                return new BiDefinitionCollection((this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Query),
                    new Type[] { rt },
                    new Type[] { expression.GetType(), typeof(int), typeof(int?) })
                .Invoke(this.m_metadataRepository, new object[] { expression, offset, count }) as IEnumerable).OfType<BiDefinition>());
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Query: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Update the specfied resource (delete/create)
        /// </summary>
        public BiDefinition Update(string resourceType, string id, BiDefinition body)
        {
            this.Delete(resourceType, id);
            return this.Create(resourceType, body);
        }

        /// <summary>
        /// Render a report
        /// </summary>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Stream RenderReport(string id, string format)
        {
            throw new NotImplementedException();
        }
    }
}
