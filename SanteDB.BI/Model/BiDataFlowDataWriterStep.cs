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

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Indicates the writer mode
    /// </summary>
    [XmlType(nameof(DataWriterModeType), Namespace = BiConstants.XmlNamespace)]
    public enum DataWriterModeType
    {
        /// <summary>
        /// Data should be inserted if it doesn't exist, or updated if it does
        /// </summary>
        [XmlEnum("insertUpdate")]
        InsertOrUpdate,
        /// <summary>
        /// Data should only be inserted
        /// </summary>
        [XmlEnum("insert")]
        Insert,
        /// <summary>
        /// Data should be only updated
        /// </summary>
        [XmlEnum("update")]
        Update,
        /// <summary>
        /// Data should be deleted
        /// </summary>
        [XmlEnum("delete")]
        Delete
    }

    /// <summary>
    /// A data writer, which writes data to the destination
    /// </summary>
    [XmlType(nameof(BiDataFlowDataWriterStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowDataWriterStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Gets or sets the connection on this writer
        /// </summary>
        [XmlElement("connection"), JsonProperty("connection")]
        public BiDataFlowConnectionStep Connection { get; set; }

        /// <summary>
        /// When true, the table should be created
        /// </summary>
        [XmlAttribute("create"), JsonProperty("create")]
        public bool CreateTable { get; set; }

        /// <summary>
        /// Truncate the table
        /// </summary>
        [XmlAttribute("truncate"), JsonProperty("truncate")]
        public bool TruncateTable { get; set; }

        /// <summary>
        /// The data insert mode
        /// </summary>
        [XmlAttribute("mode"), JsonProperty("mode")]
        public DataWriterModeType Mode { get; set; }

        /// <summary>
        /// Gets or sets the target of the writer
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public BiSchemaTableDefinition Target { get; set; }
    }
}