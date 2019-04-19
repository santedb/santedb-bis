using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a report definition
    /// </summary>
    [XmlType(nameof(BiReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiReportDefinition : BiDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiReportDefinition()
        {
            this.Queries = new List<BiQueryDefinition>();
            this.Views = new List<BiViewDefinition>();
        }

        /// <summary>
        /// Gets or sets the queries which feed this report
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add"), JsonProperty("queries")]
        public List<BiQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the views
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BiViewDefinition> Views { get; set; }

    }
}