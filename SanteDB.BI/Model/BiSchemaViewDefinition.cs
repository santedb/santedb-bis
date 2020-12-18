using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Schema view definition
    /// </summary>
    [XmlRoot(nameof(BiSchemaViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiSchemaViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaViewDefinition : BiDefinition
    {

        /// <summary>
        /// True if the view is materialized
        /// </summary>
        [XmlAttribute("materialized"), JsonProperty("materialized")]
        public bool IsMaterialized { get; set; }

        /// <summary>
        /// Gets or sets the query for this view
        /// </summary>
        [XmlElement("query"), JsonProperty("query")]
        public BiQueryDefinition Query { get; set; }
    }
   
}
