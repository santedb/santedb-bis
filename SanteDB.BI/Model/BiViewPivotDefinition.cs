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
        [XmlArray("rows"), XmlArrayItem("add"), JsonProperty("rows")]
        public List<string> Rows { get; set; }

        /// <summary>
        /// Gets or sets the columns of the pivots
        /// </summary>
        [XmlArray("columns"), XmlArrayItem("add"), JsonProperty("columns")]
        public List<string> Columns { get; set; }

        /// <summary>
        /// Gets or sets the value of the pivots
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public string Value { get; set; }

    }
}