using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a query definition
    /// </summary>
    [XmlType(nameof(BisQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BisQueryDefinition : BisDefinition
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public BisQueryDefinition()
        {
            this.DataSources = new List<BisDataSourceDefinition>();
            this.Parameters = new List<BisParameterDefinition>();
            this.QueryDefinitions = new List<BisSqlDefinition>();
        }

        /// <summary>
        /// Gets or sets the serialization definitions
        /// </summary>
        [XmlIgnore, JsonIgnore]
        internal bool ShouldSerializeDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the data sources
        /// </summary>
        [XmlArray("dataSources"), XmlArrayItem("add"), JsonProperty("dataSources")]
        public List<BisDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the parameter for this query
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BisParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the SQL definition
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add"), JsonIgnore]
        public List<BisSqlDefinition> QueryDefinitions { get; set; }

        /// <summary>
        /// Query definitions are only serialized on parse for reading/installation
        /// </summary>
        public bool ShouldSerializeQueryDefinitions() => this.ShouldSerializeDefinitions;

    }
}