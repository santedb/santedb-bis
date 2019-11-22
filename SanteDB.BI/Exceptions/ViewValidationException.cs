using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SanteDB.BI.Exceptions
{

    /// <summary>
    /// Represents a view validation exception
    /// </summary>
    public class ViewValidationException : Exception
    {

        /// <summary>
        /// Gets the element on which the validation failed
        /// </summary>
        public XElement Element { get; }

        /// <summary>
        /// Ctor with just element
        /// </summary>
        public ViewValidationException(XElement element) : this(element, null, null)
        {
        }

        /// <summary>
        /// Creates a new exception with specified message
        /// </summary>
        public ViewValidationException(XElement element, String message) : this(element, message,  null)
        {
        }

        /// <summary>
        /// Creates a new exception with specified message and cause
        /// </summary>
        public ViewValidationException(XElement element, String message, Exception innerException) : base(message, innerException)
        {
            this.Element = element;
        }
    }

}
