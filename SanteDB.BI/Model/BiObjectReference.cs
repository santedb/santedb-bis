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
using SanteDB.BI.Util;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a reference to another schema object
    /// </summary>
    [XmlType(nameof(BiObjectReference), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiObjectReference : BiDefinition
    {

        /// <summary>
        /// The resolved reference
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public BiDefinition Resolved { get; internal set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (string.IsNullOrEmpty(this.Ref))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.ref.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Ref)), Guid.Empty);
            }
            if (this.Resolved == null && !BiUtils.CanResolveRefs(this, out var unresolved))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Warning, "bi.ref.notfound", string.Format(ErrorMessages.REFERENCE_NOT_FOUND, this.Resolved), Guid.Empty);
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{this.GetType().Name}#{this.Ref} ({this.Resolved})]";
    }
}