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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    
    /// <summary>
    /// Represents Aggregate functions
    /// </summary>
    [XmlType(nameof(BiAggregateFunction), Namespace = BiConstants.XmlNamespace)]
    public enum BiAggregateFunction
    {
        /// <summary>
        /// No aggregation, just use the first value
        /// </summary>
        [XmlEnum("value")]
        Value = 0,
        /// <summary>
        /// The minimum value
        /// </summary>
        [XmlEnum("min")]
        Min,
        /// <summary>
        /// The maximum value
        /// </summary>
        [XmlEnum("max")]
        Max,
        /// <summary>
        /// The first value
        /// </summary>
        [XmlEnum("first")]
        First,
        /// <summary>
        /// The last value
        /// </summary>
        [XmlEnum("last")]
        Last,
        /// <summary>
        /// The count of total items
        /// </summary>
        [XmlEnum("count")]
        Count,
        /// <summary>
        /// The count of distinct items
        /// </summary>
        [XmlEnum("count-distinct")]
        CountDistinct,
        /// <summary>
        /// The average
        /// </summary>
        [XmlEnum("avg")]
        Average,
        /// <summary>
        /// The sum
        /// </summary>
        [XmlEnum("sum")]
        Sum,
        /// <summary>
        /// The median
        /// </summary>
        [XmlEnum("median")]
        Median
    }

    /// <summary>
    /// Represents the BI SQL Column Reference
    /// </summary>
    [XmlType(nameof(BiSqlColumnReference), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiSqlColumnReference
    {
        /// <summary>
        /// Gets or sets the name of the column
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selection text for the column
        /// </summary>
        [XmlText, JsonProperty("selector")]
        public string ColumnSelector { get; set; }
    }


    /// <summary>
    /// Represents a BI SQL Column reference which provides an aggregation function
    /// </summary>
    [XmlType(nameof(BiAggregateSqlColumnReference), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiAggregateSqlColumnReference : BiSqlColumnReference
    {

        /// <summary>
        /// Gets or sets the aggregation function
        /// </summary>
        [XmlAttribute("fn"), JsonProperty("fn")]
        public BiAggregateFunction Aggregation { get; set; }
    }
}