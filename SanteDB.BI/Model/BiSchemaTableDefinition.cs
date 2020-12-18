using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// schema table definition
    /// </summary>
    [XmlRoot(nameof(BiSchemaTableDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiSchemaTableDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaTableDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the tablespace 
        /// </summary>
        [XmlAttribute("tableSpace"), JsonProperty("tableSpace")]
        public String Tablespace { get; set; }
    }
}
