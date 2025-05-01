/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an abstract flow step
    /// </summary>
    [XmlType(nameof(BiDataFlowStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public abstract class BiDataFlowStep : BiDefinition
    {

        /// <summary>
        /// Gets or sets the filter for the flow step (where the execute can selectively route data to the step)
        /// </summary>
        [XmlIgnore, JsonIgnore]
        internal String Filter { get; set; }


        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (String.IsNullOrEmpty(this.Name) && String.IsNullOrEmpty(this.Ref))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.GetType().Name}].name.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), Guid.Empty);
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"[{this.GetType().Name} {this.Name}]";

    }
}