using SanteDB.BI.Model;
using SanteDB.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// Rendering service which renders reports locally
    /// </summary>
    public class LocalRenderService : IBiRenderService
    {

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName => "Local BI Rendering Service";

        /// <summary>
        /// Render the specified report
        /// </summary>
        public Stream Render(string reportId, string viewName, string formatName, IDictionary<string, object> parameters, out string mimeType)
        {
            try
            {

                // Get the report format 
                var formatDefinition = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>().Query<BiRenderFormatDefinition>(o => o.FormatExtension == formatName, 0, 1).FirstOrDefault();
                if (formatDefinition == null)
                    throw new KeyNotFoundException($"Report format {formatName} is not registered");

                var reportDefinition = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>().Get<BiReportDefinition>(reportId);
                if (reportDefinition == null)
                    throw new KeyNotFoundException($"Report {reportId} is not registered");

                // Render the report
                var renderer = Activator.CreateInstance(formatDefinition.Type) as IBiReportFormatProvider;

                mimeType = formatDefinition.ContentType;
                return renderer.Render(reportDefinition, viewName, parameters);
            }
            catch (Exception e)
            {
                throw new Exception($"Error rendering BIS report: {reportId}", e);
            }
        }
    }
}
