using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SanteDB.BI.Test
{

    /// <summary>
    /// A BI repository that reads from the test assembly
    /// </summary>
    public class TestBiRepository : IBiMetadataRepository
    {
        // Definition cache
        private Dictionary<Type, Dictionary<String, Object>> m_definitionCache = new Dictionary<Type, Dictionary<string, Object>>();

        /// <inheritdoc/>
        public bool IsLocal => true;

        /// <inheritdoc/>
        public string ServiceName => "BI Repository";

        /// <summary>
        /// Constructor
        /// </summary>
        public TestBiRepository()
        {
            var asm = typeof(TestBiRepository).Assembly;
            foreach (var erName in asm.GetManifestResourceNames().Where(o => o.Contains("SanteDB.BI.Test.Bi")))
            {
                using (var str = asm.GetManifestResourceStream(erName))
                {
                    var defCache = BiDefinition.Load(str);
                    if (!m_definitionCache.TryGetValue(defCache.GetType(), out var localCache))
                    {
                        localCache = new Dictionary<string, object>();
                        m_definitionCache.Add(defCache.GetType(), localCache);
                    }
                    localCache.Add(defCache.Id, defCache);
                }
            }
        }

        /// <inheritdoc/>
        public TBisDefinition Get<TBisDefinition>(string id) where TBisDefinition : BiDefinition
        {
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out var itms))
            {
                if (itms.TryGetValue(id, out var retVal))
                {
                    return retVal as TBisDefinition;
                }

            }
            return default(TBisDefinition);
        }

        /// <inheritdoc/>
        public TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BiDefinition
        {
            if (!this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out var items))
            {
                items = new Dictionary<string, object>();
                this.m_definitionCache.Add(typeof(TBisDefinition), items);
            }
            items.Remove(metadata.Id);
            items.Add(metadata.Id, metadata);
            return metadata;
        }

        /// <inheritdoc/>
        public IEnumerable<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter, int offset, int? count) where TBisDefinition : BiDefinition
            => this.Query(filter).Skip(offset).Take(count ?? 100);

        /// <inheritdoc/>
        public IQueryResultSet<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter) where TBisDefinition : BiDefinition
        {
            if (this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out var defs))
            {
                return new MemoryQueryResultSet<TBisDefinition>(defs.Values.OfType<TBisDefinition>().Where(filter.Compile()));
            }
            return new MemoryQueryResultSet<TBisDefinition>(new TBisDefinition[0]);
        }

        /// <inheritdoc/>
        public void Remove<TBisDefinition>(string id) where TBisDefinition : BiDefinition
        {
            if (!this.m_definitionCache.TryGetValue(typeof(TBisDefinition), out var items))
            {
                items = new Dictionary<string, object>();
                this.m_definitionCache.Add(typeof(TBisDefinition), items);
            }
            if (items.ContainsKey(id))
            {
                items.Remove(id);
            }
        }
    }
}
