/*
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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Enumeration for the data types 
    /// </summary>
    [XmlType(nameof(BisParameterDataType), Namespace = BiConstants.XmlNamespace)]
    public enum BisParameterDataType
    {
        [XmlEnum("ref")]
        Ref,
        [XmlEnum("uuid")]
        Uuid,
        [XmlEnum("string")]
        String,
        [XmlEnum("int")]
        Integer,
        [XmlEnum("bool")]
        Boolean,
        [XmlEnum("date")]
        Date,
        [XmlEnum("date-time")]
        DateTime
    }

    /// <summary>
    /// Represents a parameter definition
    /// </summary>
    [XmlType(nameof(BiParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiParameterDefinition : BiDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiParameterDefinition()
        {
        }

        /// <summary>
        /// Gets or sets the type of parameter
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BisParameterDataType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set the min value
        /// </summary>
        [XmlAttribute("min"), JsonProperty("min")]
        public string MinValue { get; set; }

        /// <summary>
        /// Get or sets the max value
        /// </summary>
        [XmlAttribute("max"), JsonProperty("max")]
        public string MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the values for the parameter
        /// </summary>
        [XmlElement("query", typeof(BiQueryDefinition)),
         XmlElement("values", typeof(BiParameterValueCollection)),
         JsonProperty("values")]
        public Object Values { get; set; }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        [XmlAttribute("default"), JsonProperty("default")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Required value
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public string RequiredXml
        {
            get => this.Required?.ToString().ToLower();
            set => this.Required = string.IsNullOrEmpty(value) ? false : bool.Parse(value);
        }

        /// <summary>
        /// Required parameter?
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool? Required { get; set; }
    }
}