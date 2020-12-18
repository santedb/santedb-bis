using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a connection
    /// </summary>
    [XmlType(nameof(BiDataFlowConnectionStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowConnectionStep : BiDataFlowStep
    {

        /// <summary>
        /// Gets or sets the data source
        /// </summary>
        [XmlElement("dataSource"), JsonProperty("dataSource")]
        public BiDataSourceDefinition DataSource { get; set; }

        /// <summary>
        /// Gets or sets the transaction control (if true run in transaction)
        /// </summary>
        [XmlAttribute("transaction"), JsonProperty("transaction")]
        public bool RunInTransaction { get; set; }
    }
}