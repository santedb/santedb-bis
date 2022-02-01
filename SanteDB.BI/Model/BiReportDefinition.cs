﻿/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-27
 */
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a report definition
    /// </summary>
    [XmlType(nameof(BiReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiReportDefinition : BiDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiReportDefinition()
        {
            this.DataSource = new List<BiDefinition>();
            this.Views = new List<BiReportViewDefinition>();
        }

        /// <summary>
        /// Gets or sets the data sources which feed this DIV
        /// </summary>
        [XmlArray("dataSources"),
            XmlArrayItem("query", typeof(BiQueryDefinition)),
            XmlArrayItem("view", typeof(BiViewDefinition)),
            JsonProperty("dataSources")]
        public List<BiDefinition> DataSource { get; set; }

        /// <summary>
        /// Gets or sets the views
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BiReportViewDefinition> Views { get; set; }

    }
}