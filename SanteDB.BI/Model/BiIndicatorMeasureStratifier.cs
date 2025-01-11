using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a single definition whereby a stratification may occur on a dataset
    /// </summary>
    [XmlType(nameof(BiIndicatorMeasureStratifier), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorMeasureStratifier : BiDefinition
    {
        /// <summary>
        /// Gets or sets the column reference
        /// </summary>
        [XmlElement("select"), JsonProperty("select")]
        public BiSqlColumnReference ColumnReference { get; set; }

        /// <summary>
        /// Gets or sets the subsequent columns as a sub-stratifier
        /// </summary>
        [XmlElement("thenBy"), JsonProperty("thenBy")]
        public BiIndicatorMeasureStratifier ThenBy { get; set; }
    }
}