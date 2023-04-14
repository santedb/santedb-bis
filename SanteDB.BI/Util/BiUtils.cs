/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

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
        public static TBiDefinition ResolveRefs<TBiDefinition>(TBiDefinition definition)
            where TBiDefinition : BiDefinition
        {
            return (TBiDefinition)ResolveRefs((BiDefinition)definition, new Stack<BiDefinition>());
        }

        /// <summary>
        /// Resolves all references and their proper objects from the metadata repository
        /// </summary>
        public static BiDefinition ResolveRefs(BiDefinition definition, Stack<BiDefinition> parentScope)
        {

            // Create new instance
            if (definition == null)
            {
                return null;
            }
            else if (definition is BiReportViewDefinition) // Report views cannot be resolved
            {
                return definition;
            }

            try
            {
                parentScope.Push(definition);

                var localName = definition.Name;
                var localLabel = definition.Label;
                var localRequired = (definition as BiParameterDefinition)?.Required;
                var newDef = Activator.CreateInstance(definition.GetType()) as BiDefinition;
                newDef.CopyObjectData(definition);
                definition = newDef;

                // Reference?
                if (!String.IsNullOrEmpty(definition.Ref))
                {
                    var refId = definition.Ref;
                    definition.Ref = null;
                    BiDefinition retVal = null;
                    if (refId.StartsWith("#"))
                    {
                        refId = refId.Substring(1);

                        var repository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
                        // Attempt to lookup
                        retVal = ResolveRefs(repository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Get),
                            new Type[] { definition.GetType() },
                            new Type[] { typeof(String) }).Invoke(repository, new object[] { refId }) as BiDefinition) ??
                            parentScope.Select(o=>o.FindObjectById(refId)).OfType<BiDefinition>().FirstOrDefault();
                    }
                    else
                    {
                        retVal = parentScope.Select(o => o.FindObjectByName(refId)).OfType<BiDefinition>().FirstOrDefault();
                    }
                    if (retVal == null)
                    {
                        throw new Exceptions.BiException($"{newDef.GetType().Name} #{refId} does not exist or you do not have permission", definition, null);
                    }

                    if (!String.IsNullOrEmpty(localName))
                    {
                        retVal.Name = localName;
                    }

                    if (!String.IsNullOrEmpty(localLabel))
                    {
                        retVal.Label = localLabel;
                    }

                    if (localRequired.HasValue)
                    {
                        (retVal as BiParameterDefinition).Required = localRequired.Value;
                    }

                    return retVal;
                }
                else  // Cascade
                {
                    foreach (var pi in definition.GetType().GetRuntimeProperties().Where(p=>!p.HasCustomAttribute<XmlIgnoreAttribute>()))
                    {
                        var val = pi.GetValue(definition);
                        if (val is IList)
                        {
                            var nvList = Activator.CreateInstance(val.GetType()) as IList;
                            foreach (var itm in val as IList)
                            {
                                if (itm is BiDefinition bid)
                                {
                                    nvList.Add(ResolveRefs(bid, parentScope));
                                }
                                else
                                {
                                    nvList.Add(itm);
                                }
                            }

                            pi.SetValue(definition, nvList);
                        }
                        else if(val is BiObjectReference bsor)
                        {
                            bsor.Resolved = ResolveRefs(bsor, parentScope);
                        }
                        else if (val is BiDefinition bid)
                        {
                            pi.SetValue(definition, ResolveRefs(bid, parentScope));
                        }
                    }
                }

                if (!String.IsNullOrEmpty(localName))
                {
                    definition.Name = localName;
                }

                return definition;
            }
            finally
            {
                parentScope.Pop();
            }
        }

        /// <summary>
        /// Try to resolve all the references in <paramref name="biDefinition"/>
        /// </summary>
        /// <param name="biDefinition">The BI definition that should be resolved</param>
        /// <param name="unresolved">The unresolved reference</param>
        internal static bool CanResolveRefs(BiDefinition biDefinition, out BiDefinition unresolved)
        {
            try
            {
                ResolveRefs(biDefinition);
                unresolved = null;
                return true;
            }
            catch(BiException be)
            {
                unresolved = be.Definition;
                return false;
            }
        }
    }
}
