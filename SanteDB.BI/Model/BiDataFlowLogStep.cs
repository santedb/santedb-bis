using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Writes to the log file the rows/message to the log
    /// </summary>
    [XmlType(nameof(BiDataFlowLogStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowLogStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Message
        /// </summary>
        [XmlText(), JsonProperty("message")]
        public string Message { get; set; }

    }
}