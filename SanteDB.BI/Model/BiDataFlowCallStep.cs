using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represens a data flow call 
    /// </summary>
    [XmlType(nameof(BiDataFlowCallStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowCallStep : BiDataFlowStep
    {

        /// <summary>
        /// Parameters to be passed
        /// </summary>
        [JsonProperty("parameters")]
        [XmlArray("parameters")]
        [XmlArrayItem("int", typeof(int))]
        [XmlArrayItem("string", typeof(string))]
        [XmlArrayItem("bool", typeof(bool))]
        [XmlArrayItem("uuid", typeof(Guid))]
        [XmlArrayItem("date-time", typeof(DateTime))]
        public List<object> Parameters { get; set; }

    }
}