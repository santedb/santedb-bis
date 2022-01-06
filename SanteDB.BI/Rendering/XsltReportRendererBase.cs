/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using SanteDB.BI.Components;
using SanteDB.BI.Configuration;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// A report renderer which can generate CSV files
    /// </summary>
    public abstract class XsltReportRendererBase : IBiReportFormatProvider
    {
        private readonly BiConfigurationSection m_configuration;

        /// <summary>
        /// DI constructor
        /// </summary>
        public XsltReportRendererBase(IConfigurationManager configurationManager)
        {
            this.m_configuration = configurationManager.GetSection<BiConfigurationSection>();
        }

        // XSL
        private XslCompiledTransform m_xsl;

        // True if output is text
        private bool m_isText = false;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(CsvReportRenderer));

        /// <summary>
        /// Rendering base
        /// </summary>
        public XsltReportRendererBase(Stream xslStream, bool emitPlainText)
        {
            this.m_xsl = new XslCompiledTransform(false);
            using (var xr = XmlReader.Create(xslStream))
                this.m_xsl.Load(xr);
            this.m_isText = emitPlainText;
        }

        /// <summary>
        /// Render the specified report definition with the specified view
        /// </summary>
        public virtual Stream Render(BiReportDefinition reportDefinition, string viewName, IDictionary<string, object> parameters)
        {
            foreach (var pol in reportDefinition.MetaData.Demands ?? new List<string>())
                ApplicationServiceContext.Current.GetService<IPolicyEnforcementService>().Demand(pol);

            // Get the view 
            var view = string.IsNullOrEmpty(viewName) ? reportDefinition.Views.First() : reportDefinition.Views.FirstOrDefault(o => o.Name == viewName);
            if (view == null)
                throw new KeyNotFoundException($"Report view {viewName} does not exist in {reportDefinition.Id}");

            // Demand permission to render
            // Start a new root context
            var context = new RootRenderContext(reportDefinition, viewName, parameters, this.m_configuration?.MaxBiResultSetSize);
            try
            {
                using (var tempMs = new MemoryStream())
                {
                    using (var xw = XmlWriter.Create(tempMs, new XmlWriterSettings()
                    {
                        CloseOutput = false,
#if DEBUG
                        Indent = true,
                        NewLineOnAttributes = true
#else
                Indent = false,
                OmitXmlDeclaration = true
#endif
                    }))
                        ReportViewUtil.Write(xw, view.Body, context);

                    tempMs.Seek(0, SeekOrigin.Begin);
                    var retVal = new MemoryStream();

                    XsltArgumentList args = new XsltArgumentList();
                    args.AddParam("current-date", String.Empty, DateTime.Now.ToString("o"));
                    using (var xr = XmlReader.Create(tempMs))
                    {
                        if (this.m_isText)
                            using (var xw = new StreamWriter(retVal, Encoding.UTF8, 1024, true))
                                this.m_xsl.Transform(xr, null, xw);
                        else
                            using (var xw = XmlWriter.Create(retVal, new XmlWriterSettings()
                            {
                                Indent = true,
                                OmitXmlDeclaration = false
                            }))
                                this.m_xsl.Transform(xr, xw);
                    }
                    retVal.Seek(0, SeekOrigin.Begin);

                    return retVal;
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not export report {0} view {1} - {2}", reportDefinition.Id, viewName, e);
                throw new BiException($"Could not export report view to CSV {view}", reportDefinition, e);
            }
        }
    }
}
