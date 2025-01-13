using SanteDB.BI.Model;
using SanteDB.BI.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI
{
    /// <summary>
    /// An result context which was created as a result of executing an indicator
    /// </summary>
    public class BisIndicatorMeasureResultContext : BisResultContext
    {
        public BisIndicatorMeasureResultContext(
            BiIndicatorDefinition indicatorDefinition,
            BiIndicatorMeasureDefinition measureDefinition,
            String measureOrStratifierName,
            IDictionary<string, object> arguments, 
            IBiDataSource dataSource, 
            IEnumerable<dynamic> results, 
            DateTime startTime
        ) : base(indicatorDefinition.Query, arguments, dataSource, results, startTime)
        {
            this.Measure = measureDefinition;
            this.Indicator = indicatorDefinition;
            this.StratifierPath = measureOrStratifierName;
        }

        /// <summary>
        /// Gets the measure that generated this result set
        /// </summary>
        public BiIndicatorMeasureDefinition Measure { get; }

        /// <summary>
        /// Gets the indicator definition that this result context is based on
        /// </summary>
        public BiIndicatorDefinition Indicator { get; }
        /// <summary>
        /// Mesure and stratification names
        /// </summary>
        public string StratifierPath { get; }
    }
}
