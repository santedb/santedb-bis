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
        /// Gets or sets the period of the measure (how often the measure is to be taken)
        /// </summary>
        [XmlElement("period"), JsonProperty("period")]
        public BiIndicatorPeriodDefinition Period { get; set; }

        /// <summary>
        /// Gets or sets the subject reference
        /// </summary>
        [XmlElement("subject"), JsonProperty("subject")]
        public BiIndicatorSubjectFieldRef Subject { get; set; }

        /// <summary>
        /// Gets or sets the column definition for the numerator
        /// </summary>
        [XmlElement("numerator"), JsonProperty("numerator")]
        public BiAggregateSqlColumnReference Numerator { get; set; }

        /// <summary>
        /// Gets or sets the column definition for the denominator
        /// </summary>
        [XmlElement("denominator"), JsonProperty("denominator")]
        public BiAggregateSqlColumnReference Denominator { get; set; }

        /// <summary>
        /// Gets or sets the stratification definitions
        /// </summary>
        [XmlArray("stratify"), XmlArrayItem("by"), JsonProperty("stratify")]
        public List<BiIndicatorMeasureStratifier> Stratifications { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (this.Period == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.measure.period.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Period)), DetectedIssueKeys.InvalidDataIssue);
            }
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
        }

    }
}