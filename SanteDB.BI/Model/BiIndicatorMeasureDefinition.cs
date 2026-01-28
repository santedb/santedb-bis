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
 * Date: 2025-1-10
 */
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a single measure which is calculated in the BI subsystem
    /// </summary>
    [XmlType(nameof(BiIndicatorMeasureDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorMeasureDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the computations
        /// </summary>
        [XmlArray("computed-by"),
            XmlArrayItem("numerator", typeof(BiMeasureComputationNumerator)),
            XmlArrayItem("numerator-exclusion" ,typeof(BiMeasureComputationNumeratorExclusion)),
            XmlArrayItem("denominator", typeof(BiMeasureComputationDenominator)),
            XmlArrayItem("denominator-exclusion", typeof(BiMeasureComputationDenominatorExclusion)),
            XmlArrayItem("score", typeof(BiMeasureComputationScore)), 
            JsonProperty("computedBy")]
        public List<BiMeasureComputationColumnReference> Computation { get; set; }
        
        /// <summary>
        /// Gets or sets the stratification definitions
        /// </summary>
        [XmlArray("stratify"), XmlArrayItem("by"), JsonProperty("stratify")]
        public List<BiIndicatorMeasureStratifier> Stratifiers { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
        }

    }
}