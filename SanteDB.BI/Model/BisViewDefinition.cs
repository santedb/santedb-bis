using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a view
    /// </summary>
    [XmlType(nameof(BisViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisViewDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisViewDefinition : BisDefinition
    {
        /// <summary>
        /// Gets or sets the body of the element
        /// </summary>
        [XmlElement("body", Namespace = BiConstants.HtmlNamespace)]
        public XElement Body { get; set; }

    }
}