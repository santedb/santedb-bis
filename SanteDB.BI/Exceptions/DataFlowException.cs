/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
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
