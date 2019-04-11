using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a report definition
    /// </summary>
    [XmlType(nameof(BisReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BisReportDefinition : BisDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BisReportDefinition()
        {
            this.Queries = new List<BisQueryDefinition>();
            this.Views = new List<BisViewDefinition>();
        }

        /// <summary>
        /// Gets or sets the queries which feed this report
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add"), JsonProperty("queries")]
        public List<BisQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the views
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BisViewDefinition> Views { get; set; }

    }
}