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