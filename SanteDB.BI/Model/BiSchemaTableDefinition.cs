/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using DocumentFormat.OpenXml.Math;
using Irony.Parsing;
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
    /// schema table definition
    /// </summary>
    [XmlRoot(nameof(BiSchemaTableDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiSchemaTableDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiSchemaTableDefinition : BiSchemaObjectDefinition
    {

        /// <summary>
        /// Gets or sets the parent of this object
        /// </summary>
        [XmlElement("parent"), JsonProperty("parent")]
        public BiObjectReference Parent { get; set; }

        /// <summary>
        /// True if the table should be temporary
        /// </summary>
        [XmlAttribute("temporary"), JsonProperty("temporary")]
        public bool Temporary { get; set; }

        /// <summary>
        /// Gets or sets the tablespace 
        /// </summary>
        [XmlAttribute("tableSpace"), JsonProperty("tableSpace")]
        public String Tablespace { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if(this.Temporary && !String.IsNullOrEmpty(this.Tablespace))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table[{this.Name}].temp", $"{nameof(Temporary)} and {nameof(Tablespace)} are exclusive", Guid.Empty);
            }
            if (this.Parent == null && String.IsNullOrEmpty(this.Ref)) {
                if (this.Columns == null || this.Columns.Count == 0)
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table[{this.Name}].columns.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Columns)), Guid.Empty);
                }
                else if (!this.Columns.Any(c=>c.IsKey) && !this.Temporary)
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table[{this.Name}].noPrimaryKey", String.Format(ErrorMessages.FIELD_NOT_FOUND, "Primary Key"), Guid.Empty);
                }
                else if(this.Columns.Count(c=>c.IsKey) > 1)
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table[{this.Name}].noCompositeKeys", String.Format(ErrorMessages.WOULD_RESULT_INVALID_STATE, "Composite Primary Key"), Guid.Empty);
                }
            }

            // Ensure we don't have infinite circular parents
            if(this.Parent != null)
            {
                var parentSet = new HashSet<String>();
                parentSet.Add(this.Name);
                var parent = this.Parent;
                while(parent != null)
                {
                    if(parentSet.Contains(parent.Ref))
                    {
                        yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table[{this.Name}].circular_dep", String.Format(ErrorMessages.WOULD_RESULT_INVALID_STATE, $"Circular Parent Reference : {String.Join(">", parentSet) }"), Guid.Empty);
                        break;
                    }
                    else
                    {
                        parentSet.Add(parent.Ref);
                        parent = (parent.Resolved as BiSchemaTableDefinition)?.Parent;
                    }
                }
            }
        }
    }
}
