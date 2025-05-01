/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2025-1-10
 */
using Newtonsoft.Json;
using SanteDB.Core.Model.Serialization;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A reference to a type which is in a field
    /// </summary>
    [XmlType(nameof(BiIndicatorSubjectFieldRef), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorSubjectFieldRef : BiSqlColumnReference
    {
        // Serialization binder
        private static readonly ModelSerializationBinder m_serializationBinder = new ModelSerializationBinder();

        /// <summary>
        /// Resource type XML
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public string ResourceTypeXml { get; set; }

        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        [XmlAttribute("parameter"), JsonProperty("parameter")]
        public String ParameterName { get; set; }

        /// <summary>
        /// Gets the resource type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ResourceType
        {
            get => m_serializationBinder.BindToType(null, this.ResourceTypeXml);
            set {
                if(value == null)
                {
                    this.ResourceTypeXml = null;
                }
                else
                {
                    m_serializationBinder.BindToName(value, out var asm, out var type);
                    this.ResourceTypeXml = type;
                }
            }
        }
    }
}