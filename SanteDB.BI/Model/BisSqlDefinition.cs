using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a basic SQL definition
    /// </summary>
    [XmlType(nameof(BisSqlDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisSqlDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisSqlDefinition 
    {

        /// <summary>
        /// Gets or sets the invariant of SQL dialect
        /// </summary>
        [XmlAttribute("invariant")]
        public string Invariant { get; set; }

        /// <summary>
        /// Gets or sets the SQL
        /// </summary>
        [XmlText]
        public string Sql { get; set; }

    }
}