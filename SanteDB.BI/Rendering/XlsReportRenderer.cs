/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using ClosedXML.Excel;
using SanteDB.BI.Components;
using SanteDB.BI.Configuration;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Data;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Excel Report Renderer
    /// </summary>
    public class XlsReportRenderer : IBiReportFormatProvider
    {
        private readonly BiConfigurationSection m_configuration;
        private readonly IPolicyEnforcementService m_policyEnforcementService;
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(XlsReportRenderer));

        /// <summary>
        /// DI constructor
        /// </summary>
        public XlsReportRenderer(IConfigurationManager configurationManager, IPolicyEnforcementService policyEnforcementService)
        {
            this.m_configuration = configurationManager.GetSection<BiConfigurationSection>();
            this.m_policyEnforcementService = policyEnforcementService;
        }

        /// <summary>
        /// Render the report
        /// </summary>
        public Stream Render(BiReportDefinition reportDefinition, string viewName, IDictionary<string, object> parameters)
        {
            foreach (var pol in reportDefinition.MetaData.Demands ?? new List<string>())
            {
                this.m_policyEnforcementService.Demand(pol);
            }

            // Get the view
            var view = string.IsNullOrEmpty(viewName) ? reportDefinition.Views.First() : reportDefinition.Views.FirstOrDefault(o => o.Name == viewName);
            if (view == null)
            {
                throw new KeyNotFoundException($"Report view {viewName} does not exist in {reportDefinition.Id}");
            }

            // Start a new root context
            var context = new RootRenderContext(reportDefinition, viewName, parameters, this.m_configuration?.MaxBiResultSetSize);
            try
            {
                using (var tmpStream = new TemporaryFileStream())
                {

                    ReportViewUtil.Write(tmpStream, view.Body, context);
                    tmpStream.Seek(0, SeekOrigin.Begin);
                    using (var xr = XmlReader.Create(tmpStream))
                    {
                        using (var exportExcel = new XLWorkbook())
                        {
                            bool nextCell = false, nextRow = false;
                            IXLWorksheet currentWorksheet = null;
                            while (xr.Read())
                            {
                                switch (xr.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        switch (xr.LocalName)
                                        {
                                            case "table":
                                                currentWorksheet = exportExcel.AddWorksheet();
                                                currentWorksheet.ActiveCell = currentWorksheet.FirstCell();
                                                break;
                                            case "tr":
                                                if (!nextRow)
                                                {
                                                    nextRow = true;
                                                }
                                                else
                                                {
                                                    currentWorksheet.ActiveCell = currentWorksheet.ActiveCell.CellBelow().WorksheetRow().FirstCell();
                                                }
                                                nextCell = false;
                                                break;
                                            case "td":
                                            case "th":

                                                if (!nextCell)
                                                {
                                                    nextCell = true;
                                                }
                                                else
                                                {
                                                    if (currentWorksheet.ActiveCell.IsMerged())
                                                    {
                                                        currentWorksheet.ActiveCell = currentWorksheet.ActiveCell.MergedRange().LastCell().CellRight();
                                                    }
                                                    else
                                                    {
                                                        currentWorksheet.ActiveCell = currentWorksheet.ActiveCell.CellRight();
                                                    }
                                                }
                                                if (Int32.TryParse(xr.GetAttribute("colspan"), out var colspan))
                                                {
                                                    currentWorksheet.Range(currentWorksheet.ActiveCell.Address, currentWorksheet.Cell(currentWorksheet.ActiveCell.Address.RowNumber, currentWorksheet.ActiveCell.Address.ColumnNumber + colspan - 1).Address).Merge();
                                                }

                                                if (xr.LocalName == "th")
                                                {
                                                    currentWorksheet.ActiveCell.Style.Font.SetBold();
                                                }
                                                break;
                                        }
                                        break;
                                    case XmlNodeType.Text:
                                        if (currentWorksheet?.ActiveCell != null)
                                        {
                                            currentWorksheet.ActiveCell.Value += xr.Value.Trim();
                                        }
                                        break;
                                }
                            }
                            var ms = new TemporaryFileStream();
                            exportExcel.SaveAs(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            return ms;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not export report {0} view {1} - {2}", reportDefinition.Id, viewName, e);
                throw new BiException($"Could not export report view to XLS {view}", reportDefinition, e);
            }
        }
    }
}
