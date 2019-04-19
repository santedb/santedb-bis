using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a query definition
    /// </summary>
    [XmlType(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiQueryDefinition : BiDefinition
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public BiQueryDefinition()
        {
            this.DataSources = new List<BiDataSourceDefinition>();
            this.Parameters = new List<BiParameterDefinition>();
            this.QueryDefinitions = new List<BiSqlDefinition>();
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
        public List<BiDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the parameter for this query
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BiParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the SQL definition
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add"), JsonIgnore]
        public List<BiSqlDefinition> QueryDefinitions { get; set; }

        /// <summary>
        /// Query definitions are only serialized on parse for reading/installation
        /// </summary>
        public bool ShouldSerializeQueryDefinitions() => this.ShouldSerializeDefinitions;

    }
}