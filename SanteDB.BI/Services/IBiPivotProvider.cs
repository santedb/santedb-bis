﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a PIVOT provider which can take a dataset and pivot it
    /// </summary>
    public interface IBiPivotProvider
    {

        /// <summary>
        /// Pivots <paramref name="context"/> in place returning it for chaining
        /// </summary>
        /// <param name="context">The result context to be pivoted</param>
        /// <param name="pivot">The pivot definition to apply</param>
        /// <returns>The pivoted context</returns>
        BisResultContext Pivot(BisResultContext context, BiViewPivotDefinition pivot);

    }
}
