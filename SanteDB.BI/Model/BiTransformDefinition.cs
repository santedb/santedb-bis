/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BI Extract Transform Load
    /// </summary>
    /// <remarks>
    /// The SanteDB BI plugin allows for very basic transforms which can run on the server and on mobile among
    /// the different database systems.
    /// </remarks>
    [XmlType(nameof(BiTransformDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiTransformDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlInclude(typeof(BiSchemaTableDefinition))]
    [JsonObject]
    public class BiTransformDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the schema definitions for the specified transform definition
        /// </summary>
        [XmlArray("schema"),
            XmlArrayItem("table", typeof(BiSchemaTableDefinition)), 
            XmlArrayItem("view", typeof(BiSchemaViewDefinition)), 
            JsonProperty("schemas")]
        public List<BiSchemaObjectDefinition> Schemas { get; set; }

        /// <summary>
        /// Gets or sets the data flow definitions on this definition
        /// </summary>
        [XmlArray("dataFlows"), XmlArrayItem("flow"), JsonProperty("dataFlows")]
        public List<BiDataFlowDefinition> DataFlows { get; set; }

    }
}
