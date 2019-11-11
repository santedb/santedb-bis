using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view
    /// </summary>
    [XmlType(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiReportViewDefinition : BiDefinition
    {

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