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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a definition for a column
    /// </summary>
    [XmlType(nameof(BiSchemaColumnDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaColumnDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the type of column
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BiDataType Type { get; set; }

        /// <summary>
        /// True if this is not null
        /// </summary>
        [XmlAttribute("notNull"), JsonProperty("notNull")]
        public bool NotNull { get; set; }

        /// <summary>
        /// True if this column is indexed
        /// </summary>
        [XmlAttribute("index"), JsonProperty("index")]
        public bool IsIndex { get; set; }

        /// <summary>
        /// True if this column is unique
        /// </summary>
        [XmlAttribute("unique"), JsonProperty("unique")]
        public bool IsUnique { get; set; }

        /// <summary>
        /// True if this object is a key
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets the table that this column referneces (as a foreign key)
        /// </summary>
        [XmlElement("references"), JsonProperty("references")]
        public BiSchemaObjectReference References { get; set; }


    }
}