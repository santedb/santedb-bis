using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view on data in the BI package
    /// </summary>
    [XmlType(nameof(BiViewDefinition), Namespace = BiConstants.XmlNamespace), JsonObject(nameof(BiViewDefinition))]
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

        /// <summary>
        /// Gets or sets the serialization definitions
        /// </summary>
        [XmlIgnore, JsonIgnore]
        internal bool ShouldSerializeDefinitions { get; set; }

    }
}