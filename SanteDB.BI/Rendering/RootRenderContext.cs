using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a root rendering context
    /// </summary>
    public class RootRenderContext : IRenderContext
    {

        // The report which is being executed / rendered
        private BiReportDefinition m_report;

        // The data sources
        private Dictionary<String, BisResultContext> m_dataSources;

        // The name of the view being executed
        private string m_viewName;

        /// <summary>
        /// Cretaes a new root rendering context
        /// </summary>
        /// <param name="report">The report being rendered</param>
        /// <param name="viewName">The view being rendered</param>
        /// <param name="parameters">The parameters to the renderer</param>
        public RootRenderContext(BiReportDefinition report, String viewName, IDictionary<String, object> parameters)
        {
            this.Parameters = parameters;
            this.m_viewName = viewName;
            this.m_report = report;
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
            if(!this.m_dataSources.TryGetValue(name, out retVal))
            {
                var dataSourceDefinition = this.m_report.DataSource.FirstOrDefault(o => o.Name == name);
                if (dataSourceDefinition == null)
                    throw new KeyNotFoundException($"Datasource {name} not found");

                if (dataSourceDefinition is BiViewDefinition)
                    retVal = ApplicationServiceContext.Current.GetService<IBiDataSource>().ExecuteView(dataSourceDefinition as BiViewDefinition, this.Parameters, 0, null);
                else if (dataSourceDefinition is BiQueryDefinition)
                    retVal = ApplicationServiceContext.Current.GetService<IBiDataSource>().ExecuteQuery(dataSourceDefinition as BiQueryDefinition, this.Parameters, null, 0, null);
                else
                    throw new InvalidOperationException($"Cannot determine data source type of {name}");

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
        public dynamic ScopedObject => new { Report = this.m_report, View = this.m_viewName };

        /// <summary>
        /// Gets or sets the context parameters for the render
        /// </summary>
        public IDictionary<string, object> Parameters { get; }
    }
}
