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
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a BI package
    /// </summary>
    [XmlRoot(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
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
                    (BiDefinition)this.RefSets.FirstOrDefault(o => o.Id == id) ??
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
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("refSets"), XmlArrayItem("add"), JsonProperty("refSets")]
        public List<BiReferenceDataSourceDefinition> RefSets { get; set; }

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

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
            foreach (var itm in this.DataSources.OfType<BiDefinition>().Union(this.Formats)
                .Union(this.Parameters).Union(this.Queries).Union(this.Views).Union(this.Reports).SelectMany(o => o.Validate(true)))
            {
                yield return itm;
            }
        }

    }
}
