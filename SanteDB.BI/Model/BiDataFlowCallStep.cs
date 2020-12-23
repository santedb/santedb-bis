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
        [XmlArrayItem("int", typeof(BiDataCallParameter<Int32>))]
        [XmlArrayItem("string", typeof(BiDataCallParameter<String>))]
        [XmlArrayItem("bool", typeof(BiDataCallParameter<Boolean>))]
        [XmlArrayItem("uuid", typeof(BiDataCallParameter<Guid>))]
        [XmlArrayItem("date-time", typeof(BiDataCallParameter<DateTime>))]
        public List<object> Parameters { get; set; }

    }
}