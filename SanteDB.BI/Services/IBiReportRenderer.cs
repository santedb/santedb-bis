using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a view renderer which can render a particular view given a particular context
    /// </summary>
    public interface IBiReportRenderer
    {

        /// <summary>
        /// Render the specified report accoring to the format
        /// </summary>
        /// <param name="parameters">The parameters used to populate the report</param>
        /// <param name="reportDefinition">The report that should be rendered</param>
        /// <param name="viewName">The name of the view to berendered</param>
        /// <returns>The rendered output stream</returns>
        Stream Render(BiReportDefinition reportDefinition, String viewName, IDictionary<String, Object> parameters);

        
    }
}
