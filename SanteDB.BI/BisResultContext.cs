/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Data.Import;
using SanteDB.Core.Data.Import.Format;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace SanteDB.BI
{
    /// <summary>
    /// Represents an execution context containing results
    /// </summary>
    public class BisResultContext : IDisposable
    {

        private static readonly IForeignDataFormat m_csvFormat = new CsvForeignDataFormat();

        /// <summary>
        /// Create a new result context
        /// </summary>
        public BisResultContext(BiReferenceDataSourceDefinition rawDefinition)
        {
            this.Records = this.IterateRawDefinition(rawDefinition).ToArray();
        }

        /// <summary>
        /// Iterate the raw definitions
        /// </summary>
        private IEnumerable<dynamic> IterateRawDefinition(BiReferenceDataSourceDefinition brd)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(brd.Data.Trim())))
            using (var file = m_csvFormat.Open(ms))
            using (var rdr = file.CreateReader())
            {

                while (rdr.MoveNext())
                {
                    IDictionary<string, object> record = new ExpandoObject();
                    for (var col = 0; col < rdr.ColumnCount; col++)
                    {
                        record.Add(rdr.GetName(col), rdr[col]);
                    }
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Creates a new result context
        /// </summary>
        public BisResultContext(BiQueryDefinition definition,
            IDictionary<String, Object> arguments,
            IBiDataSource source,
            IEnumerable<dynamic> results,
            DateTime startTime
            )
        {
            this.Arguments = arguments;
            this.Records = results;
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
        public IEnumerable<dynamic> Records { get; private set; }

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

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            if (this.Records is IDisposable disp)
            {
                disp.Dispose();
            }
        }
    }
}
