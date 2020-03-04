﻿/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a query definition
    /// </summary>
    [XmlType(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiQueryDefinition : BiDefinition
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public BiQueryDefinition()
        {
            this.DataSources = new List<BiDataSourceDefinition>();
            this.Parameters = new List<BiParameterDefinition>();
            this.QueryDefinitions = new List<BiSqlDefinition>();
        }

        /// <summary>
        /// Gets or sets the data sources
        /// </summary>
        [XmlArray("dataSources"), XmlArrayItem("add"), JsonProperty("dataSources")]
        public List<BiDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the parameter for this query
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BiParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the SQL definition
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add"), JsonIgnore]
        public List<BiSqlDefinition> QueryDefinitions { get; set; }

        /// <summary>
        /// Query definitions are only serialized on parse for reading/installation
        /// </summary>
        public bool ShouldSerializeQueryDefinitions() => this.ShouldSerializeDefinitions;

    }
}