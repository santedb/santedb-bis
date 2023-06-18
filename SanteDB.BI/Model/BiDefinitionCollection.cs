/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using Newtonsoft.Json;
using SanteDB.BI.Datamart;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a collection of BI definitions
    /// </summary>
    [XmlType(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [XmlInclude(typeof(BiViewDefinition))]
    [XmlInclude(typeof(BiQueryDefinition))]
    [XmlInclude(typeof(BiParameterDefinition))]
    [XmlInclude(typeof(BiReportDefinition))]
    [XmlInclude(typeof(BiRenderFormatDefinition))]
    [XmlInclude(typeof(BiDataFlowDefinition))]
    [XmlInclude(typeof(BiDatamartDefinition))]
    [XmlInclude(typeof(BiDataSourceDefinition))]
    [XmlInclude(typeof(BiPackage))]
    [XmlInclude(typeof(DatamartInfo))]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDefinitionCollection
    {

        /// <summary>
        /// Collection ctor
        /// </summary>
        public BiDefinitionCollection()
        {

        }

        /// <summary>
        /// Collection ctor with objects 
        /// </summary>
        public BiDefinitionCollection(IEnumerable<BiDefinition> items)
        {
            this.Resources = new List<BiDefinition>(items);
            this.Count = this.Resources.Count();
        }

        /// <summary>
        /// Gets or sets the items part of this collection
        /// </summary>
        [XmlElement("resource"), JsonProperty("resource")]
        public List<BiDefinition> Resources { get; set; }

        /// <summary>
        /// Gets or sets the count in this bundle
        /// </summary>
        [XmlElement("offset"), JsonProperty("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the count in this bundle
        /// </summary>
        [XmlElement("count"), JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the total results
        /// </summary>
        [XmlElement("totalResults"), JsonProperty("totalResults")]
        public int? TotalResults { get; set; }
    }
}
