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
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// A class for a "current scope" in which a particular data flow object is operating
    /// </summary>
    internal class DataFlowScope : IDisposable, IDataIntegratorVariableProvider
    {

        /// <summary>
        /// Flow scope variable metadata
        /// </summary>
        private struct BiFlowScopeVariable
        {

            // The value
            private dynamic m_value;

            /// <summary>
            /// Create a new scoped variable
            /// </summary>
            public BiFlowScopeVariable(bool constant, dynamic value)
            {
                this.Readonly = constant;
                this.m_value = value;
            }

            /// <summary>
            /// True if the variable is constant
            /// </summary>
            public bool Readonly { get; }

            /// <summary>
            /// The value of the variable
            /// </summary>
            public dynamic Value
            {
                get => this.m_value;
                set
                {
                    if (this.Readonly)
                    {
                        throw new InvalidOperationException(ErrorMessages.OBJECT_READONLY);
                    }
                    this.m_value = value;
                }
            }

        }

        private readonly DataFlowScope m_parent;
        private readonly IDictionary<string, BiFlowScopeVariable> m_variables;

        /// <summary>
        /// Creates a new root scope
        /// </summary>
        /// <param name="name">The name for this scope</param>
        /// <param name="executionContext">The execution context for this scope</param>
        public DataFlowScope(String name, IDataFlowExecutionContext executionContext)
        {
            this.Name = name;
            this.m_parent = null;
            this.m_variables = new Dictionary<String, BiFlowScopeVariable>();
            this.Context = executionContext;
        }

        /// <summary>
        /// Creates a new flow scope
        /// </summary>
        /// <param name="name">The name for this scope</param>
        /// <param name="parentVisibleScope">The parent of the scope (note: all variables in the parent are visible in this scope)</param>
        public DataFlowScope(String name, DataFlowScope parentVisibleScope)
        {
            this.Name = name;
            this.m_parent = parentVisibleScope;
            this.m_variables = new Dictionary<String, BiFlowScopeVariable>();
            this.Context = parentVisibleScope.Context;
        }

        /// <summary>
        /// Get the execution context
        /// </summary>
        public IDataFlowExecutionContext Context { get; }


        /// <summary>
        /// Get the root scope
        /// </summary>
        internal DataFlowScope Root
        {
            get
            {
                var rt = this;
                while (rt.m_parent != null)
                {
                    rt = rt.m_parent;
                }
                return rt;
            }
        }

        /// <summary>
        /// Get the variable as an indexer
        /// </summary>
        public dynamic this[string name]
        {
            get => this.GetVariable(name);
            set => this.SetVariable(name, value);
        }

        /// <summary>
        /// Gets the name of the scope
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Get variable from this or parent context
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable value</returns>
        /// <exception cref="ArgumentException">If the variable cannot be found</exception>
        internal dynamic GetVariable(String name)
        {
            if (!this.IsVisible(name))
            {
                throw new ArgumentException(name);
            }
            if (!this.m_variables.TryGetValue(name, out var retVal))
            {
                return this.m_parent?.GetVariable(name);
            }
            return retVal.Value;
        }

        /// <summary>
        /// Get variable from this or parent context
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <typeparam name="T">The type of data to return</typeparam>
        /// <returns>The variable value</returns>
        /// <exception cref="ArgumentException">If the variable cannot be found</exception>
        internal T GetVariable<T>(String name) => (T)this.GetVariable(name);

        /// <summary>
        /// True if the variable is visible to this scope
        /// </summary>
        /// <param name="name">The name of the variable</param>
        public bool IsVisible(String name) => this.m_variables.ContainsKey(name) || this.m_parent?.IsVisible(name) == true;

        /// <summary>
        /// Declare a variable
        /// </summary>
        /// <exception cref="ArgumentException">The varaible has already been declared</exception>
        /// <param name="initialValue">The initial value to set</param>
        /// <param name="name">The name of the scope variable</param>
        internal void DeclareVariable(String name, dynamic initialValue)
        {
            if (this.IsVisible(name))
            {
                throw new ArgumentException(name);
            }
            else
            {
                this.m_variables.Add(name, new BiFlowScopeVariable(false, initialValue));
            }
        }

        /// <summary>
        /// Declare a variable
        /// </summary>
        /// <exception cref="ArgumentException">The varaible has already been declared</exception>
        /// <param name="initialValue">The initial value to set</param>
        /// <param name="name">The name of the scope variable</param>
        internal void DeclareConstant(String name, dynamic initialValue)
        {
            if (this.IsVisible(name))
            {
                throw new ArgumentException(name);
            }
            else
            {
                this.m_variables.Add(name, new BiFlowScopeVariable(true, initialValue));
            }
        }

        /// <summary>
        /// Sets the variable value on this scope level
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value of the variable</param>
        /// <exception cref="ArgumentException">If the variable is not declared</exception>
        internal void SetVariable(String name, dynamic value)
        {
            if (this.m_variables.TryGetValue(name, out var valStruct))
            {
                if(valStruct.Readonly)
                {
                    throw new InvalidOperationException(String.Format(ErrorMessages.OBJECT_READONLY, name));
                }
                valStruct.Value = value;
            }
            else if (this.m_parent != null)
            {
                this.m_parent.SetVariable(name, value);
            }
            else
            {
                throw new ArgumentException(name);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (this.m_parent == null)
            {
                foreach (var itm in this.m_variables.ToArray())
                {
                    if (itm.Value.Value is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                    this.m_variables.Remove(itm.Key);
                }
            }
        }

        /// <inheritdoc/>
        bool IDataIntegratorVariableProvider.TryGetVariable(string variableName, out dynamic value)
        {
            if(this.m_variables.TryGetValue(variableName, out var valueDef))
            {
                value = valueDef.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
