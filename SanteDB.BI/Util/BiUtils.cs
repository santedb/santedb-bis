using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanteDB.Core.Model;
using System.Reflection;
using System.Collections;

namespace SanteDB.BI.Util
{
    /// <summary>
    /// BI utilities
    /// </summary>
    public static class BiUtils
    {


        /// <summary>
        /// Resolves all references to their proper objects
        /// </summary>
        public static BisDefinition ResolveRefs(BisDefinition definition)
        {
            // Create new instance
            if (definition == null)
                return null;
            var newDef = Activator.CreateInstance(definition.GetType()) as BisDefinition;
            newDef.CopyObjectData(definition);
            definition = newDef;

            // Reference?
            if (!String.IsNullOrEmpty(definition.Ref))
            {
                var refId = definition.Ref;
                definition.Ref = null;
                if (refId.StartsWith("#")) refId = refId.Substring(1);

                var repository = ApplicationServiceContext.Current.GetService<IBisMetadataRepository>();
                // Attempt to lookup
                return ResolveRefs(repository.GetType().GetGenericMethod(nameof(IBisMetadataRepository.Get),
                    new Type[] { definition.GetType() },
                    new Type[] { typeof(String) }).Invoke(repository, new object[] { refId }) as BisDefinition);
            }
            else  // Cascade
                foreach (var pi in definition.GetType().GetRuntimeProperties())
                {
                    var val = pi.GetValue(definition);
                    if (val is IList)
                    {
                        var nvList = Activator.CreateInstance(val.GetType()) as IList;
                        foreach (var itm in val as IList)
                            if (itm is BisDefinition)
                                nvList.Add(ResolveRefs(itm as BisDefinition));
                            else
                                nvList.Add(itm);
                        pi.SetValue(definition, nvList);
                    }
                }
            return definition;
        }
    }
}
