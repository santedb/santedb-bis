using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI.Exceptions
{
    /// <summary>
    /// Report rendering exception
    /// </summary>
    public class BiException : Exception
    {

        /// <summary>
        /// The object which caused the exception
        /// </summary>
        public BiDefinition Definition { get; }

        /// <summary>
        /// Creates a new BI definition
        /// </summary>
        public BiException(String message, BiDefinition definition, Exception innerException) : base(message, innerException)
        {
            this.Definition = definition;
        }
    }
}
