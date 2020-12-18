using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BI Extract Transform Load
    /// </summary>
    /// <remarks>
    /// The SanteDB BI plugin allows for very basic transforms which can run on the server and on mobile among
    /// the different database systems.
    /// </remarks>
    [XmlType(nameof(BiTransformDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiTransformDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlInclude(typeof(BiSchemaTableDefinition))]
    [JsonObject]
    public class BiTransformDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the schema definitions for the specified transform definition
        /// </summary>
        [XmlArray("schema"),
            XmlArrayItem("table", typeof(BiSchemaTableDefinition)), 
            XmlArrayItem("view", typeof(BiSchemaViewDefinition)), 
            JsonProperty("schemas")]
        public List<BiSchemaObjectDefinition> Schemas { get; set; }

        /// <summary>
        /// Gets or sets the data flow definitions on this definition
        /// </summary>
        [XmlArray("dataFlows"), XmlArrayItem("flow"), JsonProperty("dataFlows")]
        public List<BiDataFlowDefinition> DataFlows { get; set; }

    }
}
