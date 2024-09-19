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
 */
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Model.Query;
using System;
using System.Linq.Expressions;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// BI metadata extensions
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal static class BiMetadataExtensions
    {

        /// <summary>
        /// Convert a non-generic query to the proper invokation
        /// </summary>
        public static IQueryResultSet Query(this IBiMetadataRepository me, Type definitionType, Expression<Func<BiDefinition, bool>> expression)
        {
            var convertedExpression = QueryExpressionParser.BuildLinqExpression(definitionType, QueryExpressionBuilder.BuildQuery(expression));
            var mi = me.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Query), new Type[] { definitionType }, new Type[] { convertedExpression.GetType() });
            return mi.Invoke(me, new object[] { convertedExpression }) as IQueryResultSet;
        }
    }
}
