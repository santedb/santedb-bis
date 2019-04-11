using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an annotation for the object
    /// </summary>
    [XmlType(nameof(BisAnnotation), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BisAnnotation 
    {
        /// <summary>
        /// Represents the body of the annotation
        /// </summary>
        [XmlElement("div", Namespace = BiConstants.HtmlNamespace), JsonIgnore]
        public XElement Body { get; set; }

        /// <summary>
        /// Gets the body in JSON format
        /// </summary>
        [XmlIgnore, JsonProperty("doc")]
        public string JsonBody
        {
            get => this.Body?.ToString();
            set {
                if (value != null)
                {
                    using (var sr = new StringReader(value))
                        this.Body = XElement.Load(sr);
                }
                else
                    this.Body = null;
            }
        }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        [XmlAttribute("lang"), JsonProperty("lang")]
        public string Language { get; set; }
    }
}