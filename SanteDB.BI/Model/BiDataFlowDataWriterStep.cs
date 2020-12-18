using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Indicates the writer mode
    /// </summary>
    [XmlType(nameof(DataWriterModeType), Namespace = BiConstants.XmlNamespace)]
    public enum DataWriterModeType
    {
        /// <summary>
        /// Data should be inserted if it doesn't exist, or updated if it does
        /// </summary>
        [XmlEnum("insertUpdate")]
        InsertOrUpdate,
        /// <summary>
        /// Data should only be inserted
        /// </summary>
        [XmlEnum("insert")]
        Insert,
        /// <summary>
        /// Data should be only updated
        /// </summary>
        [XmlEnum("update")]
        Update,
        /// <summary>
        /// Data should be deleted
        /// </summary>
        [XmlEnum("delete")]
        Delete
    }

    /// <summary>
    /// A data writer, which writes data to the destination
    /// </summary>
    [XmlType(nameof(BiDataFlowDataWriterStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowDataWriterStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Gets or sets the connection on this writer
        /// </summary>
        [XmlElement("connection"), JsonProperty("connection")]
        public BiDataFlowConnectionStep Connection { get; set; }

        /// <summary>
        /// When true, the table should be created
        /// </summary>
        [XmlAttribute("create"), JsonProperty("create")]
        public bool CreateTable { get; set; }

        /// <summary>
        /// Truncate the table
        /// </summary>
        [XmlAttribute("truncate"), JsonProperty("truncate")]
        public bool TruncateTable { get; set; }

        /// <summary>
        /// The data insert mode
        /// </summary>
        [XmlAttribute("mode"), JsonProperty("mode")]
        public DataWriterModeType Mode { get; set; }

        /// <summary>
        /// Gets or sets the target of the writer
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public BiSchemaTableDefinition Target { get; set; }
    }
}