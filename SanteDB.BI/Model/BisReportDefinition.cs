using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Represents a report definition
    /// </summary>
    [XmlType(nameof(BisReportDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisReportDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisReportDefinition : BisDefinition
    {

        /// <summary>
        /// Gets or sets the queries which feed this report
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add")]
        public List<BisQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the views
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add")]
        public List<BisViewDefinition> Views { get; set; }

    }
}