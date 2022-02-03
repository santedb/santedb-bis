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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an instructed definition for pivoting data
    /// </summary>
    [XmlType(nameof(BiViewPivotDefinition), Namespace = BiConstants.XmlNamespace), JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiViewPivotDefinition
    {
        /// <summary>
        /// Gets or sets the rows
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the columns of the pivots
        /// </summary>
        [XmlAttribute("columnDef"), JsonProperty("columnDef")]
        public string Columns { get; set; }

        /// <summary>
        /// Gets or sets the value of the pivots
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// BI Aggregate function
        /// </summary>
        [XmlAttribute("fn"), JsonProperty("fn")]
        public BiAggregateFunction AggregateFunction { get; set; }

    }
}