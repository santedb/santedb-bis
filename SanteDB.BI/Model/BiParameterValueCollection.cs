using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a simple parameter value
    /// </summary>
    [XmlType(nameof(BisParameterValue), Namespace = BiConstants.XmlNamespace)]
    public class BisParameterValue
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Label value
        /// </summary>
        [XmlText, JsonProperty("text")]
        public string Label { get; set; }

    }

    /// <summary>
    /// Represents a collection of simple value elements
    /// </summary>
    [XmlType(nameof(BiParameterValueCollection), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiParameterValueCollection
    {
        /// <summary>
        /// Add parameter value
        /// </summary>
        [XmlElement("add"), JsonProperty("list")]
        public List<BisParameterValue> Values { get; set; }

    }
}