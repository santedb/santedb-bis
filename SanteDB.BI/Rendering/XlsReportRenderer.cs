﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
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
    public class XlsReportRenderer : XsltReportRendererBase
    {

        /// <summary>
        /// Create new CSV renderer
        /// </summary>
        public XlsReportRenderer() : base(typeof(CsvReportRenderer).Assembly.GetManifestResourceStream("SanteDB.BI.Resources.excel-xml.xsl"), false)
        {
        }

    }
}
