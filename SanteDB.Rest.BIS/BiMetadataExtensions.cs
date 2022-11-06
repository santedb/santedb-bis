using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// BI metadata extensions
    /// </summary>
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
