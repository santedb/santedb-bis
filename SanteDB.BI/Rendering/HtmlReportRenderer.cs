﻿using SanteDB.BI.Components;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Exceptions;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// An report renderer which renders its output as HTML
    /// </summary>
    public class HtmlReportRenderer : IBiReportRenderer
    {

        /// <summary>
        /// Render this report as HTML
        /// </summary>
        /// <param name="reportDefinition">The report definition being rendered</param>
        /// <param name="viewName">The name of the view being rendered</param>
        /// <param name="parameters">The parameters to the report</param>
        /// <returns>The report stream in HTML format</returns>
        public Stream Render(BiReportDefinition reportDefinition, string viewName, IDictionary<string, object> parameters)
        {

            var pdpService = ApplicationServiceContext.Current.GetService<IPolicyDecisionService>();

            foreach (var pol in reportDefinition.MetaData.Demands ?? new List<string>())
            {
                var outcome = pdpService.GetPolicyOutcome(AuthenticationContext.Current.Principal, pol);
                if (outcome != Core.Model.Security.PolicyGrantType.Grant)
                    throw new PolicyViolationException(AuthenticationContext.Current.Principal, pol, outcome);
            }

            // Get the view 
            var view = reportDefinition.Views.FirstOrDefault(o => o.Name == viewName);
            if (view == null)
                throw new KeyNotFoundException($"Report view {viewName} does not exist in {reportDefinition.Id}");

            // Demand permission to render
            // Start a new root context
            var context = new RootRenderContext(reportDefinition, viewName, parameters);
            var retVal = new MemoryStream();
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