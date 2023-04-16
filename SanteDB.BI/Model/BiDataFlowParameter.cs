/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Data Call Parameter
    /// </summary>
    [XmlType(nameof(BiDataFlowParameterBase), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public abstract class BiDataFlowParameterBase
    {

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

    }


    /// <summary>
    /// Call parameter
    /// </summary>
    [XmlType(Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiDataFlowParameter<T> : BiDataFlowParameterBase
    {

        /// <inheritdoc/>
        public override string ToString() => $"{this.Name}: {typeof(T).Name}";
    }

    /// <summary>
    /// Argument base
    /// </summary>
    [XmlType(nameof(BiDataFlowArgumentBase), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public abstract class BiDataFlowArgumentBase : BiDataFlowParameterBase
    {

        /// <summary>
        /// Get the value of the argument
        /// </summary>
        internal abstract object SimpleValue { get; }

        /// <inheritdoc/>
        public override string ToString() => $"{this.Name}: {this.SimpleValue}";
    }

    /// <summary>
    /// Call parameter
    /// </summary>
    [XmlType(Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiDataFlowCallArgument<T> : BiDataFlowArgumentBase
    {

        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public T Value { get; set; }

        /// <summary>
        /// Get the simple value
        /// </summary>
        [JsonIgnore, XmlIgnore]
        internal override object SimpleValue => this.Value;

    }
}