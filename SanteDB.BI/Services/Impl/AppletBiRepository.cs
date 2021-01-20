/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-12-24
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SanteDB.BI.Model;
using SanteDB.Core;
using SanteDB.Core.Applets;
using SanteDB.Core.Applets.Model;
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
    public class AppletBiRepository : IBiMetadataRepository
    {

        // Tracer for this repository
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletBiRepository));

        /// <summary>
        /// Definition cache 
        /// </summary>
        private Dictionary<Type, Dictionary<String, Object>> m_definitionCache = new Dictionary<Type, Dictionary<string, Object>>();

        // Lock object
        private object m_lockObject = new object();

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "Applet Based BIS Repository";

        /// <summary>
        /// Applet BI Repo is local repo
        /// </summary>
        public bool IsLocal => true;

        /// <summary>
        /// Gets the specified object from the specified type repository 
        /// </summary>
        public TBisDefinition Get<TBisDefinition>(string id) where TBisDefinition : BiDefinition
        {
            var pdp = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();

            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out Dictionary<String, Object> definitions) &&
                definitions.TryGetValue(id, out Object asset))
            {
                if (asset is AppletAsset)
                    using (var ms = new MemoryStream(ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets.RenderAssetContent(asset as AppletAsset)))
                        asset = BiDefinition.Load(ms);

                var definition = asset as BiDefinition;
                if ((definition?.MetaData?.Demands?.Count ?? 0) == 0 ||
                definition?.MetaData?.Demands.All(o => pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, o) == Core.Model.Security.PolicyGrantType.Grant) == true)
                    return (TBisDefinition)definition;
            }
            return null;
        }

        /// <summary>
        /// Inserts the specified definition into the cache
        /// </summary>
        public TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BiDefinition
        {
            // Demand unrestricted metadata
            if (AuthenticationContext.Current.Principal != AuthenticationContext.SystemPrincipal)
                ApplicationServiceContext.Current.GetService<IPolicyEnforcementService>().Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata);

            // Locate type definitions
            if (!this.m_definitionCache.TryGetValue(metadata.GetType(), out Dictionary<String, Object>  typeDefinitions))
            {
                typeDefinitions = new Dictionary<string, Object>();
                this.m_definitionCache.Add(metadata.GetType(), typeDefinitions);
            }

            // Add the defintiion
            if (typeDefinitions.TryGetValue(metadata.Id, out object existing))
            {
                // Can't replace sys object
                if(existing is BiDefinition && !(existing as BiDefinition).IsSystemObject)
                    typeDefinitions[metadata.Id] = metadata;
                else if (existing is AppletAsset && metadata is BiDefinition) // cant downgrade but can upgrade
                    typeDefinitions[metadata.Id] = metadata;
            }
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

            // TODO: If the definition is an applet asset then load it
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out Dictionary<String, Object> definitions))
                return definitions.Values
                    .Select(o =>
                    {
                        if (o is AppletAsset)
                            using (var ms = new MemoryStream(ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets.RenderAssetContent(o as AppletAsset)))
                                return BiDefinition.Load(ms);
                        else
                            return o;
                    })
                    .OfType<TBisDefinition>()
                    .Where(filter.Compile())
                    .Where(o => (o.MetaData?.Demands?.Count ?? 0) == 0 || o.MetaData?.Demands?.All(d => pdp.GetPolicyOutcome(AuthenticationContext.Current.Principal, d) == Core.Model.Security.PolicyGrantType.Grant) == true)
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
            if(AuthenticationContext.Current.Principal != AuthenticationContext.SystemPrincipal)
                ApplicationServiceContext.Current.GetService<IPolicyEnforcementService>().Demand(PermissionPolicyIdentifiers.UnrestrictedMetadata);

            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out Dictionary<String, object> definitions) &&
                definitions.TryGetValue(id, out object existing) &&
                (existing is AppletAsset || (existing as BiDefinition).IsSystemObject))
                definitions.Remove(id);

        }

        /// <summary>
        /// Loads all definitions from the applet manager service
        /// </summary>
        private void LoadAllDefinitions()
        {
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            this.m_tracer.TraceInfo("(Re)Loading all BIS Definitions from Applets");
            this.m_definitionCache.Clear();
            var solutions = ApplicationServiceContext.Current.GetService<IAppletSolutionManagerService>()?.Solutions.ToList();

            // Doesn't have a solution manager
            if (solutions == null)
                this.ProcessApplet(ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets);
            else
            {
                solutions.Add(new Core.Applets.Model.AppletSolution() { Meta = new Core.Applets.Model.AppletInfo() { Id = String.Empty } });
                foreach (var s in solutions)
                {
                    var appletCollection = ApplicationServiceContext.Current.GetService<IAppletSolutionManagerService>().GetApplets(s.Meta.Id);
                    this.ProcessApplet(appletCollection);
                }
            }

        }

        /// <summary>
        /// Process an applet collection
        /// </summary>
        private void ProcessApplet(ReadonlyAppletCollection appletCollection)
        {

            var bisDefinitions = appletCollection.SelectMany(o => o.Assets)
                .Where(o => o.Name.StartsWith("bi/") && o.Name.EndsWith(".xml"))
                .Select(o =>
                {
                    try
                    {
                        this.m_tracer.TraceVerbose("Attempting to load {0}", o.Name);
                        
                        using (var ms = new MemoryStream(appletCollection.RenderAssetContent(o)))
                            return new { Definition = BiDefinition.Load(ms), Asset = o };
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceWarning("Could not load BIS Definition: {0} : {1}", o.Name, e);
                        return null;
                    }
                })
                .OfType<dynamic>()
                .ToArray();


            // Process contents
           foreach (var itm in bisDefinitions)
            {
                this.ProcessBisDefinition(itm.Definition);
#if DEBUG
                if(itm.Definition is BiReportDefinition || itm.Definition is BiViewDefinition || itm.Definition is BiQueryDefinition)
                    this.m_definitionCache[itm.Definition.GetType()][itm.Definition.Id] = itm.Asset;    
#endif
            }
        }

        /// <summary>
        /// Process the BIS definition item
        /// </summary>
        private void ProcessBisDefinition(BiDefinition definition)
        {
            if (definition is BiPackage)
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
        public AppletBiRepository()
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
