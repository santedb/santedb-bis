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
 * Date: 2023-5-19
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

        private static IBiMetadataRepository s_repository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();

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
        private static BiDefinition ResolveRefs(BiDefinition definition, Stack<BiDefinition> parentScope)
        {

            if (definition == null)
            {
                return null;
            }
            else if (definition is BiReportViewDefinition) // Report views cannot be resolved
            {
                return definition;
            }
            else if (parentScope.Contains(definition))
            {
                return definition;
            }


            try
            {

                var clonedDefinition = Activator.CreateInstance(definition.GetType()) as BiDefinition;
                clonedDefinition.CopyObjectData(definition);
                parentScope.Push(clonedDefinition);

                if (!String.IsNullOrEmpty(clonedDefinition.Ref))
                {
                    var refId = clonedDefinition.Ref;

                    // Check for already resolved 
                    BiDefinition resolvedTarget = null;
                    if (refId.StartsWith("#")) // identity reference
                    {
                        refId = refId.Substring(1);

                        // Check local definition first by id
                        resolvedTarget = parentScope.Select(o => o.FindObjectById(refId)).OfType<BiDefinition>().FirstOrDefault() ??
                            ResolveRefs(s_repository.GetType().GetGenericMethod(nameof(IBiMetadataRepository.Get),
                                new Type[] { definition.GetType() },
                                new Type[] { typeof(String) }).Invoke(s_repository, new object[] { refId }) as BiDefinition, parentScope);
                        //resolved.Add(clonedDefinition.Ref, resolvedTarget);
                    }
                    else // resolve by local name
                    {
                        resolvedTarget = parentScope.Select(o => o.FindObjectByName(refId)).OfType<BiDefinition>().FirstOrDefault();
                        //resolved.Add(clonedDefinition.Ref, resolvedTarget);

                    }

                    if (resolvedTarget == null)
                    {
                        throw new BiException($"{clonedDefinition.Ref} @ {String.Join("/", parentScope.Reverse().Select(o => o.Name ?? o.Id ?? o.GetType().Name))} not found", definition, null);
                    }


                    // Clone the return value - as we don't want to muck up repository copy of the object
                    var retVal = ResolveRefs(resolvedTarget, parentScope);
                    retVal.Label = definition.Label ?? definition.Label;
                    if (retVal is BiParameterDefinition bid && definition is BiParameterDefinition did)
                    {
                        bid.Required = did.Required;
                    }
                    retVal.Name = definition.Name ?? retVal.Name;

                    // This value is a resolved wrapper - so we want to set its resolved property
                    if (clonedDefinition is BiObjectReference bor)
                    {
                        bor.Resolved = retVal;
                        return clonedDefinition;
                    }
                    else
                    {
                        clonedDefinition.Ref = null;
                        return retVal;
                    }
                }
                else  // Cascade
                {
                    foreach (var pi in clonedDefinition.GetType().GetRuntimeProperties().Where(p => !p.HasCustomAttribute<XmlIgnoreAttribute>() && p.CanWrite))
                    {
                        var val = pi.GetValue(clonedDefinition);
                        if (val is IList)
                        {
                            var nvList = Activator.CreateInstance(val.GetType()) as IList;
                            foreach (var itm in val as IList)
                            {
                                if (itm is BiDefinition bid)
                                {
                                    nvList.Add(ResolveRefs(bid, parentScope));
                                }
                                else if (itm is BiDataFlowCallArgument<BiObjectReference> bica)
                                {
                                    nvList.Add(new BiDataFlowCallArgument<BiObjectReference>()
                                    {
                                        Value = (BiObjectReference)ResolveRefs(bica.Value, parentScope),
                                        Name = bica.Name
                                    });
                                }
                                else
                                {
                                    nvList.Add(itm);
                                }
                            }

                            pi.SetValue(clonedDefinition, nvList);
                        }
                        else if (val is BiDefinition bid)
                        {
                            pi.SetValue(clonedDefinition, ResolveRefs(bid, parentScope));
                        }
                    }
                }


                return clonedDefinition;
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
            catch (BiException be)
            {
                unresolved = be.Definition;
                return false;
            }
        }
    }
}
