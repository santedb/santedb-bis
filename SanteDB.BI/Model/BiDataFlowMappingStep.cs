using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A data flow which maps data from an input stream to an output stream
    /// </summary>
    [XmlType(nameof(BiDataFlowMappingStep))]
    [JsonObject]
    public class BiDataFlowMappingStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Represents a map between source and target columns
        /// </summary>
        [XmlElement("map"), JsonProperty("map")]
        public List<BiColumnMapping> Mapping { get; set; }

    }
}