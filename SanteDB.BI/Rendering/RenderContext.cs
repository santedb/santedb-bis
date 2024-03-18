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
 * User: fyfej
 * Date: 2023-6-21
 */
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a report rendering context implementation linked to a report
    /// </summary>
    public class RenderContext : IRenderContext
    {
        /// <summary>
        /// Creates a new child rendering context
        /// </summary>
        public RenderContext(IRenderContext parent, dynamic scopedObject)
        {
            this.Parent = parent;
            this.ScopedObject = scopedObject;
            this.Tags = new Dictionary<String, Object>();
        }

        /// <summary>
        /// Gets the root
        /// </summary>
        public IRenderContext Root => this.Parent?.Root ?? this;

        /// <summary>
        /// Gets the parent context
        /// </summary>
        public IRenderContext Parent { get; }

        /// <summary>
        /// Gets the scoped object
        /// </summary>
        public dynamic ScopedObject { get; }

        /// <summary>
        /// Report watches for this instance of the report
        /// </summary>
        public IDictionary<String, Object> Tags { get; }
    }
}