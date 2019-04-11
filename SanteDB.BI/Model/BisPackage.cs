using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    [JsonObject]
    public class BisPackage : BisDefinition
    {

        public BisPackage()
        {
            this.DataSources = new List<BisDataSourceDefinition>();
            this.Formats = new List<BisRenderFormatDefinition>();
            this.Parameters = new List<BisParameterDefinition>();
            this.Queries = new List<BisQueryDefinition>();
            this.Views = new List<BisViewDefinition>();
            this.Reports = new List<BisReportDefinition>();
        }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("sources"), XmlArrayItem("add"), JsonProperty("sources")]
        public List<BisDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("formats"), XmlArrayItem("add"), JsonProperty("formats")]
        public List<BisRenderFormatDefinition> Formats { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter definitions
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BisParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the list of query definitions
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add"), JsonProperty("queries")]
        public List<BisQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the list of view defintiions
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BisViewDefinition> Views { get; set; }

        /// <summary>
        /// Gets or set sthe list of report definitions
        /// </summary>
        [XmlArray("reports"), XmlArrayItem("add"), JsonProperty("reports")]
        public List<BisReportDefinition> Reports { get; set; }

        /// <summary>
        /// Saves the specified BIS Package
        /// </summary>
        public void Save(Stream s)
        {

            // First we need to enable the saving of definitions data 
            foreach (var itm in this.Reports.SelectMany(r => r.Views))
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Reports.SelectMany(q => q.Queries))
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Queries)
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Views)
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Parameters.Select(p => p.Values).OfType<BisQueryDefinition>())
                itm.ShouldSerializeDefinitions = true;

            new XmlSerializer(typeof(BisPackage)).Serialize(s, this);
        }
    }
}
