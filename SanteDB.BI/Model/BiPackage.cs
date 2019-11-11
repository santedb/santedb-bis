using Newtonsoft.Json;
using System;
using System.Collections;
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

        /// <summary>
        /// Gets the specified definition from this package regardless of type
        /// </summary>
        public BiDefinition this[string id]
        {
            get
            {
                return (BiDefinition)this.DataSources.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Formats.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Parameters.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Queries.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Reports.FirstOrDefault(o => o.Id == id) ??
                    (BiDefinition)this.Views.FirstOrDefault(o => o.Id == id);
            }
        }

        /// <summary>
        /// Constructor for bi package
        /// </summary>
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
        /// True if the definitions should be serialized
        /// </summary>
        internal override bool ShouldSerializeDefinitions {
            get => base.ShouldSerializeDefinitions;
            set
            {
                base.ShouldSerializeDefinitions = value;
                foreach (var itm in new BiPackageEnumerator(this))
                    itm.ShouldSerializeDefinitions = value;
            }
        }

    }
}
