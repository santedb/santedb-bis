using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SanteDB.BI.Model;
using SanteDB.Core;
using SanteDB.Core.Applets.Services;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Exceptions;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// Represents a default implementation of a BIS metadata repository which loads definitions from loaded applets
    /// </summary>
    public class AppletMetadataRepository : IBiMetadataRepository
    {

        // Tracer for this repository
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletMetadataRepository));

        /// <summary>
        /// Definition cache 
        /// </summary>
        private Dictionary<Type, Dictionary<String, BiDefinition>> m_definitionCache = new Dictionary<Type, Dictionary<string, BiDefinition>>();

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "Applet Based BIS Repository";

        /// <summary>
        /// Gets the specified object from the specified type repository 
        /// </summary>
        public TBisDefinition Get<TBisDefinition>(string id) where TBisDefinition : BiDefinition
        {
            var pdp = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();
            Dictionary<String, BiDefinition> definitions = null;
            BiDefinition definition = null;

            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions) &&
                definitions.TryGetValue(id, out definition) &&
                ((definition?.MetaData?.Demands?.Count ?? 0) == 0 ||
                definition?.MetaData?.Demands.All(o=>pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, o) == Core.Model.Security.PolicyGrantType.Grant) == true))
                return definition as TBisDefinition;
            return null;
        }

        /// <summary>
        /// Inserts the specified definition into the cache
        /// </summary>
        public TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BiDefinition
        {
            // Demand unrestricted metadata
            var pdp = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();
            var outcome = pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, PermissionPolicyIdentifiers.UnrestrictedMetadata);
            if (outcome != Core.Model.Security.PolicyGrantType.Grant)
                throw new PolicyViolationException(AuthenticationContext.Current.Principal, PermissionPolicyIdentifiers.UnrestrictedMetadata, outcome);

            // Locate type definitions
            Dictionary<String, BiDefinition> typeDefinitions = null;
            if (!this.m_definitionCache.TryGetValue(metadata.GetType(), out typeDefinitions))
            {
                typeDefinitions = new Dictionary<string, BiDefinition>();
                this.m_definitionCache.Add(metadata.GetType(), typeDefinitions);
            }

            // Add the defintiion
            if (typeDefinitions.ContainsKey(metadata.Id))
                typeDefinitions[metadata.Id] = metadata;
            else
                typeDefinitions.Add(metadata.Id, metadata);

            return metadata;
        }

        /// <summary>
        /// Queries the specified definition type
        /// </summary>
        public IEnumerable<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter, int offset, int? count) where TBisDefinition : BiDefinition
        {
            var pdp = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();

            Dictionary<String, BiDefinition> definitions = null;
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions))
                return definitions.Values
                    .OfType<TBisDefinition>()
                    .Where(filter.Compile())
                    .Where(o=>(o.MetaData?.Demands?.Count ?? 0) == 0 || o.MetaData?.Demands?.All(d=>pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, d) == Core.Model.Security.PolicyGrantType.Grant) == true)
                    .Skip(offset)
                    .Take(count ?? 100);
            return new TBisDefinition[0];
        }

        /// <summary>
        /// Remove the specified object from the repository
        /// </summary>
        public void Remove<TBisDefinition>(string id) where TBisDefinition : BiDefinition
        {
            // Demand unrestricted metadata
            var pdp = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();
            var outcome = pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, PermissionPolicyIdentifiers.UnrestrictedMetadata);
            if (outcome != Core.Model.Security.PolicyGrantType.Grant)
                throw new PolicyViolationException(AuthenticationContext.Current.Principal, PermissionPolicyIdentifiers.UnrestrictedMetadata, outcome);

            Dictionary<String, BiDefinition> definitions = null;
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions))
                definitions.Remove(id);
        }

        /// <summary>
        /// Loads all definitions from the applet manager service
        /// </summary>
        private void LoadAllDefinitions()
        {
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            this.m_tracer.TraceInfo("(Re)Loading all BIS Definitions from Applets");

            var appletCollection = ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets;

            var bisDefinitions = appletCollection.SelectMany(o => o.Assets)
                .Where(o => o.Name.StartsWith("bi/") && o.Name.EndsWith(".xml"))
                .Select(o =>
                {
                    try
                    {
                        this.m_tracer.TraceVerbose("Attempting to load {0}", o.Name);
                        using(var ms = new MemoryStream(appletCollection.RenderAssetContent(o)))
                            return BiDefinition.Load(ms);
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceWarning("Could not load BIS Definition: {0} : {1}", o.Name, e);
                        return null;
                    }
                })
                .OfType<BiDefinition>()
                .ToArray();

            // Process contents
            foreach(var itm in bisDefinitions)
                this.ProcessBisDefinition(itm);

        }

        /// <summary>
        /// Process the BIS definition item
        /// </summary>
        private void ProcessBisDefinition(BiDefinition definition)
        {
            if(definition is BiPackage)
            {
                this.m_tracer.TraceInfo("Processing BIS Package: {0}", definition.Id);
                var pkg = definition as BiPackage;
                var objs = pkg.DataSources.OfType<BiDefinition>()
                    .Union(pkg.Formats.OfType<BiDefinition>())
                    .Union(pkg.Parameters.OfType<BiDefinition>())
                    .Union(pkg.Queries.OfType<BiDefinition>())
                    .Union(pkg.Reports.OfType<BiDefinition>())
                    .Union(pkg.Views.OfType<BiDefinition>());

                foreach (var itm in objs)
                {
                    itm.MetaData?.Demands.AddRange(pkg.MetaData?.Demands);
                    this.ProcessBisDefinition(itm);
                }
                this.Insert(pkg);
            }
            else
            {
                this.m_tracer.TraceInfo("Processing BIS Definition: {0}", definition.Id);

                this.Insert(definition);
            }
        }

        /// <summary>
        /// Applet metadata repository
        /// </summary>
        public AppletMetadataRepository()
        {
            ApplicationServiceContext.Current.Started += (o, e) =>
            {
                // Re-scans the loaded applets for definitions when the collection has changed
                ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets.CollectionChanged += (oa, ea) =>
                {
                    this.LoadAllDefinitions();
                };

                this.LoadAllDefinitions();
            };
        }
    }
}
