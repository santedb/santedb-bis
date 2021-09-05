/*
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
using SanteDB.BI.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Util
{
    /// <summary>
    /// Represents an enumerator for a BiPackage
    /// </summary>
    public class BiPackageEnumerator : IEnumerable<BiDefinition>
    {
        // The package
        private BiPackage m_package;

        /// <summary>
        /// Creates a new BI Package
        /// </summary>
        public BiPackageEnumerator(BiPackage package)
        {
            this.m_package = package;
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<BiDefinition> GetEnumerator()
        {
            return this.m_package.DataSources.OfType<BiDefinition>()
                .Union(this.m_package.Formats.OfType<BiDefinition>())
                .Union(this.m_package.Parameters.OfType<BiDefinition>())
                .Union(this.m_package.Queries.OfType<BiDefinition>())
                .Union(this.m_package.Reports.OfType<BiDefinition>())
                .Union(this.m_package.Views.OfType<BiDefinition>()).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
