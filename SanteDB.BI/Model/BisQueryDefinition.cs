using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a query definition
    /// </summary>
    [XmlType(nameof(BisQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisQueryDefinition : BisDefinition
    {

        /// <summary>
        /// Gets or sets the data sources
        /// </summary>
        [XmlArray("dataSources"), XmlArrayItem("add")]
        public List<BisDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the parameter for this query
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add")]
        public List<BisParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the SQL definition
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add")]
        public List<BisSqlDefinition> QueryDefinitions { get; set; }


    }
}