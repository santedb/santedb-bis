using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// A column mapping between a souce and destination column
    /// </summary>
    [XmlType(nameof(BiColumnMapping), Namespace = BiConstants.XmlNamespace)]
    public class BiColumnMapping
    {
        /// <summary>
        /// Gets or sets the source of the mapping
        /// </summary>
        [XmlElement("source"), JsonProperty("source")]
        public BiColumnMappingSource Source { get; set; }

        /// <summary>
        /// Gets or sets the target of the mapping
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public String Target { get; set; }
    }

    /// <summary>
    /// Schema definition for an input column with an optional transform
    /// </summary>
    [XmlType(nameof(BiColumnMappingSource), Namespace = BiConstants.XmlNamespace)]
    public class BiColumnMappingSource : BiSchemaColumnDefinition
    {

        /// <summary>
        /// The column transformation expression
        /// </summary>
        [XmlElement("transform", typeof(String))]
        [XmlElement("lookup", typeof(BiColumnMappingTransformJoin))]
        public Object TransformExpression { get; set; }

    }

    /// <summary>
    /// Indicates the source transform from another column
    /// </summary>
    [XmlType(nameof(BiColumnMappingTransformJoin), Namespace = BiConstants.XmlNamespace)]
    public class BiColumnMappingTransformJoin 
    {

        /// <summary>
        /// Gets or sets the input stream step reference where the join should be sourced
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public BiDataFlowStreamStep Input { get; set; }

        /// <summary>
        /// Gets or sets the join expression for the join operation
        /// </summary>
        [XmlElement("join") ,JsonProperty("join")]
        public String Join { get; set; }
    }
}