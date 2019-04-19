using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Defines an abstract class for a BIS artifact definition
    /// </summary>
    [XmlType(nameof(BiDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public abstract class BiDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiDefinition()
        {
            this.Demands = new List<string>();
        }

        // Serializer
        private static XmlSerializer m_serializer;

        /// <summary>
        /// Gets or sets the alias name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [XmlAttribute("label"), JsonProperty("label")]
        public String Label { get; set; }

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        [XmlAttribute("ref"), JsonProperty("$ref")]
        public String Ref { get; set; }

        /// <summary>
        /// Gets or sets the list of demand policies
        /// </summary>
        [XmlArray("policies"), XmlArrayItem("demand"), JsonProperty("policies")]
        public List<String> Demands { get; set; }

        /// <summary>
        /// Gets or sets the documentation for this object
        /// </summary>
        [XmlElement("annotation"), JsonProperty("doc")]
        public BiAnnotation Annotation { get; set; }

        /// <summary>
        /// Load the specified object
        /// </summary>
        public static BiDefinition Load(Stream s)
        {
            if(m_serializer == null)
                m_serializer = new XmlSerializer(typeof(BiPackage), new Type[]
                {
                    typeof(BiQueryDefinition),
                    typeof(BiDataSourceDefinition),
                    typeof(BiParameterDefinition),
                    typeof(BiReportDefinition),
                    typeof(BiViewDefinition)
                });
            // Attempt to load the appropriate serializer
            using (var xr = XmlReader.Create(s))
                if (m_serializer.CanDeserialize(xr))
                    return m_serializer.Deserialize(xr) as BiDefinition;
                else
                    throw new InvalidDataException("Stream does not contain a valid BIS definition");
        }

    }
}