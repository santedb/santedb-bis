using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Call parameter
    /// </summary>
    [XmlType(nameof(BiDataCallParameter<T>), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataCallParameter<T>
    {

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public T Value { get; set; }

    }
}