﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using SanteDB.BI.Components;
using SanteDB.BI.Configuration;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Data;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// An report renderer which renders its output as HTML
    /// </summary>
    public class HtmlReportRenderer : IBiReportFormatProvider
    {
        private readonly BiConfigurationSection m_configuration;

        /// <summary>
        /// DI constructor
        /// </summary>
        public HtmlReportRenderer(IConfigurationManager configurationManager)
        {
            this.m_configuration = configurationManager.GetSection<BiConfigurationSection>();
        }

        /// <summary>
        /// Render this report as HTML
        /// </summary>
        /// <param name="reportDefinition">The report definition being rendered</param>
        /// <param name="viewName">The name of the view being rendered</param>
        /// <param name="parameters">The parameters to the report</param>
        /// <returns>The report stream in HTML format</returns>
        public Stream Render(BiReportDefinition reportDefinition, string viewName, IDictionary<string, object> parameters)
        {

            foreach (var pol in reportDefinition.MetaData.Demands ?? new List<string>())
            {
                ApplicationServiceContext.Current.GetService<IPolicyEnforcementService>().Demand(pol);
            }

            // Get the view 
            var view = string.IsNullOrEmpty(viewName) ? reportDefinition.Views.First() : reportDefinition.Views.FirstOrDefault(o => o.Name == viewName);
            if (view == null)
            {
                throw new KeyNotFoundException($"Report view {viewName} does not exist in {reportDefinition.Id}");
            }

            // Demand permission to render
            // Start a new root context
            var context = new RootRenderContext(reportDefinition, viewName, parameters, this.m_configuration?.MaxBiResultSetSize);
            var retVal = new TemporaryFileStream();
            using (var xw = XmlWriter.Create(retVal, new XmlWriterSettings()
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
            {
                xw.WriteStartElement("html", BiConstants.HtmlNamespace);

                xw.WriteStartElement("head", BiConstants.HtmlNamespace);
                xw.WriteElementString("title", BiConstants.HtmlNamespace, view.Name);
                xw.WriteEndElement();

                xw.WriteStartElement("body", BiConstants.HtmlNamespace);

                ReportViewUtil.Write(xw, view.Body, context);

                xw.WriteEndElement();

                xw.WriteEndElement(); // html
            }
            retVal.Seek(0, SeekOrigin.Begin);
            return retVal;
        }

    }
}
