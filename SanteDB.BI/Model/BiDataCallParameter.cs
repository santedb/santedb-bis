using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Data Call Parameter
    /// </summary>
    [XmlType(nameof(BiDataCallParameter), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public abstract class BiDataCallParameter
    {

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

    }

    /// <summary>
    /// Call parameter
    /// </summary>
    [XmlType(Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataCallParameter<T> : BiDataCallParameter
    {

       
        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public T Value { get; set; }

    }
}