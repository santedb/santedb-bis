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

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Default implementation of the BIS service contract
    /// </summary>
    [ServiceBehavior(Name = "Business Intelligence Service", InstanceMode = ServiceInstanceMode.Singleton)]
    public class BisServiceBehavior : IBisServiceContract
    {

        // Default tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(BisServiceBehavior));

        // Metadata repository
        private IBisMetadataRepository m_metadataRepository = ApplicationServiceContext.Current.GetService<IBisMetadataRepository>();

        /// <summary>
        /// Get resource type
        /// </summary>
        private Type GetResourceType (String resourceTypeName)
        {
            return typeof(BisDefinition).Assembly.ExportedTypes.FirstOrDefault(o => o.GetCustomAttribute<XmlRootAttribute>()?.ElementName == resourceTypeName);
        }

        /// <summary>
        /// Create the specified BIS metadata object
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public BisDefinition Create(string resourceType, BisDefinition body)
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
        public BisDefinition Delete(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                return this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBisMetadataRepository.Remove),
                    new Type[] { rt },
                    new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BisDefinition;
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
        public BisDefinition Get(string resourceType, string id)
        {
            try
            {
                var rt = this.GetResourceType(resourceType);
                return this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBisMetadataRepository.Get),
                    new Type[] { rt },
                    new Type[] { typeof(String) }).Invoke(this.m_metadataRepository, new object[] { id }) as BisDefinition;
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
        /// Render the specified query
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.QueryWarehouseData)]
        public IEnumerable<dynamic> RenderQuery(string id, string format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Render the specified query
        /// </summary>
        [Demand(PermissionPolicyIdentifiers.QueryWarehouseData)]
        public Stream RenderView(string id, string format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for BIS definitions
        /// </summary>
        public List<BisDefinition> Search(string resourceType)
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
                return (this.m_metadataRepository.GetType().GetGenericMethod(nameof(IBisMetadataRepository.Query),
                    new Type[] { rt },
                    new Type[] { expression.GetType(), typeof(int), typeof(int?) })
                .Invoke(this.m_metadataRepository, new object[] { expression, offset, count }) as IEnumerable).OfType<BisDefinition>().ToList();
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
        public BisDefinition Update(string resourceType, string id, BisDefinition body)
        {
            this.Delete(resourceType, id);
            return this.Create(resourceType, body);
        }
    }
}
