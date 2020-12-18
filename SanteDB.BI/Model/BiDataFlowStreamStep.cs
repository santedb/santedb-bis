using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A data flow step which streams data from another data flow step
    /// </summary>
    [XmlType(nameof(BiDataFlowStreamStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public abstract class BiDataFlowStreamStep : BiDataFlowStep
    {

        /// <summary>
        /// Input for the data flow step
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public BiDataFlowStep InputFlow { get; set; }

    }
}