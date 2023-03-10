﻿/*
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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view on data in the BI package
    /// </summary>
    [XmlType(nameof(BiViewDefinition), Namespace = BiConstants.XmlNamespace), JsonObject(nameof(BiViewDefinition))]
    [XmlRoot(nameof(BiViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiViewDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the query upon which this view is based
        /// </summary>
        [XmlElement("query"), JsonProperty("query")]
        public BiQueryDefinition Query { get; set; }

        /// <summary>
        /// Gets or sets the aggregation definitions
        /// </summary>
        [XmlArray("aggregations"), XmlArrayItem("add"), JsonProperty("aggregations")]
        public List<BiAggregationDefinition> AggregationDefinitions { get; set; }

        /// <summary>
        /// True if aggregations should be serialized
        /// </summary>
        public bool ShouldSerializeAggregationDefinitions() => this.ShouldSerializeDefinitions;

        /// <summary>
        /// Gets or sets the pivot to perform
        /// </summary>
        [XmlElement("pivot"), JsonProperty("pivot")]
        public BiViewPivotDefinition Pivot { get; set; }

        /// <summary>
        /// True if pivot should be serialized
        /// </summary>
        public bool ShouldSerializePivot() => this.ShouldSerializeDefinitions;


    }
}