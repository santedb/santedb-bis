using Newtonsoft.Json;
using SanteDB.Core.Model.Serialization;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A reference to a type which is in a field
    /// </summary>
    [XmlType(nameof(BiIndicatorSubjectFieldRef), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorSubjectFieldRef : BiSqlColumnReference
    {
        // Serialization binder
        private static readonly ModelSerializationBinder m_serializationBinder = new ModelSerializationBinder();

        /// <summary>
        /// Resource type XML
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public string ResourceTypeXml { get; set; }

        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        [XmlAttribute("parameter"), JsonProperty("parameter")]
        public String ParameterName { get; set; }

        /// <summary>
        /// Gets the resource type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ResourceType
        {
            get => m_serializationBinder.BindToType(null, this.ResourceTypeXml);
            set {
                if(value == null)
                {
                    this.ResourceTypeXml = null;
                }
                else
                {
                    m_serializationBinder.BindToName(value, out var asm, out var type);
                    this.ResourceTypeXml = type;
                }
            }
        }
    }
}