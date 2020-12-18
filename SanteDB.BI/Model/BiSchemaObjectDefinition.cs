using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// A schema definition 
    /// </summary>
    [XmlType(nameof(BiSchemaObjectDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaObjectDefinition : BiDefinition
    {

        /// <summary>
        /// Columns in the object definition
        /// </summary>
        [XmlElement("column"), JsonProperty("columns")]
        public List<BiSchemaColumnDefinition> Columns { get; set; }
    }
}