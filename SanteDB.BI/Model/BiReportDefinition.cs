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
            this.DataSource = new List<BiDefinition>();
            this.Views = new List<BiReportViewDefinition>();
        }

        /// <summary>
        /// Gets or sets the data sources which feed this DIV
        /// </summary>
        [XmlArray("dataSources"),
            XmlArrayItem("query", typeof(BiQueryDefinition)),
            XmlArrayItem("view", typeof(BiViewDefinition)),
            JsonProperty("dataSources")]
        public List<BiDefinition> DataSource { get; set; }

        /// <summary>
        /// Gets or sets the views
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BiReportViewDefinition> Views { get; set; }

    }
}