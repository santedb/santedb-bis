using SanteDB.BI.Model;
using System;

namespace SanteDB.BI.Exceptions
{
    /// <summary>
    /// An exception has occurred during a data flow execution
    /// </summary>
    public class DataFlowException : Exception
    {

        /// <summary>
        /// Create a new data flow exception
        /// </summary>
        /// <param name="stepAtException">The current step when the exception occurred</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public DataFlowException(BiDataFlowStep stepAtException, Exception innerException) : base($"BI Error @{stepAtException.Name ?? stepAtException.Id}", innerException)
        {
            this.FlowObject = stepAtException;
        }

        /// <summary>
        /// Create a new data flow exception
        /// </summary>
        /// <param name="stepAtException">The current step when the exception occurred</param>
        /// <param name="message">The message for this error</param>
        public DataFlowException(BiDataFlowStep stepAtException, String message) : base($"BI Error @{stepAtException.Name ?? stepAtException.Id} - {message}")
        {
            this.FlowObject = stepAtException;
        }

        /// <summary>
        /// Gets the flow object which caused the exception
        /// </summary>
        public BiDataFlowStep FlowObject { get; }
    }
}
