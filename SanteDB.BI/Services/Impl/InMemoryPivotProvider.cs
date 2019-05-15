using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SanteDB.BI.Model;
using SanteDB.Core.Model;

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
            switch(aggregateFunction)
            {
                case BiAggregateFunction.Average:
                    aggFn = nameof(Enumerable.Average);
                    break;
                case BiAggregateFunction.Count:
                    aggFn = nameof(Enumerable.Count);
                    break;
                case BiAggregateFunction.First:
                    aggFn = nameof(Enumerable.First);
                    break;
                case BiAggregateFunction.Last:
                    aggFn = nameof(Enumerable.Last);
                    break;
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

            var ftype = bucket.First().GetType();
            var aggMethod = typeof(Enumerable).GetGenericMethod(aggFn,
                new Type[] { ftype.GetType() },
                new Type[] { typeof(IEnumerable<>).MakeGenericType(ftype), typeof(Func<,>).MakeGenericType(ftype, ftype) });

            // We want to convert all values 
            var convertMethod = typeof(Enumerable).GetGenericMethod(nameof(Enumerable.Select),
                new Type[] { typeof(Object) },
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
        /// Pivot provider
        /// </summary>
        public BisResultContext Pivot(BisResultContext context, BiViewPivotDefinition pivot)
        {
            var buckets = new List<ExpandoObject>();
            // Algorithm for pivoting : 
            IDictionary<String, Object> cobject = null;
            foreach (IDictionary<String, Object> itm in context.Dataset)
            {
                var key = itm[pivot.Key];
                if (cobject == null || !key.Equals(cobject[pivot.Key]) && cobject.Count > 0)
                {
                    if (cobject != null)
                        buckets.Add(cobject as ExpandoObject);
                    cobject = new ExpandoObject();
                    cobject.Add(pivot.Key, key);
                }

                // Same key, so lets create or accumulate
                var column = itm[pivot.Columns];
                if (!cobject.ContainsKey(column.ToString()))
                    cobject.Add(column.ToString(), new List<Object>() { itm[pivot.Value] });
                else
                {
                    var cvalue = cobject[column.ToString()] as List<Object>;
                    var avalue = itm[pivot.Value];
                    cvalue.Add(avalue);
                }
            }

            // Now we have our buckets, we want to apply our aggregation function
            foreach (IDictionary<String, Object> itm in buckets)
                foreach (var value in itm)
                {
                    if (value.Key == pivot.Key) continue; // Don't de-bucket the object

                    var bucket = (value.Value as IEnumerable<Object>);
                    itm[value.Key] = this.Aggregate((value.Value as List<Object>), pivot.AggregateFunction);
                }
            return new BisResultContext(context.QueryDefinition, context.Arguments, context.DataSource, buckets, context.StartTime.DateTime);
        }

        private T List<T>(T v)
        {
            throw new NotImplementedException();
        }
    }
}
