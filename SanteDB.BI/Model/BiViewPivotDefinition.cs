using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an instructed definition for pivoting data
    /// </summary>
    [XmlType(nameof(BiViewPivotDefinition), Namespace = BiConstants.XmlNamespace), JsonObject]
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