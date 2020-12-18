using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a single data flow which is an atomic operation which modifies data
    /// </summary>
    [XmlType(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowDefinition : BiDefinition
    {

        /// <summary>
        /// True if this data flow can be called directly via API
        /// </summary>
        [JsonProperty("public"), XmlAttribute("public")]
        public bool IsPbulic { get; set; }

        /// <summary>
        /// XmlElement
        /// </summary>
        [XmlElement("call", typeof(BiDataFlowCallStep))]
        [XmlElement("reader", typeof(BiDataFlowDataReaderStep))]
        [XmlElement("writer", typeof(BiDataFlowDataWriterStep))]
        [XmlElement("connection", typeof(BiDataFlowConnectionStep))]
        [XmlElement("map", typeof(BiDataFlowMappingStep))]
        [XmlElement("pivot", typeof(BiDataFlowPivotStep))]
        [XmlElement("log", typeof(BiDataFlowLogStep))]
        [JsonProperty("step")]
        public List<BiDataFlowStep> Steps { get; set; }
    }
}