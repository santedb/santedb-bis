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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a connection
    /// </summary>
    [XmlType(nameof(BiDataFlowConnectionStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowConnectionStep : BiDataFlowStep
    {

        /// <summary>
        /// Gets or sets the data source
        /// </summary>
        [XmlElement("dataSource"), JsonProperty("dataSource")]
        public BiDataSourceDefinition DataSource { get; set; }

        /// <summary>
        /// Gets or sets the transaction control (if true run in transaction)
        /// </summary>
        [XmlAttribute("transaction"), JsonProperty("transaction")]
        public bool RunInTransaction { get; set; }
    }
}