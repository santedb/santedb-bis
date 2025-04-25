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
 * Date: 2025-1-13
 */
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a single computation for a measure
    /// </summary>
    [XmlType(nameof(BiMeasureComputationColumnReference), Namespace = BiConstants.XmlNamespace)]
    public abstract class BiMeasureComputationColumnReference : BiAggregateSqlColumnReference
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public override string Name {
            get => this.GetColumnName();
            set { }
        }

        /// <summary>
        /// Get the name of the measure column
        /// </summary>
        /// <returns></returns>
        public abstract string GetColumnName();

    }
    
    /// <summary>
    /// Computation represents the numerator
    /// </summary>
    [XmlType(nameof(BiMeasureComputationNumerator), Namespace = BiConstants.XmlNamespace)]
    public class BiMeasureComputationNumerator : BiMeasureComputationColumnReference {
        public override string GetColumnName() => "numerator";
    }

    /// <summary>
    /// Computation represents the denominator
    /// </summary>
    [XmlType(nameof(BiMeasureComputationNumeratorExclusion), Namespace = BiConstants.XmlNamespace)]
    public class BiMeasureComputationNumeratorExclusion : BiMeasureComputationColumnReference { 
        public override string GetColumnName() => "numerator_exclusion";
    }


    [XmlType(nameof(BiMeasureComputationDenominator), Namespace = BiConstants.XmlNamespace)]
    public class BiMeasureComputationDenominator : BiMeasureComputationColumnReference {
        public override string GetColumnName() => "denominator";

    }
    [XmlType(nameof(BiMeasureComputationDenominatorExclusion), Namespace = BiConstants.XmlNamespace)]
    public class BiMeasureComputationDenominatorExclusion : BiMeasureComputationColumnReference {
        public override string GetColumnName() => "denominator_exclusion";
    }
    [XmlType(nameof(BiMeasureComputationScore), Namespace = BiConstants.XmlNamespace)]
    public class BiMeasureComputationScore : BiMeasureComputationColumnReference {
        public override string GetColumnName() => "score";

    }

}