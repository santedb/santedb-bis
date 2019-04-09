using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Documents the render formatters
    /// </summary>
    [XmlType(nameof(BisRenderFormatDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisRenderFormatDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisRenderFormatDefinition : BisDefinition
    {

        /// <summary>
        /// Gets the render format
        /// </summary>
        [XmlAttribute("extension")]
        public String FormatExtension { get; set; }

        /// <summary>
        /// Gets or sets the mime encoding for the formatting
        /// </summary>
        [XmlAttribute("contentType")]
        public String ContentType { get; set; }

        /// <summary>
        /// Gets or sets the rendere xml format
        /// </summary>
        [XmlAttribute("renderer")]
        public String TypeXml {
            get => this.Type?.AssemblyQualifiedName;
            set
            {
                if (value != null)
                    this.Type = Type.GetType(value);
                else
                    this.Type = null;
            }
        }

        /// <summary>
        /// Gets or sets the actual renderer type
        /// </summary>
        [XmlIgnore]
        public Type Type { get; set; }

    }
}
