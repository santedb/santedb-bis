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
    /// Represens a data flow call 
    /// </summary>
    [XmlType(nameof(BiDataFlowCallStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowCallStep : BiDataFlowStep
    {

        /// <summary>
        /// Get the data flow target
        /// </summary>
        [XmlElement("dataFlow"), JsonProperty("dataFlow")]
        public BiObjectReference TargetMethod { get; set; }

        /// <summary>
        /// Parameters to be passed
        /// </summary>
        [JsonProperty("args")]
        [XmlArray("args")]
        [XmlArrayItem("int", typeof(BiDataFlowCallArgument<Int32>))]
        [XmlArrayItem("string", typeof(BiDataFlowCallArgument<String>))]
        [XmlArrayItem("bool", typeof(BiDataFlowCallArgument<Boolean>))]
        [XmlArrayItem("uuid", typeof(BiDataFlowCallArgument<Guid>))]
        [XmlArrayItem("date-time", typeof(BiDataFlowCallArgument<DateTime>))]
        [XmlArrayItem("ref", typeof(BiDataFlowCallArgument<BiObjectReference>))]
        public List<BiDataFlowArgumentBase> Arguments { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
           
            if(this.TargetMethod == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.mart.flow.call.dataFlow.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(TargetMethod)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.TargetMethod.Validate(false))
                {
                    yield return itm;
                }
            }
        }

        /// <inheritdoc/>
        internal override BiDefinition FindObjectByName(string name)
        {
            return this.Arguments.Find(o=>o.Name == name)?.SimpleValue as BiDefinition ??
                base.FindObjectByName(name);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{this.GetType().Name} {this.TargetMethod?.Resolved?.Name ?? this.TargetMethod?.Ref ?? this.Name}({String.Join(",", this.Arguments.Select(a => a.ToString()))})]";
    }
}