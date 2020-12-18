using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Data flow pivoting step
    /// </summary>
    [XmlType(nameof(BiDataFlowPivotStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowPivotStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// The pivot to perform
        /// </summary>
        [XmlElement("pivot"), JsonProperty("pivot")]
        public BiViewPivotDefinition Pivot { get; set; }

    }
}