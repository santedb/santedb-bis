/*
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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view
    /// </summary>
    [XmlType(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiReportViewDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the body of the element
        /// </summary>
        [XmlElement("div", Namespace = BiConstants.HtmlNamespace), JsonIgnore]
        public XElement Body { get; set; }

        /// <summary>
        /// Gets whether the body should be serialized
        /// </summary>
        public bool ShouldSerializeBody() => this.ShouldSerializeDefinitions;

        /// <summary>
        /// Include body in serialization
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool IncludeBody { get => this.ShouldSerializeDefinitions; set => this.ShouldSerializeDefinitions = value; }
    }
}