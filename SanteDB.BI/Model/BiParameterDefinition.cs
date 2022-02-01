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
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Enumeration for the data types 
    /// </summary>
    [XmlType(nameof(BiDataType), Namespace = BiConstants.XmlNamespace)]
    public enum BiDataType
    {
        /// <summary>
        /// The data type is a reference to another object
        /// </summary>
        [XmlEnum("ref")]
        Ref,
        /// <summary>
        /// The type of data is a universally unique identifier
        /// </summary>
        [XmlEnum("uuid")]
        Uuid,
        /// <summary>
        /// The type of data is a string
        /// </summary>
        [XmlEnum("string")]
        String,
        /// <summary>
        /// The type of data is a signed integer
        /// </summary>
        [XmlEnum("int")]
        Integer,
        /// <summary>
        /// The type of data is a boolean
        /// </summary>
        [XmlEnum("bool")]
        Boolean,
        /// <summary>
        /// The type of data is a simple date
        /// </summary>
        [XmlEnum("date")]
        Date,
        /// <summary>
        /// The type of data is a date with a time
        /// </summary>
        [XmlEnum("date-time")]
        DateTime
    }

    /// <summary>
    /// Represents a parameter definition
    /// </summary>
    [XmlType(nameof(BiParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
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
        public BiDataType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Allow the entry of multiple entries
        /// </summary>
        [XmlAttribute("multiple"), JsonProperty("multiple")]
        public bool Multiple { get; set; }

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