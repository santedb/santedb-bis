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
using DocumentFormat.OpenXml.Wordprocessing;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public String StratifierPath { get; }

        /// <summary>
        /// Get all identifiers on the current result set
        /// </summary>
        /// <returns>The identity of the appropriate stratum</returns>
        public List<BiIdentity> GetResultSetIdentifiers()
        {
            var stratumPath = new Queue<String>(this.StratifierPath.Split('/').Skip(1));
            if(!stratumPath.Any())
            {
                return this.Measure.Identifier;
            }
            else
            {
                var rootStratumName = stratumPath.Dequeue();
                var searchStratum = this.Measure.Stratifiers.Find(o => o.Name == rootStratumName); // First represents the first stratifier
                while(stratumPath.Any())
                {
                    if (searchStratum.ThenBy.Name == stratumPath.Peek())
                    {
                        searchStratum = searchStratum.ThenBy;
                        stratumPath.Dequeue();
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(ErrorMessages.DATA_STRUCTURE_NOT_APPROPRIATE, nameof(BiIndicatorMeasureStratifier.ThenBy), $" the stratum name {searchStratum.ThenBy.Name} does not match expected value of {stratumPath.Peek()}"));
                    }

                }
                return searchStratum.Identifier;
            }
        }
    }

}
