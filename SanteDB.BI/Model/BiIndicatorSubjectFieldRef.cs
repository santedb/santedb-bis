using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A reference to a type which is in a field
    /// </summary>
    [XmlType(nameof(BiIndicatorSubjectFieldRef), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorSubjectFieldRef : BiSqlColumnReference
    {
        /// <summary>
        /// Resource type XML
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public string ResourceTypeXml { get; set; }

    }
}