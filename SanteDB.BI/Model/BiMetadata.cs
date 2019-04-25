using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Identifies the states which a definition can carry
    /// </summary>
    [XmlType(nameof(BiDefinitionStatus), Namespace = BiConstants.XmlNamespace)]
    public enum BiDefinitionStatus
    {
        /// <summary>
        /// The definition is new and has not been reviewed
        /// </summary>
        [XmlEnum("new")]
        New = 0x0,
        /// <summary>
        /// The definition is in draft form
        /// </summary>
        [XmlEnum("draft")]
        Draft = 0x1,
        /// <summary>
        /// The definition is in review
        /// </summary>
        [XmlEnum("in-review")]
        InReview = 0x2,
        /// <summary>
        /// The definition is reviewed and active
        /// </summary>
        [XmlEnum("active")]
        Active = 0x3,
        /// <summary>
        /// The definition still works, however is deprecated
        /// </summary>
        [XmlEnum("deprecated")]
        Deprecated = 0x4,
        /// <summary>
        /// The definition is obsolete and should not be used
        /// </summary>
        [XmlEnum("obsolete")]
        Obsolete = 0x5
    }

    /// <summary>
    /// BI metadata
    /// </summary>
    [XmlType(nameof(BiMetadata), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiMetadata
    {

        /// <summary>
        /// BI Metadata ctor
        /// </summary>
        public BiMetadata()
        {
            this.Demands = new List<string>();
            this.Status = BiDefinitionStatus.New;
        }

        /// <summary>
        /// Gets or sets the version of this object
        /// </summary>
        [XmlAttribute("version"), JsonProperty("version")]
        public String Version { get; set; }
        
        /// <summary>
        /// Gets or sets the authors
        /// </summary>
        [XmlArray("authors"), XmlArrayItem("add"), JsonProperty("authors")]
        public List<string> Authors { get; set; }

        /// <summary>
        /// Gets or sets the status of the BI artifact
        /// </summary>
        [XmlAttribute("status"), JsonProperty("status")]
        public BiDefinitionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the documentation for this object
        /// </summary>
        [XmlElement("annotation"), JsonProperty("doc")]
        public BiAnnotation Annotation { get; set; }

        /// <summary>
        /// Gets or sets the list of demand policies
        /// </summary>
        [XmlArray("policies"), XmlArrayItem("demand"), JsonProperty("policies")]
        public List<string> Demands { get; set; }


    }
}