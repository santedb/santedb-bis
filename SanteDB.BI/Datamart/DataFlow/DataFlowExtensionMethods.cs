using SanteDB;
using SanteDB.BI.Model;
using SanteDB.Core;
using SanteDB.Core.i18n;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static SanteDB.BI.Model.BiDataFlowDefinition;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Data flow extension methods
    /// </summary>
    internal static class DataFlowExtensionMethods
    {

        // Executors
        private static readonly IDictionary<Type, IDataFlowStepExecutor> m_executors;

        /// <summary>
        /// Static CTOR
        /// </summary>
        static DataFlowExtensionMethods()
        {
            var svcm = ApplicationServiceContext.Current.GetService<IServiceManager>();
            m_executors = typeof(DataFlowExtensionMethods)
                .Assembly
                .GetTypes()
                .Where(t => typeof(IDataFlowStepExecutor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => svcm.CreateInjected(t) as IDataFlowStepExecutor)
                .ToDictionary(o => o.Handles, o => o);
        }

        /// <summary>
        /// Get an executor
        /// </summary>
        /// <param name="me">The flow step to get the executor logic for</param>
        /// <returns>The executor or null if none is registered</returns>
        public static IDataFlowStepExecutor GetExecutor(this BiDataFlowStep me) => m_executors.TryGetValue(me.GetType(), out var retVal) ? retVal : null;

        /// <summary>
        /// Execute the specified flow step
        /// </summary>
        /// <param name="me">The flow step to execute</param>
        /// <param name="context">The context in which the flow step is executing (the ETL job)</param>
        /// <param name="scope">The scope of the current DataFlow, Paralell, Transaction, etc.</param>
        /// <returns>The result of the execution</returns>
        public static IEnumerable<dynamic> Execute(this BiDataFlowStep me, DataFlowScope scope)
        {
            var executor = me.GetExecutor();
            if (executor != null)
            {
                return executor.Execute(me, scope);
            }
            throw new MissingMethodException(String.Format(ErrorMessages.METHOD_NOT_FOUND, me.GetType().GetSerializationName()));
        }

        /// <summary>
        /// Get the execution root objects
        /// </summary>
        /// <param name="me">The step collection to evaulate</param>
        /// <returns>The step roots</returns>
        public static IEnumerable<BiDataFlowStep> GetExecutionTreeRoot(this BiFlowStepCollectionBase me) => me.Steps.Where(step => String.IsNullOrEmpty(step.Name) || !me.Steps.OfType<IDataFlowStreamStepDefinition>().Any(q => q.InputStep?.Name == step.Name) && !me.Steps.OfType<IDataFlowMultiStreamStepDefinition>().Any(m => m.InputSteps.Any(s => s?.Name == step.Name)));

        /// <summary>
        /// Get the full execution plan
        /// </summary>
        public static IEnumerable<BiDataFlowStep> GetExecutionPlan(this BiDataFlowStep me)
        {
            if (me is BiFlowStepCollectionBase bfs)
            {
                foreach (var itm in bfs.GetExecutionTreeRoot())
                {
                    yield return itm;
                }
            }
            else if (me is IDataFlowStreamStepDefinition strDef && strDef.InputStep != null)
            {
                foreach (var itm in strDef.InputStep.GetExecutionPlan())
                {
                    yield return itm;
                }

                if (me is IDataFlowMultiStreamStepDefinition mstrDef)
                {
                    foreach (var itm in mstrDef.InputSteps.SelectMany(m => m.GetExecutionPlan()))
                    {
                        yield return itm;
                    }

                }

                yield return me;
            }
            else
            {
                yield return me;
            }
        }

        /// <summary>
        /// Get the execution plan for the object
        /// </summary>
        public static String FormatExecutionPlan(this BiDataFlowStep me) => $"{me.GetType().GetSerializationName()}/{me.Name} execution plan: \r\n\t> {String.Join("\r\n\t> ", me.GetExecutionPlan().Select(e => e.ToString()))}";

        /// <summary>
        /// Set a system variable if not already set 
        /// </summary>
        internal static void SetSysVar<TValue>(this DataFlowScope scope, string name, TValue value)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            else if (!String.IsNullOrEmpty(name))
            {
                scope.DeclareConstant($"${name}", value);
            }
        }

        /// <summary>
        /// Lookup a system variable
        /// </summary>
        internal static bool TryGetSysVar<TValue>(this DataFlowScope scope, string name, out TValue result)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            else if (String.IsNullOrEmpty(name))
            {
                result = default(TValue);
                return false;
            }

            var sysVarName = $"${name}";
            if (scope.IsVisible(sysVarName))
            {
                result = scope.GetVariable<TValue>(sysVarName);
                return true;
            }
            else
            {
                result = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Attempt to get the resolved object
        /// </summary>
        internal static TValue ResolveReferenceTo<TValue>(this BiObjectReference bir, DataFlowScope scope)
        {
            if (bir.Resolved is TValue tv)
            {
                return tv;
            }
            else if (bir.Resolved is BiDataFlowParameterBindingRef bpf)
            {
                if (typeof(BiDefinition).IsAssignableFrom(typeof(TValue))) // caller wants the original
                {
                    return scope.GetVariable<TValue>(bpf.Name);
                }
                else if (scope.TryGetSysVar(bpf.Name, out tv))
                {
                    return tv;
                }
                else
                {
                    return default(TValue);
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(TValue), bir.Resolved.GetType()));
            }
        }
    }
}
