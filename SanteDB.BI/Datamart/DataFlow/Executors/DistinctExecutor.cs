using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Distinct executor
    /// </summary>
    internal class DistinctExecutor : DataStreamExecutorBase<BiDataFlowDistinctStep>
    {
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowDistinctStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {
            var hsKey = new HashSet<String>();

            foreach(DataFlowStreamTuple itm in inputStream.Select(o=>this.CreateStreamTuple(o)))
            {
                bool skip = false;
                foreach(var don in flowStep.DistinctOn)
                {
                    var k = $"{don}={itm.GetData(don)}";
                    if (hsKey.Contains(k))
                    {
                        skip = true;
                        break;
                    }
                    hsKey.Add(k);
                }
                if(!skip)
                {
                    yield return itm;
                }
            }
        }
    }
}
