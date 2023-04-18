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
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2013.Excel;
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
    /// An individual when statement
    /// </summary>
    [XmlType(nameof(BiDataFlowWhenStatement), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowWhenStatement : BiDataFlowStreamFilterSetting
    {

        /// <summary>
        /// The flow to call 
        /// </summary>
        [XmlElement("call"), JsonProperty("call")]
        public BiDataFlowCallStep Call { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate()
        {
           
            if (this.Call == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.choose.when[{this.Value}].call.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Call)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.Call.Validate(false))
                {
                    yield return itm;
                }
            }
        }
    }

    /// <summary>
    /// Represents a choice between different routing pathways
    /// </summary>
    [XmlType(nameof(BiDataFlowChooseStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowChooseStep : BiDataFlowStreamStep
    {

        /// <summary>
        /// The when instructions
        /// </summary>
        [XmlElement("when"), JsonProperty("when")]
        public List<BiDataFlowWhenStatement> When { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if(this.When?.Any() != true)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.choice[{this.Name}].when.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(When)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.When.SelectMany(o=>o.Validate()))
                {
                    yield return itm;
                }
            }
        }

    }
}