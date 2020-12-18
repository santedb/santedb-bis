using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a definition for a column
    /// </summary>
    [XmlType(nameof(BiSchemaColumnDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaColumnDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the type of column
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BiDataType Type { get; set; }

        /// <summary>
        /// True if this is not null
        /// </summary>
        [XmlAttribute("notNull"), JsonProperty("notNull")]
        public bool NotNull { get; set; }

        /// <summary>
        /// True if this column is indexed
        /// </summary>
        [XmlAttribute("index"), JsonProperty("index")]
        public bool IsIndex { get; set; }

        /// <summary>
        /// True if this column is unique
        /// </summary>
        [XmlAttribute("unique"), JsonProperty("unique")]
        public bool IsUnique { get; set; }

        /// <summary>
        /// True if this object is a key
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets the table that this column referneces (as a foreign key)
        /// </summary>
        [XmlElement("references"), JsonProperty("references")]
        public BiSchemaObjectReference References { get; set; }


    }
}