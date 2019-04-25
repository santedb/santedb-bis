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
    [XmlRoot(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiPackage), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiPackage : BiDefinition
    {

        public BiPackage()
        {
            this.DataSources = new List<BiDataSourceDefinition>();
            this.Formats = new List<BiRenderFormatDefinition>();
            this.Parameters = new List<BiParameterDefinition>();
            this.Queries = new List<BiQueryDefinition>();
            this.Views = new List<BiViewDefinition>();
            this.Reports = new List<BiReportDefinition>();
        }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("sources"), XmlArrayItem("add"), JsonProperty("sources")]
        public List<BiDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the list of data sources
        /// </summary>
        [XmlArray("formats"), XmlArrayItem("add"), JsonProperty("formats")]
        public List<BiRenderFormatDefinition> Formats { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter definitions
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BiParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the list of query definitions
        /// </summary>
        [XmlArray("queries"), XmlArrayItem("add"), JsonProperty("queries")]
        public List<BiQueryDefinition> Queries { get; set; }

        /// <summary>
        /// Gets or sets the list of view defintiions
        /// </summary>
        [XmlArray("views"), XmlArrayItem("add"), JsonProperty("views")]
        public List<BiViewDefinition> Views { get; set; }

        /// <summary>
        /// Gets or set sthe list of report definitions
        /// </summary>
        [XmlArray("reports"), XmlArrayItem("add"), JsonProperty("reports")]
        public List<BiReportDefinition> Reports { get; set; }

        /// <summary>
        /// Saves the specified BIS Package
        /// </summary>
        public void Save(Stream s)
        {

            // First we need to enable the saving of definitions data 
            foreach (var itm in this.Reports.SelectMany(r => r.Views))
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Reports.SelectMany(q => q.Views))
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Queries)
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Views)
                itm.ShouldSerializeDefinitions = true;
            foreach (var itm in this.Parameters.Select(p => p.Values).OfType<BiQueryDefinition>())
                itm.ShouldSerializeDefinitions = true;

            new XmlSerializer(typeof(BiPackage)).Serialize(s, this);
        }
    }
}
