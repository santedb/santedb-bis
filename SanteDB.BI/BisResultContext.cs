/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI
{
    /// <summary>
    /// Represents an execution context containing results
    /// </summary>
    public class BisResultContext
    {

        
        public BisResultContext(BiQueryDefinition definition, 
            IDictionary<String, Object> arguments,
            IBiDataSource source,
            IEnumerable<dynamic> results,
            DateTime startTime
            )
        {
            this.Arguments = arguments;
            this.Dataset = results;
            this.DataSource = source;
            this.QueryDefinition = definition;
            this.StartTime = startTime;
            this.StopTime = DateTime.Now;
        }

        /// <summary>
        /// Gets the arguments used 
        /// </summary>
        public IDictionary<String, Object> Arguments { get; private set; }

        /// <summary>
        /// Gets the query definition
        /// </summary>
        public BiQueryDefinition QueryDefinition { get; private set; }

        /// <summary>
        /// Gets the dataset
        /// </summary>
        public IEnumerable<dynamic> Dataset { get; private set; }

        /// <summary>
        /// Gets the data source
        /// </summary>
        public IBiDataSource DataSource { get; private set; }

        /// <summary>
        /// Gets the start time
        /// </summary>
        public DateTimeOffset StartTime { get; private set; }

        /// <summary>
        /// Gets the stop time
        /// </summary>
        public DateTimeOffset StopTime { get; private set; }


    }
}
