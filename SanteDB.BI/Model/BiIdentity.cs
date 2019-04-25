using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a simple BI identity
    /// </summary>
    [XmlType(nameof(BiIdentity), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiIdentity
    {

        /// <summary>
        /// Gets or sets the system which assigned the identity
        /// </summary>
        [XmlAttribute("system"), JsonProperty("system")]
        public string System { get; set; }

        /// <summary>
        /// Gets or sets the identifier value
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public string Value { get; set; }

    }
}