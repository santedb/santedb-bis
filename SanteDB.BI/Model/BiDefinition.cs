/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2023-6-21
 */
using Newtonsoft.Json;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Util;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using SanteDB.Core.Model.Attributes;
using SanteDB.Core.Model.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
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
    [XmlInclude(typeof(BiDatamartDefinition))]
    [XmlInclude(typeof(BiPackage))]
    [XmlInclude(typeof(BiReportViewDefinition))]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDefinition
    {
        // Serializers
        private static List<XmlSerializer> s_serializers = new List<XmlSerializer>(10);
        // True if the definition has already been validated
        private IEnumerable<DetectedIssue> m_validationResult = null;

        /// <summary>
        /// BI Definition
        /// </summary>
        static BiDefinition()
        {
            var types = new Type[]
            {
                typeof(BiDataSourceDefinition),
                typeof(BiRenderFormatDefinition),
                typeof(BiQueryDefinition),
                typeof(BiParameterDefinition),
                typeof(BiReportDefinition),
                typeof(BiViewDefinition),
                typeof(BiReportViewDefinition),
                typeof(BiDatamartDefinition),
                typeof(BiSchemaTableDefinition),
                typeof(BiSchemaViewDefinition),
                typeof(BiDataFlowDefinition),
                typeof(BiPackage)
            };


            foreach (var t in types)
            {
                s_serializers.Add(new XmlSerializer(t, types));
            }
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
        /// Gets or sets the UUID
        /// </summary>
        [XmlAttribute("uuid"), JsonProperty("uuid"), QueryParameter("uuid")]
        public Guid Uuid { get; set; }

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
        /// Gets or sets the status of the BI artifact
        /// </summary>
        [XmlAttribute("status"), JsonProperty("status")]
        public BiDefinitionStatus Status { get; set; }

        /// <summary>
        /// Saves this metadata definition to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            XmlModelSerializerFactory.Current.CreateSerializer(this.GetType()).Serialize(s, this);
        }

        /// <summary>
        /// Load the specified object
        /// </summary>
        public static BiDefinition Load(Stream s)
        {
            // Attempt to load the appropriate serializer
            using (var xr = XmlReader.Create(s))
            {
                foreach (var sz in s_serializers)
                {
                    if (sz.CanDeserialize(xr))
                    {
                        return sz.Deserialize(xr) as BiDefinition;
                    }
                }
            }
            throw new InvalidDataException("Stream does not contain a valid BIS definition");
        }

        /// <summary>
        /// Get an object in this object by ID
        /// </summary>
        /// <param name="id">The name of the object to find</param>
        /// <returns></returns>
        internal virtual BiDefinition FindObjectById(string id)
        {
            // Initialize the sub-properties 
            foreach (var pi in this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var piValue = pi.GetValue(this);
                if (piValue is BiDefinition bid && bid.Id == id)
                {
                    return bid;
                }
                else if (piValue is IList list)
                {
                    foreach (var itm in list?.OfType<BiDefinition>())
                    {
                        if (itm.Id == id)
                        {
                            return itm;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get an object in this object by name
        /// </summary>
        /// <param name="name">The name of the object to find</param>
        /// <returns></returns>
        internal virtual BiDefinition FindObjectByName(string name)
        {
            if (this.Name == name)
            {
                return this;
            }
            // Initialize the sub-properties 
            foreach (var pi in this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => typeof(BiDefinition).IsAssignableFrom(p.PropertyType) || typeof(IList).IsAssignableFrom(p.PropertyType)))
            {
                var piValue = pi.GetValue(this);
                if (piValue is BiDefinition bid && bid.Name == name)
                {
                    return bid;
                }
                else if (piValue is IList list)
                {
                    foreach (var itm in list?.OfType<BiDefinition>())
                    {
                        if (itm.Name == name)
                        {
                            return itm;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Validate this BI definition
        /// </summary>
        public IEnumerable<DetectedIssue> Validate()
        {
            try
            {
                if (this.m_validationResult == null)
                {
                    var copy = BiUtils.ResolveRefs(this);
                    this.m_validationResult = copy.Validate(true);
                }
                return this.m_validationResult;
            }
            catch (BiException e)
            {
                return new DetectedIssue[] { new DetectedIssue(DetectedIssuePriorityType.Error, "bi.resolve", e.Message, Guid.Empty) };
            }
        }

        /// <summary>
        /// Validate the BI values
        /// </summary>
        internal virtual IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (String.IsNullOrEmpty(this.Id) && isRoot)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.id.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Id)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (String.IsNullOrEmpty(this.Name) && isRoot)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Warning, "bi.name.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (this.MetaData == null && isRoot)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.meta.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(MetaData)), DetectedIssueKeys.InvalidDataIssue);
            }

            if (isRoot && !BiUtils.CanResolveRefs(this, out var unresolved))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.ref.error", string.Format(ErrorMessages.REFERENCE_NOT_FOUND, unresolved.Ref), DetectedIssueKeys.OtherIssue);
            }
        }

        /// <summary>
        /// Get this instance as a summarized instanced
        /// </summary>
        /// <returns></returns>
        public virtual BiDefinition AsSummarized()
        {
            var retVal = Activator.CreateInstance(this.GetType()) as BiDefinition;
            retVal.Id = this.Id;
            if (this.Identifier != null)
            {
                retVal.Identifier = new BiIdentity().CopyObjectData(this.Identifier);
            }
            retVal.Name = this.Name;
            retVal.Label = this.Label;
            retVal.MetaData = new BiMetadata().CopyObjectData(this.MetaData);
            return retVal;
        }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is BiDefinition bid)
            {
                return bid.Id == this.Id &&
                    bid.Name == this.Name &&
                    bid.Ref == this.Ref;
            }
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = 0;
            if (!String.IsNullOrEmpty(this.Name))
            {
                hash += this.Name.GetHashCode() * 17;
            }
            if (!String.IsNullOrEmpty(this.Id))
            {
                hash += this.Id.GetHashCode() * 17;
            }
            if (!String.IsNullOrEmpty(this.Ref))
            {
                hash += this.Ref.GetHashCode() * 17;
            }
            if (hash == 0)
            {
                return base.GetHashCode();
            }
            return hash;
        }

    }
}