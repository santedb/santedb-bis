﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a rendering context
    /// </summary>
    public interface IRenderContext
    {

        /// <summary>
        /// Gets the parent of this element
        /// </summary>
        IRenderContext Parent { get; }

        /// <summary>
        /// Gets the root of this element
        /// </summary>
        IRenderContext Root { get; }

        /// <summary>
        /// Gets the scope object
        /// </summary>
        dynamic ScopedObject { get; }

        /// <summary>
        /// Get a report tag
        /// </summary>
        IDictionary<String, Object> Tags { get; }
    }
}
