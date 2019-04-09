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
        /// <summary>
        /// Gets the arguments used 
        /// </summary>
        public IDictionary<String, Object> Arguments { get; private set; }

        /// <summary>
        /// Gets the query definition
        /// </summary>
        public BisQueryDefinition QueryDefinition { get; private set; }

        /// <summary>
        /// Gets the dataset
        /// </summary>
        public IEnumerable<dynamic> Dataset { get; private set; }

        /// <summary>
        /// Gets the data source
        /// </summary>
        public IBisDataSource DataSource { get; private set; }

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
