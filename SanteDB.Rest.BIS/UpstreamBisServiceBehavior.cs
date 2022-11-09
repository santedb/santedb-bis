﻿using RestSrvr;
using RestSrvr.Attributes;
using RestSrvr.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Http;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using SanteDB.Rest.BIS.Configuration;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// BIS behavior that can call the upstream
    /// </summary>
    public class UpstreamBisServiceBehavior : BisServiceBehavior
    {
        private readonly IUpstreamIntegrationService m_upstreamIntegrationService;
        private readonly IUpstreamAvailabilityProvider m_upstreamAvailabilityProvider;
        private readonly IRestClientFactory m_restClientFactory;
        private readonly IBiMetadataRepository m_biRepository;
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(UpstreamBisServiceBehavior));
        private readonly BisServiceConfigurationSection m_configuration;

        /// <summary>
        /// Constructor rest service
        /// </summary>
        public UpstreamBisServiceBehavior()
            : this(
                  ApplicationServiceContext.Current.GetService<IConfigurationManager>(),
                  ApplicationServiceContext.Current.GetService<IServiceManager>(),
                  ApplicationServiceContext.Current.GetService<IBiMetadataRepository>(),
                  ApplicationServiceContext.Current.GetService<IAuditService>(),
                  ApplicationServiceContext.Current.GetService<IUpstreamIntegrationService>(),
                  ApplicationServiceContext.Current.GetService<IUpstreamAvailabilityProvider>(),
                  ApplicationServiceContext.Current.GetService<IRestClientFactory>(),
                  ApplicationServiceContext.Current.GetService<IBiMetadataRepository>()

                )
        {

        }

        /// <summary>
        /// Tag the object if it is only upstream or if it exists locally 
        /// </summary>
        private void TagUpstream(params object[] dataObjects)
        {
            using (AuthenticationContext.EnterSystemContext())
            {
                foreach (var data in dataObjects)
                {
                    if (data is BiDefinition definition &&
                                       this.m_biRepository?.Query(data.GetType(), o => o.Id == definition.Id).Any() != true)
                    {
                        definition.MetaData.Tags.Add(new BiMetadataTag("$upstream", "true"));
                    }
                    else if (data is BiDefinitionCollection bundle)
                    {
                        this.TagUpstream(bundle.Items.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public UpstreamBisServiceBehavior(IConfigurationManager configurationManager, IServiceManager serviceManager, IBiMetadataRepository metadataRepository, IAuditService auditService, IUpstreamIntegrationService upstreamIntegrationService, IUpstreamAvailabilityProvider upstreamAvailabilityProvider, IRestClientFactory restClientFactory, IBiMetadataRepository biMetadataRepository) : base(serviceManager, metadataRepository, auditService)
        {
            this.m_upstreamIntegrationService = upstreamIntegrationService;
            this.m_upstreamAvailabilityProvider = upstreamAvailabilityProvider;
            this.m_restClientFactory = restClientFactory;
            this.m_biRepository = biMetadataRepository;
            this.m_configuration = configurationManager.GetSection<BisServiceConfigurationSection>();
        }
        /// <summary>
        /// Returns true if the request should be forwarded
        /// </summary>
        private bool ShouldForwardRequest() =>
            Boolean.TryParse(RestOperationContext.Current.IncomingRequest.QueryString[QueryControlParameterNames.HttpUpstreamParameterName], out var upstreamQry) && upstreamQry ||
                Boolean.TryParse(RestOperationContext.Current.IncomingRequest.Headers[ExtendedHttpHeaderNames.UpstreamHeaderName], out var upstreamHdr) && upstreamHdr ||
            this.m_configuration.AutomaticallyForwardRequests;

        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's query to the configured upstream server")]
        public override BiDefinition Create(string resourceType, BiDefinition body)
        {
            // Perform only on the external server
            if(this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            return restClient.Post<BiDefinition, BiDefinition>($"{resourceType}", body);
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.Create(resourceType, body);
            }
        }

        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's delete to the configured upstream server")]
        public override BiDefinition Delete(string resourceType, string id)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            return restClient.Delete<BiDefinition>($"{resourceType}/{id}");
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.Delete(resourceType, id);
            }
        }

        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's get to the configured upstream server")]
        public override BiDefinition Get(string resourceType, string id)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            var retVal = restClient.Get<BiDefinition>($"{resourceType}/{id}");
                            this.TagUpstream(retVal);
                            return retVal;
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.Get(resourceType, id);
            }
        }


        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's get to the configured upstream server")]
        public override IEnumerable<dynamic> RenderQuery(string id)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            var retVal = restClient.Get<IEnumerable<dynamic>>($"Query/{id}");
                            return retVal;
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.RenderQuery(id);
            }
        }


        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's get to the configured upstream server")]
        public override Stream RenderReport(string id, string format)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            var retVal = restClient.Get($"Report/{format}/{id}");
                            return new MemoryStream(retVal);
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.RenderReport(id, format);
            }
        }


        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's get to the configured upstream server")]
        public override BiDefinitionCollection Search(string resourceType)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            var retVal = restClient.Get<BiDefinitionCollection>($"{resourceType}", RestOperationContext.Current.IncomingRequest.QueryString);
                            this.TagUpstream(retVal);
                            return retVal;
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.Search(resourceType);
            }
        }

        /// <inheritdoc/>
        [UrlParameter(QueryControlParameterNames.HttpUpstreamParameterName, typeof(bool), "When true, forces this API to relay the caller's update to the configured upstream server")]
        public override BiDefinition Update(string resourceType, string id, BiDefinition body)
        {
            // Perform only on the external server
            if (this.ShouldForwardRequest())
            {
                if (this.m_upstreamAvailabilityProvider.IsAvailable(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                {
                    try
                    {
                        using (var restClient = this.m_restClientFactory.GetRestClientFor(Core.Interop.ServiceEndpointType.BusinessIntelligenceService))
                        {
                            restClient.Responded += (o, e) => RestOperationContext.Current.OutgoingResponse.SetETag(e.ETag);
                            return restClient.Put<BiDefinition, BiDefinition>($"{resourceType}/{id}", body);
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error performing online operation: {0}", e.InnerException);
                        throw;
                    }
                }
                else
                {
                    throw new FaultException(System.Net.HttpStatusCode.BadGateway);
                }
            }
            else
            {
                return base.Update(resourceType, id, body);
            }
        }
    }
}