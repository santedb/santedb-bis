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
using Microsoft.CSharp.RuntimeBinder;
using SanteDB.BI.Model;
using SanteDB.Core.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// An in-memory dataset pivot provider
    /// </summary>
    public class InMemoryPivotProvider : IBiPivotProvider
    {

        /// <summary>
        /// Aggegrate the <paramref name="bucket"/> according to <paramref name="aggregateFunction"/>
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="aggregateFunction"></param>
        /// <returns></returns>
        public object Aggregate(IEnumerable<Object> bucket, BiAggregateFunction aggregateFunction)
        {

            String aggFn = String.Empty;
            switch (aggregateFunction)
            {
                case BiAggregateFunction.Average:
                    aggFn = nameof(Enumerable.Average);
                    break;
                case BiAggregateFunction.Count:
                    aggFn = nameof(Enumerable.Count);
                    break;
                case BiAggregateFunction.First:
                    return bucket.FirstOrDefault();
                case BiAggregateFunction.Last:
                    return bucket.LastOrDefault();
                case BiAggregateFunction.Max:
                    aggFn = nameof(Enumerable.Max);
                    break;
                case BiAggregateFunction.Min:
                    aggFn = nameof(Enumerable.Min);
                    break;
                case BiAggregateFunction.Sum:
                    aggFn = nameof(Enumerable.Sum);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot apply aggregate function {aggregateFunction} on pivot");
            }

            // For numeric types we have to use reflection
            var ftype = bucket.First().GetType();
            var aggMethod = typeof(Enumerable).GetGenericMethod(aggFn,
                new Type[] { ftype },
                new Type[] { typeof(IEnumerable<>).MakeGenericType(ftype), typeof(Func<,>).MakeGenericType(ftype, ftype) });

            // We want to convert all values 
            var convertMethod = typeof(Enumerable).GetGenericMethod(nameof(Enumerable.Select),
                new Type[] { typeof(Object), ftype },
                new Type[] { typeof(IEnumerable<Object>), typeof(Func<,>).MakeGenericType(typeof(Object), ftype) });
            var parm = Expression.Parameter(typeof(Object));
            var selectorFn = Expression.Lambda(Expression.Convert(parm, ftype), parm);
            var converted = convertMethod.Invoke(null, new object[] { bucket, selectorFn.Compile() });

            // Now run the aggregation on the converted array
            parm = Expression.Parameter(ftype);
            selectorFn = Expression.Lambda(parm, parm);
            return aggMethod.Invoke(null, new object[] { converted, selectorFn.Compile() });
        }

        /// <summary>
        /// Perform a pivot internally
        /// </summary>
        public IEnumerable<dynamic> Pivot(IEnumerable<dynamic> sourceRecords, BiViewPivotDefinition pivot)
        {
            if (sourceRecords is IOrderableQueryResultSet iqrs)
            {
                var parm = Expression.Parameter(typeof(object));
                CallSiteBinder keyBinder = Binder.GetMember(CSharpBinderFlags.None, pivot.Key, typeof(InMemoryPivotProvider), new CSharpArgumentInfo[0]);
                var sortExpression = Expression.Lambda(Expression.Dynamic(keyBinder, typeof(object), parm), parm);
                sourceRecords = iqrs.OrderBy(sortExpression).OfType<dynamic>();
            }

            IDictionary<String, Object> currentTuple = null;
            foreach (IDictionary<String, Object> tuple in sourceRecords)
            {
                var keyValue = tuple[pivot.Key];
                if (currentTuple == null)
                {
                    currentTuple = new ExpandoObject();
                    currentTuple.Add(pivot.Key, keyValue);
                }
                else if (!keyValue.Equals(currentTuple[pivot.Key]))
                {
                    yield return this.AggregateTuple(currentTuple, pivot);

                    currentTuple = new ExpandoObject();
                    currentTuple.Add(pivot.Key, keyValue);
                }

                // Add extra columns that are in the result set but not the column selector, key, or value
                foreach (var col in tuple.Where(o => o.Key != pivot.Key && o.Key != pivot.Value && o.Key != pivot.ColumnSelector && !pivot.Columns.Contains(o.Key)))
                {
                    if (currentTuple.ContainsKey(col.Key))
                    {
                        currentTuple[col.Key] = col.Value;
                    }
                    else
                    {
                        currentTuple.Add(col.Key, col.Value);
                    }
                }

                // Grouping / aggregator columns
                var columnName = tuple[pivot.ColumnSelector]?.ToString();
                if (!currentTuple.TryGetValue(columnName, out var items))
                {
                    items = new List<Object>() { tuple[pivot.Value] };
                    currentTuple.Add(columnName, items);
                }
                else if (items is IList list)
                {
                    list.Add(tuple[pivot.Value]);
                }
                else
                {
                    currentTuple[columnName] = tuple[pivot.Value];
                }



            }

            if (currentTuple != null)
            {
                yield return this.AggregateTuple(currentTuple, pivot);
            }


        }

        /// <summary>
        /// Aggreate the specified tuple
        /// </summary>
        private dynamic AggregateTuple(IDictionary<string, object> tupleToAggregate, BiViewPivotDefinition pivot)
        {
            // Fill in declared categories
            if (pivot.Columns?.Any() == true)
            {
                pivot.Columns.ForEach(c =>
                {
                    if (!tupleToAggregate.ContainsKey(c))
                    {
                        tupleToAggregate.Add(c, null);
                    }
                });
            }
            return tupleToAggregate.GroupBy(o => o.Key, o =>
            {
                if (o.Key == pivot.Key)
                {
                    return o.Value;
                }
                else if (o.Value is IEnumerable<Object> list)
                {
                    return this.Aggregate(list, pivot.AggregateFunction);
                }
                else
                {
                    return o.Value;
                }
            }).ToDictionary(o => o.Key, o => o.First());
        }

        /// <summary>
        /// Pivot provider
        /// </summary>
        public BisResultContext Pivot(BisResultContext context, BiViewPivotDefinition pivot)
        {
            return new BisResultContext(context.QueryDefinition, context.Arguments, context.DataSource, this.Pivot(context.Records, pivot), context.StartTime.DateTime);
        }

    }
}
