using SanteDB.BI.Components;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Security.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// A report renderer which can generate CSV files
    /// </summary>
    public class CsvReportRenderer : XsltReportRendererBase
    {

        /// <summary>
        /// Create new report renderer
        /// </summary>
        public CsvReportRenderer() : base(typeof(CsvReportRenderer).Assembly.GetManifestResourceStream("SanteDB.BI.Resources.csv.xsl"), true)
        { }


    }
}
