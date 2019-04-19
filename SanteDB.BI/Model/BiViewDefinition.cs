using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view
    /// </summary>
    [XmlType(nameof(BiViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiViewDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets whether serialization should occur for definitional objects
        /// </summary>
        internal bool ShouldSerializeDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the body of the element
        /// </summary>
        [XmlElement("div", Namespace = BiConstants.HtmlNamespace), JsonIgnore]
        public XElement Body { get; set; }

        /// <summary>
        /// Gets whether the body should be serialized
        /// </summary>
        public bool ShouldSerializeBody() => this.ShouldSerializeDefinitions;
    }
}