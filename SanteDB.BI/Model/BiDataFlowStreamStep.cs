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
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using SanteDB.BI.Datamart.DataFlow;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// A data flow step which streams data from another data flow step
    /// </summary>
    [XmlType(nameof(BiDataFlowStreamStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public abstract class BiDataFlowStreamStep : BiDataFlowStep, IDataFlowStreamStepDefinition
    {

        /// <summary>
        /// Input for the data flow step
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public BiObjectReference InputObject { get; set; }

        /// <inheritdoc/>
        BiDataFlowStep IDataFlowStreamStepDefinition.InputStep => this.InputObject?.Resolved as BiDataFlowStep;


        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.InputObject == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.Name}].input.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(InputObject)), Guid.Empty);
            }
            else if(this.InputObject.Resolved == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Warning, $"bi.mart.flow.step[{this.Name}].input.notFound", string.Format(ErrorMessages.OBJECT_NOT_FOUND, this.InputObject.Ref), Guid.Empty);

            }
            else
            {
                foreach (var itm in this.InputObject.Resolved.Validate(false))
                {
                    yield return itm;
                }
            }

        }
    }
}