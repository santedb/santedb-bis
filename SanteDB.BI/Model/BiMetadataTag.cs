using Newtonsoft.Json;
using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a tag on Bi metadata
    /// </summary>
    [XmlType(nameof(BiMetadataTag), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiMetadataTag 
    {
        /// <summary>
        /// Serialization constructor
        /// </summary>
        public BiMetadataTag()
        {

        }

        /// <summary>
        /// Creates a BI metadata tag with the specified name and value
        /// </summary>
        /// <param name="name">The name of the tag</param>
        /// <param name="value">The value of the tag</param>
        public BiMetadataTag(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
        /// <summary>
        /// Gets the name of the tag
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the value of the tag
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public String Value { get; set; }

    }
}
