﻿/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using System;
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
        public ViewValidationException(XElement element, String message) : this(element, message, null)
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
