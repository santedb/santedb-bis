/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2019-11-27
 */
using Newtonsoft.Json;
using SanteDB.Core.Model.Attributes;
using SanteDB.Core.Model.Serialization;
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
    [XmlInclude(typeof(BiDataFlowDefinition))]
    [XmlInclude(typeof(BiSchemaTableDefinition))]
    [XmlInclude(typeof(BiSchemaViewDefinition))]
    [XmlInclude(typeof(BiTransformDefinition))]
    [XmlInclude(typeof(BiPackage))]
    public abstract class BiDefinition
    {

        // Serializers
        private static List<XmlSerializer> m_serializer = new List<XmlSerializer>();

        /// <summary>
        /// BI Definition
        /// </summary>
        static BiDefinition()
        {
            var types = new Type[]
            {
                typeof(BiPackage),
                typeof(BiQueryDefinition),
                typeof(BiDataSourceDefinition),
                typeof(BiParameterDefinition),
                typeof(BiReportDefinition),
                typeof(BiViewDefinition),
                typeof(BiReportViewDefinition),
                typeof(BiRenderFormatDefinition),
                typeof(BiTransformDefinition),
                typeof(BiSchemaTableDefinition),
                typeof(BiSchemaViewDefinition),
                typeof(BiDataFlowDefinition)
            };
            foreach (var t in types)
                m_serializer.Add(XmlModelSerializerFactory.Current.CreateSerializer(t, types));

        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public BiDefinition()
        {
        }

        /// <summary>
        /// When true identifies the object as a system object
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool IsSystemObject = false;

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
            XmlModelSerializerFactory.Current.CreateSerializer(this.GetType()).Serialize(s, this);
        }

        /// <summary>
        /// Load the specified object
        /// </summary>
        public static BiDefinition Load(Stream s)
        {

            // Attempt to load the appropriate serializer
            using (var xr = XmlReader.Create(s))
                foreach (var ser in m_serializer)
                    if (ser.CanDeserialize(xr))
                        return ser.Deserialize(xr) as BiDefinition;
            throw new InvalidDataException("Stream does not contain a valid BIS definition");
        }


        /// <summary>
        /// Gets or sets the serialization definitions
        /// </summary>
        [XmlIgnore, JsonIgnore]
        internal virtual bool ShouldSerializeDefinitions { get; set; }

    }
}