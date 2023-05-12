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
using Newtonsoft.Json;
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
    /// Instructs all flow object within the object to occur in parallel
    /// </summary>
    [XmlType(nameof(BiFlowStepCollectionBase), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage]
    public abstract class BiFlowStepCollectionBase : BiDataFlowStep
    {
        /// <summary>
        /// XmlElement
        /// </summary>
        [XmlArray("pipeline")]
        //[XmlArrayItem("parallel", typeof(BiDataFlowParallelStep))]
        [XmlArrayItem("halt", typeof(BiDataFlowHaltStep))]
        [XmlArrayItem("choose", typeof(BiDataFlowChooseStep))]
        [XmlArrayItem("filter", typeof(BiDataFlowFilterStep))]
        [XmlArrayItem("call", typeof(BiDataFlowCallStep))]
        [XmlArrayItem("union", typeof(BiDataFlowUnionStreamStep))]
        //[XmlArrayItem("intersect", typeof(BiDataFlowIntersectStreamStep))]
        [XmlArrayItem("reader", typeof(BiDataFlowDataReaderStep))]
        [XmlArrayItem("writer", typeof(BiDataFlowDataWriterStep))]
        [XmlArrayItem("connection", typeof(BiDataFlowConnectionStep))]
        [XmlArrayItem("transform", typeof(BiDataFlowMappingStep))]
        [XmlArrayItem("crosstab", typeof(BiDataFlowCrosstabStep))]
        [XmlArrayItem("transaction", typeof(BiDataFlowTransactionStep))]
        [XmlArrayItem("log", typeof(BiDataFlowLogStep))]
        [JsonProperty("step")]
        public List<BiDataFlowStep> Steps { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.Steps == null || this.Steps.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.schema.flow.steps.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Steps)), Guid.Empty);
            }
            else
            {
                foreach (var itm in this.Steps.OfType<BiDefinition>().SelectMany(o => o.Validate(false)))
                {
                    yield return itm;
                }
            }

        }

    }
}