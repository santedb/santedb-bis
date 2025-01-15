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