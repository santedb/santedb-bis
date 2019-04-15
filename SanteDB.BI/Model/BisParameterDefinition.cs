using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Enumeration for the data types 
    /// </summary>
    [XmlType(nameof(BisParameterDataType), Namespace = BiConstants.XmlNamespace)]
    public enum BisParameterDataType
    {
        [XmlEnum("ref")]
        Ref,
        [XmlEnum("uuid")]
        Uuid,
        [XmlEnum("string")]
        String,
        [XmlEnum("int")]
        Integer,
        [XmlEnum("bool")]
        Boolean,
        [XmlEnum("date")]
        Date,
        [XmlEnum("date-time")]
        DateTime
    }

    /// <summary>
    /// Represents a parameter definition
    /// </summary>
    [XmlType(nameof(BisParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisParameterDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BisParameterDefinition : BisDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BisParameterDefinition()
        {
        }

        /// <summary>
        /// Gets or sets the type of parameter
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BisParameterDataType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set the min value
        /// </summary>
        [XmlAttribute("min"), JsonProperty("min")]
        public string MinValue { get; set; }

        /// <summary>
        /// Get or sets the max value
        /// </summary>
        [XmlAttribute("max"), JsonProperty("max")]
        public string MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the values for the parameter
        /// </summary>
        [XmlElement("query", typeof(BisQueryDefinition)),
         XmlElement("values", typeof(BisParameterValueCollection)),
         JsonProperty("values")]
        public Object Values { get; set; }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        [XmlAttribute("default"), JsonProperty("default")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Required value
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public string RequiredXml
        {
            get => this.Required?.ToString().ToLower();
            set => this.Required = string.IsNullOrEmpty(value) ? false : bool.Parse(value);
        }

        /// <summary>
        /// Required parameter?
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool? Required { get; set; }
    }
}