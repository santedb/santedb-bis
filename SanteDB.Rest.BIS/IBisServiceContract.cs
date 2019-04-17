 using RestSrvr.Attributes;
using SanteDB.BI.Model;
using SanteDB.Core.Interop;
using SanteDB.Rest.Common.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Represents a service contract
    /// </summary>
    [ServiceContract(Name = "BIS")]
    [ServiceKnownResource(typeof(BisPackage))]
    [ServiceKnownResource(typeof(BisDataSourceDefinition))]
    [ServiceKnownResource(typeof(BisParameterDefinition))]
    [ServiceKnownResource(typeof(BisQueryDefinition))]
    [ServiceKnownResource(typeof(BisReportDefinition))]
    [ServiceKnownResource(typeof(BisViewDefinition))]
    [ServiceKnownResource(typeof(BisRenderFormatDefinition))]
    [ServiceProduces("application/json")]
    [ServiceProduces("application/xml")]
    public interface IBisServiceContract
    {

        /// <summary>
        /// Renders the specified BIS view in the specified format
        /// </summary>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [Get("/View/{id}.{format}")]
        Stream RenderView(String id, String format);

        /// <summary>
        /// Executes the specified query 
        /// </summary>
        [Get("/Query/{id}")]
        IEnumerable<dynamic> RenderQuery(String id);

        /// <summary>
        /// Options for this instance of the BIS service
        /// </summary>
        [RestInvoke("OPTIONS", "/")]
        ServiceOptions Options();

        /// <summary>
        /// Pings the service to ensure that it is available on the current endpoint
        /// </summary>
        [RestInvoke("PING", "/")]
        void Ping();
        
        /// <summary>
        /// Searches the specified resource at endpoint
        /// </summary>
        [Get("/{resourceType}")]
        List<BisDefinition> Search(String resourceType);

        /// <summary>
        /// Gets the specified resource at the endpoint
        /// </summary>
        /// <returns></returns>
        [Get("/{resourceType}/{id}")]
        BisDefinition Get(String resourceType, String id);

        /// <summary>
        /// Creates a new instance of a BIS resource
        /// </summary>
        [Post("/{resourceType}")]
        BisDefinition Create(String resourceType, BisDefinition body);

        /// <summary>
        /// Deletes the specified BIS resource
        /// </summary>
        [Delete("/{resourceType}/{id}")]
        BisDefinition Delete(String resourceType, String id);

        /// <summary>
        /// Updates the specified BIS resource
        /// </summary>
        [Put("/{resourceType}/{id}")]
        BisDefinition Update(String resourceType, String id, BisDefinition body);

    }
}
