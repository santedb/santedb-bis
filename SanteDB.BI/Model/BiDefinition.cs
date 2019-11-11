﻿using Newtonsoft.Json;
using SanteDB.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Defines an abstract class for a BIS artifact definition
    /// </summary>
    [XmlType(nameof(BiDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [XmlInclude(typeof(BiViewDefinition))]
    [XmlInclude(typeof(BiQueryDefinition))]
    [XmlInclude(typeof(BiParameterDefinition))]
    [XmlInclude(typeof(BiReportDefinition))]
    [XmlInclude(typeof(BiRenderFormatDefinition))]
    [XmlInclude(typeof(BiDataSourceDefinition))]
    [XmlInclude(typeof(BiPackage))]
    public abstract class BiDefinition
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiDefinition()
        {
        }

        /// <summary>
        /// Gets or sets the alias name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name"), QueryParameter("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id"), QueryParameter("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [XmlAttribute("label"), JsonProperty("label")]
        public String Label { get; set; }

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        [XmlAttribute("ref"), JsonProperty("$ref"), QueryParameter("ref")]
        public String Ref { get; set; }
        /// <summary>
        /// Represents BI metadata about the object
        /// </summary>
        [XmlElement("meta"), JsonProperty("meta")]
        public BiMetadata MetaData { get; set; }

        /// <summary>
        /// Allows for the association of an external identifier
        /// </summary>
        [XmlElement("identifier"), JsonProperty("identifier")]
        public BiIdentity Identifier { get; set; }

        /// <summary>
        /// Saves this metadata definition to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            this.ShouldSerializeDefinitions = true;
            new XmlSerializer(this.GetType()).Serialize(s, this);
        }

        /// <summary>
        /// Load the specified object
        /// </summary>
        public static BiDefinition Load(Stream s)
        {
            var types = new Type[]
                    {
                                    typeof(BiPackage),
                                    typeof(BiQueryDefinition),
                                    typeof(BiDataSourceDefinition),
                                    typeof(BiParameterDefinition),
                                    typeof(BiReportDefinition),
                                    typeof(BiViewDefinition),
                                    typeof(BiReportViewDefinition)
                    };
            // Attempt to load the appropriate serializer
            using (var xr = XmlReader.Create(s))
                foreach (var t in types)
                {

                    var ser = new XmlSerializer(t, types);
                    if (ser.CanDeserialize(xr))
                        return ser.Deserialize(xr) as BiDefinition;
                }
                throw new InvalidDataException("Stream does not contain a valid BIS definition");
        }


        /// <summary>
        /// Gets or sets the serialization definitions
        /// </summary>
        [XmlIgnore, JsonIgnore]
        internal virtual bool ShouldSerializeDefinitions { get; set; }
    }
}