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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a simple parameter value
    /// </summary>
    [XmlType(nameof(BisParameterValue), Namespace = BiConstants.XmlNamespace)]
    public class BisParameterValue
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Label value
        /// </summary>
        [XmlText, JsonProperty("text")]
        public string Label { get; set; }

    }

    /// <summary>
    /// Represents a collection of simple value elements
    /// </summary>
    [XmlType(nameof(BiParameterValueCollection), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiParameterValueCollection
    {
        /// <summary>
        /// Add parameter value
        /// </summary>
        [XmlElement("add"), JsonProperty("list")]
        public List<BisParameterValue> Values { get; set; }

    }
}