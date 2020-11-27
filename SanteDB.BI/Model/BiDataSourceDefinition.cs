/*
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
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BIS datasource definition
    /// </summary>
    [XmlType(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataSourceDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the instance of the provider
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public String ProviderTypeXml {
            get => this.ProviderType?.AssemblyQualifiedName;
            set
            {
                if (value != null)
                    this.ProviderType = Type.GetType(value);
                else
                    this.ProviderType = null;
            }
        }

        /// <summary>
        /// Gets or sets the C# type of the provider
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        [XmlAttribute("connectionString"), JsonProperty("connectionString")]
        public String ConnectionString { get; set; }

    }
}