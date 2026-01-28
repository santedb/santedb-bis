/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2024-10-30
 */
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a stream step that filters records 
    /// </summary>
    [XmlType(nameof(BiDataFlowDistinctStep), Namespace = BiConstants.XmlNamespace)]
    public class BiDataFlowDistinctStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Filter to apply
        /// </summary>
        [XmlElement("on"), JsonProperty("on")]
        public List<String> DistinctOn { get; set; }

        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if(this.DistinctOn?.Any() != true)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "err.bi.distinct.on", "Distinct filter requires a field name", Guid.Empty);
            }
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
        }
    }
}