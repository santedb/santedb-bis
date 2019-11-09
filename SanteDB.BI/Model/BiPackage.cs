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
    public class BiPackage : BiDefinition, IEnumerable<BiDefinition>
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

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<BiDefinition> GetEnumerator()
        {
            return this.DataSources.OfType<BiDefinition>()
                .Union(this.Formats.OfType<BiDefinition>())
                .Union(this.Parameters.OfType<BiDefinition>())
                .Union(this.Queries.OfType<BiDefinition>())
                .Union(this.Reports.OfType<BiDefinition>())
                .Union(this.Views.OfType<BiDefinition>()).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
