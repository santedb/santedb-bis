/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2023-6-21
 */
using RestSrvr;
using RestSrvr.Attributes;
using RestSrvr.Exceptions;
using SanteDB.BI;
using SanteDB.BI.Datamart;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Audit;
using SanteDB.Core.Model.Parameters;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Audit;
using SanteDB.Core.Security.Claims;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using SanteDB.Rest.Common;
using SanteDB.Rest.Common.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Default implementation of the BIS service contract
    /// </summary>
    [ServiceBehavior(Name = BisMessageHandler.ConfigurationName, InstanceMode = ServiceInstanceMode.Singleton)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BisServiceBehavior : IBisServiceContract
    {

        // Default tracer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(BisServiceBehavior));

        // Metadata repository
        private readonly IBiMetadataRepository m_metadataRepository;

        // Datamart repository
        private readonly IBiDatamartRepository m_datamartRepository;

        // Service manager
        private readonly IServiceManager m_serviceManager;

        // Audit Service
        private readonly IAuditService _AuditService;

        // Operations registered
        private readonly IEnumerable<IApiChildOperation> m_registeredOperations;

        /// <summary>
        /// BI Service behavior
        /// </summary>
        public BisServiceBehavior() :
            this(ApplicationServiceContext.Current.GetService<IServiceManager>(),
                ApplicationServiceContext.Current.GetService<IBiMetadataRepository>(),
                ApplicationServiceContext.Current.GetService<IBiDatamartRepository>(),
                ApplicationServiceContext.Current.GetAuditService())
        {
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public BisServiceBehavior(IServiceManager serviceManager, IBiMetadataRepository metadataRepository, IBiDatamartRepository datamartRepository, IAuditService auditService)
        {
            this.m_serviceManager = serviceManager;
            this.m_datamartRepository = datamartRepository;
            this.m_metadataRepository = metadataRepository;
            this.m_registeredOperations = serviceManager.CreateInjectedOfAll<IApiChildOperation>()
                .Where(o => o.ScopeBinding == ChildObjectScopeBinding.Instance && o.ParentTypes.Any(t => typeof(IBisServiceContract).GetCustomAttributes<ServiceKnownResourceAttribute>().Any(k => k.Type == t)))
                .ToList();
            _AuditService = auditService;
        }

        /// <summary>
        /// Get resource type
        /// </summary>
        private Type GetResourceType(String resourceTypeName)
        {
            return typeof(BiDefinition).Assembly.GetExportedTypesSafe().FirstOrDefault(o => o.GetCustomAttribute<XmlRootAttribute>()?.ElementName == resourceTypeName);
        }

        /// <summary>
        /// Create the specified BIS metadata object
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public virtual BiDefinition Create(string resourceType, BiDefinition body)
        {

            try
            {
                // Ensure that endpoint agrees with the body definition
                var rt = this.GetResourceType(resourceType);
                if (body.GetType() != rt)
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadRequest, "Invalid resource type");
                }

                if (rt == typeof(DatamartInfo) && body is BiDatamartDefinition definition)
                {
                    return new DatamartInfo(this.m_datamartRepository.Register(definition));
                }
                else
                {
                    return this.m_metadataRepository.Insert(body);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Create: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Delete the specified resource type
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public virtual BiDefinition Delete(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                if (rt == typeof(DatamartInfo))
                {
                    var reg = this.m_datamartRepository.Find(o => o.Id == id).FirstOrDefault();
                    if (reg == null)
                    {
                        throw new KeyNotFoundException();
                    }
                    this.m_datamartRepository.Unregister(reg);
                    return new DatamartInfo(reg);
                }
                else
                {
                    return this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Remove),
                    new Type[] { rt },
                    new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BiDefinition;
                }
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
        public virtual BiDefinition Get(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);


                if (rt == typeof(DatamartInfo))
                {
                    var reg = this.m_datamartRepository.Find(o => o.Id == id).FirstOrDefault();
                    if (reg == null)
                    {
                        throw new KeyNotFoundException(id);
                    }
                    return new DatamartInfo(reg);
                }
                else
                {
                    var retVal = this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Get),
                        new Type[] { rt },
                        new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BiDefinition;


                    // Resolve any refs in the object
                    if (retVal == null)
                    {
                        throw new KeyNotFoundException(id);
                    }
                    else
                    {
                        retVal = BiUtils.ResolveRefs(retVal);
                        return retVal;
                    }
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
        public virtual ServiceOptions Options()
        {
            return new ServiceOptions()
            {
                InterfaceVersion = typeof(BisServiceBehavior).Assembly.GetName().Version.ToString(),
                ServerVersion = $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product} v{Assembly.GetEntryAssembly().GetName().Version} ({Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion})",
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
                    }, null
                    )).ToList()
            };
        }

        /// <summary>
        /// Execute a ping
        /// </summary>
        public virtual void Ping()
        {
            RestOperationContext.Current.OutgoingResponse.StatusCode = (int)System.Net.HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Hydrate the query
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        private BisResultContext HydrateQuery(String queryId)
        {
            var audit = _AuditService.Audit(DateTimeOffset.Now, ActionType.Execute, OutcomeIndicator.Success, EventIdentifierType.Query, EventTypeCodes.SecondaryUseQuery);

            try
            {
                // First we want to grab the appropriate source for this ID
                var viewDef = this.m_metadataRepository.Get<BiViewDefinition>(queryId);
                if (viewDef == null)
                {
                    var queryDef = this.m_metadataRepository.Get<BiQueryDefinition>(queryId);
                    if (queryDef == null) // Parameter value
                    {
                        var parmDef = this.m_metadataRepository.Get<BiParameterDefinition>(queryId);
                        if (parmDef == null)
                        {
                            throw new KeyNotFoundException($"Could not find a Parameter, Query or View to hydrate named {queryId}");
                        }

                        queryDef = parmDef?.ValueDefinition as BiQueryDefinition;
                        queryDef.Id = queryDef.Id ?? queryId;
                    }

                    viewDef = new BiViewDefinition()
                    {
                        Id = queryDef.Id,
                        Query = queryDef
                    };
                }

                viewDef = SanteDB.BI.Util.BiUtils.ResolveRefs(viewDef) as BiViewDefinition;
                var dsource = viewDef.Query?.DataSources.FirstOrDefault(o => o.Name == "main") ?? viewDef.Query?.DataSources.FirstOrDefault();
                if (dsource == null)
                {
                    throw new KeyNotFoundException("Query does not contain a data source");
                }

                IBiDataSource providerImplementation = null;
                if (dsource.ProviderType != null && this.m_metadataRepository.IsLocal)
                {
                    providerImplementation = this.m_serviceManager.CreateInjected(dsource.ProviderType) as IBiDataSource;
                }
                else
                {
                    providerImplementation = ApplicationServiceContext.Current.GetService<IBiDataSource>(); // Global default
                }

                // Populate data about the query
                audit = audit.WithAuditableObjects(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.ReportNumber,
                    LifecycleType = AuditableObjectLifecycle.Report,
                    ObjectId = queryId,
                    QueryData = RestOperationContext.Current.IncomingRequest.Url.Query,
                    Role = AuditableObjectRole.Query,
                    Type = AuditableObjectType.SystemObject
                });

                var parameters = this.CreateParameterDictionary();

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
                                ColumnSelector = o.Contains("::")  ? $"CAST({o.Substring(0, o.IndexOf(":"))} AS {o.Substring(o.IndexOf(":") + 2)})" : o,
                                Name = o.Contains("::") ? o.Substring(0, o.IndexOf(":")) : o
                            }).ToList(),
                            Columns = RestOperationContext.Current.IncomingRequest.QueryString.GetValues("_select").Select(o=>{
                                var match = aggRx.Match(o);
                                if(!match.Success) { throw new InvalidOperationException("Aggregation function must be in format AGGREGATOR(COLUMN)"); } return new BiAggregateSqlColumnReference()
                                {
                                    Aggregation = (BiAggregateFunction)Enum.Parse(typeof(BiAggregateFunction), match.Groups[1].Value, true),
                                    ColumnSelector = match.Groups[2].Value,
                                    Name = match.Groups[2].Value
                                };
                            }).ToList()
                        }
                    };
                }

                int offset = 0, count = 100;
                _ = Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString[QueryControlParameterNames.HttpOffsetParameterName], out offset);
                if (!Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString[QueryControlParameterNames.HttpCountParameterName], out count))
                {
                    count = 100;
                }

                var queryData = providerImplementation.ExecuteView(viewDef, parameters, offset, count);
                return queryData;
            }
            catch (KeyNotFoundException)
            {
                audit = audit.WithOutcome(OutcomeIndicator.SeriousFail);
                throw;
            }
            catch (Exception e)
            {
                audit = audit.WithOutcome(OutcomeIndicator.SeriousFail);
                this.m_tracer.TraceError("Error rendering query: {0}", e);
                throw new FaultException(System.Net.HttpStatusCode.InternalServerError, $"Error rendering query {queryId}", e);
            }
            finally
            {
                audit.WithRemoteSource(RemoteEndpointUtil.Current.GetRemoteClient()).WithLocalDestination().WithPrincipal().Send();
            }
        }

        /// <summary>
        /// Creates a parameter dictionary from the HTTP request context
        /// </summary>
        private IDictionary<String, Object> CreateParameterDictionary() =>  RestOperationContext.Current.IncomingRequest.Url.Query.ParseQueryString().ToDictionary().ToDictionary(o => o.Key, o => o.Value.Length == 1 ? o.Value[0] : (object)o.Value);

        /// <summary>
        /// Render the specified query
        /// </summary>
        /// Policies controlled by query implementation
        [Demand(PermissionPolicyIdentifiers.Login)]
        public virtual IEnumerable<dynamic> RenderQuery(string id)
        {
            var retVal = this.HydrateQuery(id);
            return retVal.Records;
        }

        /// <summary>
        /// Search for BIS definitions
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.ReadMetadata)]
        public virtual BiDefinitionCollection Search(string resourceType)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                int offset = Int32.Parse(RestOperationContext.Current.IncomingRequest.QueryString["_offset"] ?? "0"),
                    count = Int32.Parse(RestOperationContext.Current.IncomingRequest.QueryString["_count"] ?? "100");

                if (rt == typeof(DatamartInfo))
                {
                    var expression = QueryExpressionParser.BuildLinqExpression<IDatamart>(RestOperationContext.Current.IncomingRequest.Url.Query.ParseQueryString());
                    var results = this.m_datamartRepository.Find(expression);
                    return new BiDefinitionCollection(results.Skip(offset).Take(count).ToArray().Select(o => new DatamartInfo(o)))
                    {
                        TotalResults = results.Count(),
                        Offset = offset
                    };
                }
                else
                {
                    var expression = QueryExpressionParser.BuildLinqExpression(rt, RestOperationContext.Current.IncomingRequest.Url.Query.ParseQueryString());
                    // Execute the query
                    var retVal = (this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Query),
                        new Type[] { rt },
                        new Type[] { expression.GetType() })
                    .Invoke(this.m_metadataRepository, new object[] { expression }) as IEnumerable).OfType<BiDefinition>().Select(o => o.AsSummarized());

                    return new BiDefinitionCollection(retVal.Skip(offset).Take(count))
                    {
                        TotalResults = retVal.Count(),
                        Offset = offset
                    };
                }
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
        public virtual BiDefinition Update(string resourceType, string id, BiDefinition body)
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
        [Demand(PermissionPolicyIdentifiers.Login)]
        public virtual Stream RenderReport(string id, string format)
        {
            try
            {

                // Get the view name
                var view = RestOperationContext.Current.IncomingRequest.QueryString["_view"];

                var retVal = ApplicationServiceContext.Current.GetService<IBiRenderService>().Render(id, view, format, this.CreateParameterDictionary(), out string mimeType);

                // Set output headers
                RestOperationContext.Current.OutgoingResponse.ContentType = mimeType;
                if (RestOperationContext.Current.IncomingRequest.QueryString["_download"] == "true")
                {
                    RestOperationContext.Current.OutgoingResponse.AddHeader("Content-Disposition", $"attachment; filename=\"{id}-{view}-{DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss")}.{format}\"");
                }

                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error rendering BIS report: {0} : {1}", id, e);
                throw new Exception($"Error rendering BIS report: {id}", e);
            }
        }

        /// <inheritdoc/>
        public virtual object Invoke(string resourceType, string id, string operationName, ParameterCollection parameters)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                var candService = this.m_registeredOperations.FirstOrDefault(o => o.Name == operationName && o.ParentTypes.Contains(rt));
                if (candService == null)
                {
                    throw new KeyNotFoundException(operationName);
                }
                return candService.Invoke(rt, id, parameters);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing BIS Operation: {0}", e);
                throw;
            }
        }
    }
}