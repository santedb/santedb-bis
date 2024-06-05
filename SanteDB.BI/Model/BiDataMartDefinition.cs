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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BI Extract Transform Load
    /// </summary>
    /// <remarks>
    /// The SanteDB BI plugin allows for very basic transforms which can run on the server and on mobile among
    /// the different database systems.
    /// </remarks>
    [XmlType(nameof(BiDatamartDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDatamartDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlInclude(typeof(BiSchemaTableDefinition))]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiDatamartDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the transform definition that this is based on
        /// </summary>
        [XmlElement("basedOn"), JsonProperty("basedOn")]
        public BiObjectReference BasedOn { get; set; }

        /// <summary>
        /// Identifies that this mart extends another
        /// </summary>
        [XmlElement("extends"), JsonProperty("extends")]
        public BiObjectReference Extends { get; set; }

        /// <summary>
        /// Gets or sets the data mart that this mart definition produces
        /// </summary>
        [XmlElement("produces"), JsonProperty("produces")]
        public BiDataSourceDefinition Produces { get; set; }

        /// <summary>
        /// Gets or sets the schema definitions for the specified transform definition
        /// </summary>
        [XmlArray("schema"),
            XmlArrayItem("table", typeof(BiSchemaTableDefinition)),
            XmlArrayItem("view", typeof(BiSchemaViewDefinition)),
            JsonProperty("schemas")]
        public List<BiSchemaObjectDefinition> SchemaObjects { get; set; }

        /// <summary>
        /// Gets or sets the data flow definitions on this definition
        /// </summary>
        [XmlArray("dataFlows"), XmlArrayItem("flow"), JsonProperty("dataFlows")]
        public List<BiDataFlowDefinition> DataFlows { get; set; }

        /// <summary>
        /// Entry flow
        /// </summary>
        [XmlElement("startFlow"), JsonProperty("startFlow")]
        public BiObjectReference EntryFlow { get; set; }

        /// <inheritdoc />
        internal override BiDefinition FindObjectByName(string name)
        {
            return this.SchemaObjects.Find(o => o.Name == name) ??
                this.DataFlows.Find(o => o.Name == name) ??
                this.DataFlows.Select(o => o.FindObjectByName(name)).FirstOrDefault() ??
                base.FindObjectByName(name);
        }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if ((this.SchemaObjects == null || this.SchemaObjects.Count == 0) && this.BasedOn == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.mart.schema.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(SchemaObjects)), Guid.Empty);
            }
            if (this.DataFlows == null || this.DataFlows.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.mart.flows.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(DataFlows)), Guid.Empty);
            }
            if (this.Produces == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.mart.produces.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Produces)), Guid.Empty);
            }
            else if (String.IsNullOrEmpty(this.Produces.Id))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.mart.produces.id.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Id)), Guid.Empty);
            }


            foreach (var itm in this.SchemaObjects.OfType<BiDefinition>().Union(this.DataFlows))
            {
                foreach (var de in itm.Validate(false))
                {
                    yield return de;
                }
            }

            if (this.EntryFlow != null && !(this.EntryFlow.Resolved is BiDataFlowDefinition) && !this.DataFlows.Any(f => f.Name == "main"))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.mart.flows.startFlow.missing", String.Format(ErrorMessages.MISSING_ENTRY_POINT, this.EntryFlow.Ref), Guid.Empty);
            }



        }
    }
}
