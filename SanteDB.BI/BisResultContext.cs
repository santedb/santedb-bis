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
