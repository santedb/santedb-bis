/*
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
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a root rendering context
    /// </summary>
    public class RootRenderContext : IRenderContext
    {

        // The report which is being executed / rendered
        private BiReportDefinition m_report;

        // Maximum size of the render context
        private int? m_maxResultSetSize;

        // The data sources
        private Dictionary<String, BisResultContext> m_dataSources = new Dictionary<string, BisResultContext>();

        // The name of the view being executed
        private string m_viewName;

        // Scoped object
        private ExpandoObject m_scopedObject;

        /// <summary>
        /// Cretaes a new root rendering context
        /// </summary>
        /// <param name="report">The report being rendered</param>
        /// <param name="viewName">The view being rendered</param>
        /// <param name="maxResultSetSize">The maximum number of results which can be present in the BI result set</param>
        /// <param name="parameters">The parameters to the renderer</param>
        public RootRenderContext(BiReportDefinition report, String viewName, IDictionary<String, object> parameters, int? maxResultSetSize)
        {
            this.Parameters = parameters;
            this.m_viewName = viewName;
            this.m_report = report;
            this.Tags = new Dictionary<String, Object>();
            this.m_maxResultSetSize = maxResultSetSize;
        }

        /// <summary>
        /// Returns true if the specified report has the specified data source
        /// </summary>
        /// <param name="name">The name of the data source</param>
        /// <returns>True if the report has the data source</returns>
        public bool HasDataSource(string name)
        {
            return this.m_report.DataSource.Any(o => o.Name == name);
        }

        /// <summary>
        /// Gets or executes teh result context
        /// </summary>
        public BisResultContext GetOrExecuteQuery(string name)
        {

            BisResultContext retVal = null;
            if (!this.m_dataSources.TryGetValue(name, out retVal))
            {

                var viewDef = this.m_report.DataSource.FirstOrDefault(o => o.Name == name);
                if (viewDef == null)
                {
                    throw new KeyNotFoundException($"Datasource {name} not found");
                }

                viewDef = BiUtils.ResolveRefs(viewDef);

                // Get the datasource for the execution engine
                var dsource = (viewDef as BiViewDefinition)?.Query?.DataSources.FirstOrDefault(o => o.Name == "main") ?? (viewDef as BiViewDefinition)?.Query?.DataSources.FirstOrDefault() ??
                    (viewDef as BiQueryDefinition)?.DataSources.FirstOrDefault(o => o.Name == "main") ?? (viewDef as BiQueryDefinition)?.DataSources.FirstOrDefault();

                IBiDataSource providerImplementation = null;
                if (dsource.ProviderType != null)
                {
                    providerImplementation = ApplicationServiceContext.Current.GetService<IServiceManager>().CreateInjected(dsource.ProviderType) as IBiDataSource;
                }
                else
                {
                    providerImplementation = ApplicationServiceContext.Current.GetService<IBiDataSource>(); // Global default
                }

                // Load from cache instead of DB?
                var cacheService = ApplicationServiceContext.Current.GetService<IAdhocCacheService>();
                var key = $"{name}?{String.Join("&", this.Parameters.Select(o => $"{o.Key}={o.Value}"))}";
                var cacheResult = cacheService?.Get<IEnumerable<dynamic>>(key);

                int count = 10000;
                if (this.m_maxResultSetSize.HasValue)
                {
                    count = this.m_maxResultSetSize.Value;
                }
                else if (this.Parameters.TryGetValue("_count", out var parameterCount) && Int32.TryParse(parameterCount.ToString(), out var tCount))
                {
                    count = tCount;
                }

                if (cacheResult != null)
                {
                    return new BisResultContext(null, this.Parameters, providerImplementation, cacheResult, DateTime.Now);
                }
                else if (viewDef is BiViewDefinition)
                {
                    retVal = providerImplementation.ExecuteView(viewDef as BiViewDefinition, this.Parameters, 0, count);
                }
                else if (viewDef is BiQueryDefinition)
                {
                    retVal = providerImplementation.ExecuteQuery(viewDef as BiQueryDefinition, this.Parameters, null, 0, count);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot determine data source type of {name}");
                }

                cacheService?.Add(key, retVal.Dataset, new TimeSpan(0, 1, 0));
                this.m_dataSources.Add(name, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Gets the paren of this context
        /// </summary>
        public IRenderContext Parent => null;

        /// <summary>
        /// Gets the root
        /// </summary>
        public IRenderContext Root => this;

        /// <summary>
        /// The scoped object is the report
        /// </summary>
        public dynamic ScopedObject
        {
            get
            {
                if (this.m_scopedObject == null)
                {
                    this.m_scopedObject = new ExpandoObject();
                    (this.m_scopedObject as IDictionary<String, Object>).Add("Report", this.m_report);
                    (this.m_scopedObject as IDictionary<String, Object>).Add("View", this.m_viewName);
                    (this.m_scopedObject as IDictionary<String, Object>).Add("Parameters", this.Parameters);
                    Func<String, BisResultContext> getOrCreateQuery = (qname) => this.GetOrExecuteQuery(qname);
                    (this.m_scopedObject as IDictionary<String, Object>).Add("DataSource", getOrCreateQuery);
                }
                return this.m_scopedObject;
            }
        }


        /// <summary>
        /// Gets or sets the context parameters for the render
        /// </summary>
        public IDictionary<string, object> Parameters { get; }

        /// <summary>
        /// Report watches for this instance of the report
        /// </summary>
        public IDictionary<String, Object> Tags { get; }
    }
}
