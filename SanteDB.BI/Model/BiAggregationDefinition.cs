using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BI Aggregation definition
    /// </summary>
    [XmlType(nameof(BiAggregationDefinition), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiAggregationDefinition : BiSqlDefinition
    {
        /// <summary>
        /// Gets or sets the groupings
        /// </summary>
        [XmlArray("grouping"), XmlArrayItem("column"), JsonProperty("grouping")]
        public List<BiSqlColumnReference> Groupings { get; set; }

        /// <summary>
        /// Gets or sets the selectors
        /// </summary>
        [XmlArray("select"), XmlArrayItem("column"), JsonProperty("select")]
        public List<BiAggregateSqlColumnReference> Columns { get; set; }

    }
}