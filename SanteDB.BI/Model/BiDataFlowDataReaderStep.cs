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
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A data reader which queries data 
    /// </summary>
    [XmlType(nameof(BiDataFlowDataReaderStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowDataReaderStep : BiDataFlowStep, IDataFlowStreamStepDefinition
    {

        /// <summary>
        /// Gets or sets the input connection
        /// </summary>
        [XmlElement("connection"), JsonProperty("connection")]
        public BiObjectReference InputConnection { get; set; }

        /// <summary>
        /// Gets or sets the definitions for the data source
        /// </summary>
        [XmlArray("sql"), XmlArrayItem("add"), JsonProperty("definitions")]
        public List<BiSqlDefinition> Definition { get; set; }

        /// <summary>
        /// Gets or sets the schema of the reader
        /// </summary>
        [XmlElement("schema"), JsonProperty("schema")]
        public BiSchemaTableDefinition Schema { get; set; }

        /// <inheritdoc/>
        BiDataFlowStep IDataFlowStreamStepDefinition.InputStep => this.InputConnection?.Resolved as BiDataFlowConnectionStep;

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.Definition == null || this.Definition.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.Name}].sql.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Definition)), Guid.Empty);
            }
            else
            {
                foreach (var itm in this.Definition.SelectMany(o => o.Validate(false)))
                {
                    yield return itm;
                }
            }

            if (this.InputConnection == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.Name}].input.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(InputConnection)), Guid.Empty);
            }
            else if (this.InputConnection.Resolved == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Warning, $"bi.mart.flow.step[{this.Name}].input.notFound", string.Format(ErrorMessages.OBJECT_NOT_FOUND, this.InputConnection.Ref), Guid.Empty);

            }
            else
            {
                foreach (var itm in this.InputConnection.Resolved.Validate(false))
                {
                    yield return itm;
                }
            }


        }
    }
}