/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
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
        public static BiDefinition ResolveRefs(BiDefinition definition)
        {
            // Create new instance
            if (definition == null)
                return null;
            else if (definition is BiReportViewDefinition) // Report views cannot be resolved
                return definition;
            var newDef = Activator.CreateInstance(definition.GetType()) as BiDefinition;
            newDef.CopyObjectData(definition);
            definition = newDef;

            // Reference?
            if (!String.IsNullOrEmpty(definition.Ref))
            {
                var refId = definition.Ref;
                definition.Ref = null;
                if (refId.StartsWith("#")) refId = refId.Substring(1);

                var repository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
                // Attempt to lookup
                var retVal = ResolveRefs(repository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Get),
                    new Type[] { definition.GetType() },
                    new Type[] { typeof(String) }).Invoke(repository, new object[] { refId }) as BiDefinition);
                if (retVal == null)
                    throw new KeyNotFoundException($"{newDef.GetType().Name} #{refId} does not exist or you do not have permission");
                return retVal;
            }
            else  // Cascade
                foreach (var pi in definition.GetType().GetRuntimeProperties())
                {
                    var val = pi.GetValue(definition);
                    if (val is IList)
                    {
                        var nvList = Activator.CreateInstance(val.GetType()) as IList;
                        foreach (var itm in val as IList)
                            if (itm is BiDefinition)
                                nvList.Add(ResolveRefs(itm as BiDefinition));
                            else
                                nvList.Add(itm);
                        pi.SetValue(definition, nvList);
                    }
                    else if (val is BiDefinition)
                        pi.SetValue(definition, ResolveRefs(val as BiDefinition));
                }
            return definition;
        }
    }
}
