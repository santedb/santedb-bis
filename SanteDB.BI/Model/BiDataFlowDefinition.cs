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
 * Date: 2023-6-21
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
    /// Represents a single data flow which is an atomic operation which modifies data
    /// </summary>
    [XmlType(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataFlowDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowDefinition : BiFlowStepCollectionBase
    {

        /// <summary>
        /// HACK: To satisfy the resolveRefs some parameters need to be bound to values
        /// </summary>
        internal class BiDataFlowParameterBindingRef : BiDefinition
        {
        }

        /// <summary>
        /// Parameters to be passed
        /// </summary>
        [JsonProperty("parameters")]
        [XmlArray("parameters")]
        [XmlArrayItem("int", typeof(BiDataFlowParameter<Int32>))]
        [XmlArrayItem("string", typeof(BiDataFlowParameter<String>))]
        [XmlArrayItem("bool", typeof(BiDataFlowParameter<Boolean>))]
        [XmlArrayItem("uuid", typeof(BiDataFlowParameter<Guid>))]
        [XmlArrayItem("date-time", typeof(BiDataFlowParameter<DateTime>))]
        [XmlArrayItem("ref", typeof(BiDataFlowParameter<BiObjectReference>))]
        public List<BiDataFlowParameterBase> Parameters { get; set; }


        /// <summary>
        /// Gets or sets the object this flow returns
        /// </summary>
        [XmlElement("return"), JsonProperty("return")]
        public BiObjectReference ReturnObject { get; set; }


        /// <inheritdoc/>
        internal override BiDefinition FindObjectByName(string name)
        {
            var parameterMatch = this.Parameters?.OfType<BiDataFlowParameter<BiObjectReference>>().FirstOrDefault(o => o.Name == name);
            if (parameterMatch != null)
            {
                return new BiDataFlowParameterBindingRef() { Name = name };
            }
            return base.FindObjectByName(name);
        }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.Parameters?.Count > 0)
            {
                var p = 0;
                foreach (var itm in this.Parameters)
                {
                    p++;
                    if (String.IsNullOrEmpty(itm.Name))
                    {
                        yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.schema.flow.parameter", String.Format(ErrorMessages.MISSING_VALUE, $"{this.Name}.{nameof(Parameters)}[{p}]"), Guid.Empty);
                    }
                }
            }

            if (this.ReturnObject?.Resolved != null)
            {
                this.ReturnObject.Resolved.Validate(false);
            }
        }
    }
}