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
        /// Pivot provider
        /// </summary>
        public BisResultContext Pivot(BisResultContext context, BiViewPivotDefinition pivot)
        {
            var buckets = new List<ExpandoObject>();
            // First we must order by the pivot 
            context.Dataset.OrderBy(o => (o as IDictionary<String, Object>)[pivot.Key]);

            // Algorithm for pivoting : 
            IDictionary<String, Object> cobject = null;
            foreach (IDictionary<String, Object> itm in context.Dataset)
            {
                var key = itm[pivot.Key];
                if (cobject == null || !key.Equals(cobject[pivot.Key]) && cobject.Count > 0)
                {
                    cobject = new ExpandoObject();
                    cobject.Add(pivot.Key, key);
                    buckets.Add(cobject as ExpandoObject);
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
            var colNames = new List<String>();
            foreach (IDictionary<String, Object> itm in buckets)
                foreach (var value in itm)
                {
                    if (value.Key == pivot.Key) continue; // Don't de-bucket the object

                    var bucket = (value.Value as IEnumerable<Object>);
                    itm[value.Key] = this.Aggregate((value.Value as List<Object>), pivot.AggregateFunction);
                    if(!colNames.Contains(value.Key)) colNames.Add(value.Key);
                }

            // Add where necessary
            var output = new List<ExpandoObject>();
            foreach (IDictionary<String, Object> itm in buckets)
            {
                var tuple = (new ExpandoObject() as IDictionary<String, Object>);
                tuple[pivot.Key] = itm[pivot.Key];
                foreach (var col in colNames)
                    if (itm.ContainsKey(col))
                        tuple[col] = itm[col];
                    else
                        tuple[col] = null;
                output.Add(tuple as ExpandoObject);
            }

            return new BisResultContext(context.QueryDefinition, context.Arguments, context.DataSource, output, context.StartTime.DateTime);
        }

    }
}
