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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a single data flow which is an atomic operation which modifies data
    /// </summary>
    [XmlType(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiDataFlowDefinition : BiDefinition
    {

        /// <summary>
        /// True if this data flow can be called directly via API
        /// </summary>
        [JsonProperty("public"), XmlAttribute("public")]
        public bool IsPbulic { get; set; }

        /// <summary>
        /// XmlElement
        /// </summary>
        [XmlElement("call", typeof(BiDataFlowCallStep))]
        [XmlElement("reader", typeof(BiDataFlowDataReaderStep))]
        [XmlElement("writer", typeof(BiDataFlowDataWriterStep))]
        [XmlElement("connection", typeof(BiDataFlowConnectionStep))]
        [XmlElement("map", typeof(BiDataFlowMappingStep))]
        [XmlElement("pivot", typeof(BiDataFlowPivotStep))]
        [XmlElement("log", typeof(BiDataFlowLogStep))]
        [JsonProperty("step")]
        public List<BiDataFlowStep> Steps { get; set; }
    }
}