using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A data reader which queries data 
    /// </summary>
    [XmlType(nameof(BiDataFlowDataReaderStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowDataReaderStep : BiDataFlowStep
    {

        /// <summary>
        /// Gets or sets the data source
        /// </summary>
        [XmlElement("connection"), JsonProperty("connection")]
        public BiDataFlowConnectionStep Connection { get; set; }

        /// <summary>
        /// Gets or sets the definitions for the data source
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add") , JsonProperty("definitions")]
        public List<BiSqlDefinition> Definition { get; set; }
    }
}