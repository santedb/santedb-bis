﻿/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-5-30
 */
using RestSrvr;
using RestSrvr.Attributes;
using RestSrvr.Exceptions;
using SanteDB.BI;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Audit;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Audit;
using SanteDB.Core.Security.Claims;
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
    [ServiceBehavior(Name = "BIS", InstanceMode = ServiceInstanceMode.Singleton)]
    public class BisServiceBehavior : IBisServiceContract
    {

        /// <summary>
        /// Default context parameters to be sent to all reports
        /// </summary>
        protected Dictionary<String, Func<Object>> m_contextParams = new Dictionary<string, Func<object>>()
        {
            { "Context.UserName", () => AuthenticationContext.Current.Principal.Identity.Name },
            { "Context.UserId", () => ApplicationServiceContext.Current.GetService<ISecurityRepositoryService>().GetUser(AuthenticationContext.Current.Principal.Identity)?.Key },
            { "Context.UserEntityId", () => ApplicationServiceContext.Current.GetService<ISecurityRepositoryService>().GetUserEntity(AuthenticationContext.Current.Principal.Identity)?.Key },
            { "Context.Language", () => AuthenticationContext.Current.Principal.GetClaimValue(SanteDBClaimTypes.Language) ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName }
        };

        // Default tracer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(BisServiceBehavior));

        // Metadata repository
        private readonly IBiMetadataRepository m_metadataRepository;

        // Service manager
        private readonly IServiceManager m_serviceManager;

        readonly IAuditService _AuditService;


        /// <summary>
        /// BI Service behavior
        /// </summary>
        public BisServiceBehavior() : 
            this(ApplicationServiceContext.Current.GetService<IServiceManager>(), ApplicationServiceContext.Current.GetService<IBiMetadataRepository>(), ApplicationServiceContext.Current.GetAuditService())
        {

        }
        /// <summary>
        /// DI constructor
        /// </summary>
        public BisServiceBehavior(IServiceManager serviceManager, IBiMetadataRepository metadataRepository, IAuditService auditService)
        {
            this.m_serviceManager = serviceManager;
            this.m_metadataRepository = metadataRepository;
            _AuditService = auditService;
        }

        /// <summary>
        /// Get resource type
        /// </summary>
        private Type GetResourceType(String resourceTypeName)
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
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadRequest, "Invalid resource type");
                }

                return this.m_metadataRepository.Insert(body);
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
                {
                    throw new KeyNotFoundException(id);
                }
                else
                {
                    retVal = BiUtils.ResolveRefs(retVal);
                    (retVal as BiReportDefinition)?.Views?.ForEach(o => o.IncludeBody = true);
                    return retVal;
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
                    }, null
                    )).ToList()
            };
        }

        /// <summary>
        /// Execute a ping
        /// </summary>
        public void Ping()
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

                        queryDef = parmDef?.Values as BiQueryDefinition;
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
                if (!Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString[QueryControlParameterNames.HttpOffsetParameterName], out offset))
                {
                    throw new FormatException("_offset is not in the correct format");
                }

                if (!Int32.TryParse(RestOperationContext.Current.IncomingRequest.QueryString[QueryControlParameterNames.HttpCountParameterName], out count))
                {
                    throw new FormatException("_count is not in the correct format");
                }

                var queryData = providerImplementation.ExecuteView(viewDef, parameters, offset, count);
                return queryData;
            }
            catch (KeyNotFoundException)
            {
                audit = audit.WithOutcome(OutcomeIndicator.MinorFail);
                throw;
            }
            catch (Exception e)
            {
                audit = audit.WithOutcome(OutcomeIndicator.MinorFail);
                this.m_tracer.TraceError("Error rendering query: {0}", e);
                throw new FaultException(System.Net.HttpStatusCode.InternalServerError, $"Error rendering query {queryId}", e);
            }
            finally
            {
                audit.WithLocalDevice().WithPrincipal().Send();
            }
        }

        /// <summary>
        /// Creates a parameter dictionary from the HTTP request context
        /// </summary>
        private IDictionary<String, Object> CreateParameterDictionary()
        {
            // Parameters
            Dictionary<String, object> parameters = RestOperationContext.Current.IncomingRequest.Url.Query.ParseQueryString().ToDictionary().ToDictionary(o => o.Key, o => o.Value.Length == 1 ? o.Value[0] : (object)o.Value);

            // Context parameters
            foreach (var kv in this.m_contextParams)
            {
                if (!parameters.ContainsKey(kv.Key))
                {
                    try
                    {
                        parameters.Add(kv.Key, kv.Value());
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceWarning("Cannot hydrate parameter {0} : {1}", kv.Key, e);
                    }
                }
            }

            return parameters;
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
                var expression = QueryExpressionParser.BuildLinqExpression(rt, RestOperationContext.Current.IncomingRequest.Url.Query.ParseQueryString());

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
        [Demand(PermissionPolicyIdentifiers.Login)]
        public Stream RenderReport(string id, string format)
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
    }
}