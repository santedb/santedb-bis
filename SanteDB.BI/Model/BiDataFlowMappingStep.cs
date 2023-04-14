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
    /// A data flow which maps data from an input stream to an output stream
    /// </summary>
    [XmlType(nameof(BiDataFlowMappingStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowMappingStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// Represents a map between source and target columns
        /// </summary>
        [XmlElement("map"), JsonProperty("map")]
        public List<BiColumnMapping> Mapping { get; set; }

        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if(this.Mapping == null || this.Mapping.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.Name}].mapping.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Mapping)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.Mapping.SelectMany(o=>o.Validate()))
                {
                    yield return itm;
                }
            }
        }

    }
}