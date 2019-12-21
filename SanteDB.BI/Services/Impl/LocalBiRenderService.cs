/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
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
 * User: Justin Fyfe
 * Date: 2019-12-13
 */
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
    public class LocalBiRenderService : IBiRenderService
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
