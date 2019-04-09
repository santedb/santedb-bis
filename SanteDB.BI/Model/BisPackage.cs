using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BI package
    /// </summary>
    [XmlRoot(nameof(BisPackage), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BisPackage), Namespace = BiConstants.XmlNamespace)]
    public class BisPackage : BisDefinition
    {
        
        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("sources"), XmlArrayItem("add")]
        public List<BisDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("formats"), XmlArrayItem("add")]
        public List<BisRenderFormatDefinition> Formats { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter definitions
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add")]
        public List<BisParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the list of query definitions
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add")]
        public List<BisQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the list of view defintiions
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add")]
        public List<BisViewDefinition> Views { get; set; }

        /// <summary>
        /// Gets or set sthe list of report definitions
        /// </summary>
        [XmlArray("reports"), XmlArrayItem("add")]
        public List<BisReportDefinition> Reports { get; set; }

    }
}
