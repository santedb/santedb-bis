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
using SanteDB.Core.Services;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// Represents a default implementation of a BIS metadata repository which loads definitions from loaded applets
    /// </summary>
    public class AppletMetadataRepository : IBisMetadataRepository, IDaemonService
    {

        // Tracer for this repository
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletMetadataRepository));

        /// <summary>
        /// Definition cache 
        /// </summary>
        private Dictionary<Type, Dictionary<String, BisDefinition>> m_definitionCache;

        /// <summary>
        /// True when the service is running
        /// </summary>
        public bool IsRunning => this.m_definitionCache != null;

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "Applet Based BIS Repository";

        /// <summary>
        /// Fired when the service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Fired when the service has stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the specified object from the specified type repository 
        /// </summary>
        public TBisDefinition Get<TBisDefinition>(string id) where TBisDefinition : BisDefinition
        {
            Dictionary<String, BisDefinition> definitions = null;
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions))
                return definitions[id] as TBisDefinition;
            return null;
        }

        /// <summary>
        /// Inserts the specified definition into the cache
        /// </summary>
        public TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BisDefinition
        {
            // Locate type definitions
            Dictionary<String, BisDefinition> typeDefinitions = null;
            if (!this.m_definitionCache.TryGetValue(metadata.GetType(), out typeDefinitions))
            {
                typeDefinitions = new Dictionary<string, BisDefinition>();
                this.m_definitionCache.Add(metadata.GetType(), typeDefinitions);
            }

            // Add the defintiion
            if (typeDefinitions.ContainsKey(metadata.Id))
                throw new InvalidOperationException($"Definition {metadata.Id} of type {metadata.GetType().Name} already exists!");
            else
                typeDefinitions.Add(metadata.Id, metadata);

            return metadata;
        }

        /// <summary>
        /// Queries the specified definition type
        /// </summary>
        public IEnumerable<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter, int offset, int? count) where TBisDefinition : BisDefinition
        {
            Dictionary<String, BisDefinition> definitions = null;
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions))
                definitions.Values.OfType<TBisDefinition>().Where(filter.Compile()).Skip(offset).Take(count ?? 100);
            return new TBisDefinition[0];
        }

        /// <summary>
        /// Remove the specified object from the repository
        /// </summary>
        public void Remove<TBisDefinition>(string id) where TBisDefinition : BisDefinition
        {
            Dictionary<String, BisDefinition> definitions = null;
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out definitions))
                definitions.Remove(id);
        }

        /// <summary>
        /// Loads all definitions from the applet manager service
        /// </summary>
        private void LoadAllDefinitions()
        {
            this.m_tracer.TraceInfo("(Re)Loading all BIS Definitions from Applets");
            this.m_definitionCache?.Clear();
            this.m_definitionCache = new Dictionary<Type, Dictionary<string, BisDefinition>>();

            var appletCollection = ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets;

            var bisDefinitions = appletCollection.SelectMany(o => o.Assets)
                .Where(o => o.Name.StartsWith("bi/") && o.Name.EndsWith(".xml"))
                .Select(o =>
                {
                    try
                    {
                        this.m_tracer.TraceVerbose("Attempting to load {0}", o.Name);
                        using(var ms = new MemoryStream(appletCollection.RenderAssetContent(o)))
                            return BisDefinition.Load(ms);
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceWarning("Could not load BIS Definition: {0} : {1}", o.Name, e);
                        return null;
                    }
                })
                .OfType<BisDefinition>()
                .ToArray();

            // Process contents
            foreach(var itm in bisDefinitions)
                this.ProcessBisDefinition(itm);

        }

        /// <summary>
        /// Process the BIS definition item
        /// </summary>
        private void ProcessBisDefinition(BisDefinition definition)
        {
            if(definition is BisPackage)
            {
                this.m_tracer.TraceInfo("Processing BIS Package: {0}", definition.Id);
                var pkg = definition as BisPackage;
                var objs = pkg.DataSources.OfType<BisDefinition>()
                    .Union(pkg.Formats.OfType<BisDefinition>())
                    .Union(pkg.Parameters.OfType<BisDefinition>())
                    .Union(pkg.Queries.OfType<BisDefinition>())
                    .Union(pkg.Reports.OfType<BisDefinition>())
                    .Union(pkg.Views.OfType<BisDefinition>());

                foreach (var itm in objs)
                {
                    itm.Demands.AddRange(pkg.Demands);
                    this.ProcessBisDefinition(itm);
                }
            }
            else
            {
                this.m_tracer.TraceInfo("Processing BIS Definition: {0}", definition.Id);

                this.Insert(definition);
            }
        }

        /// <summary>
        /// Start the service 
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Re-scans the loaded applets for definitions when the collection has changed
            ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets.CollectionChanged += (o, e) =>
            {
                this.LoadAllDefinitions();
            };

            this.LoadAllDefinitions();
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            this.m_definitionCache.Clear();
            this.m_definitionCache = null;

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
