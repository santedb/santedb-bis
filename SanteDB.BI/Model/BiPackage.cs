/*
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
using Newtonsoft.Json;
using SanteDB.BI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a BI package
    /// </summary>
    [XmlRoot(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiPackage : BiDefinition
    {

        /// <summary>
        /// Gets the specified definition from this package regardless of type
        /// </summary>
        public BiDefinition this[string id]
        {
            get
            {
                return (BiDefinition)this.DataSources.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Formats.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Parameters.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Queries.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Reports.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Views.FirstOrDefault(o => o.Id == id);
            }
        }

        /// <summary>
        /// Constructor for bi package
        /// </summary>
        public BiPackage()
        {
            this.DataSources = new List<BiDataSourceDefinition>();
            this.Formats = new List<BiRenderFormatDefinition>();
            this.Parameters = new List<BiParameterDefinition>();
            this.Queries = new List<BiQueryDefinition>();
            this.Views = new List<BiViewDefinition>();
            this.Reports = new List<BiReportDefinition>();
        }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("sources"), XmlArrayItem("add"), JsonProperty("sources")]
        public List<BiDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("formats"), XmlArrayItem("add"), JsonProperty("formats")]
        public List<BiRenderFormatDefinition> Formats { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter definitions
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BiParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the list of query definitions
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add"), JsonProperty("queries")]
        public List<BiQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the list of view defintiions
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BiViewDefinition> Views { get; set; }

        /// <summary>
        /// Gets or set sthe list of report definitions
        /// </summary>
        [XmlArray("reports"), XmlArrayItem("add"), JsonProperty("reports")]
        public List<BiReportDefinition> Reports { get; set; }

        /// <summary>
        /// True if the definitions should be serialized
        /// </summary>
        internal override bool ShouldSerializeDefinitions {
            get => base.ShouldSerializeDefinitions;
            set
            {
                base.ShouldSerializeDefinitions = value;
                foreach (var itm in new BiPackageEnumerator(this))
                    itm.ShouldSerializeDefinitions = value;
            }
        }

    }
}
