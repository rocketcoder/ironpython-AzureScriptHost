using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Xml.Linq;
using IronPython.Runtime;
using System.Reflection;


namespace AzureScriptHost
{
    /// <summary>
    /// Interfaces with the IronPython engine to create Function objects
    /// </summary>
    public class Rules
    {
        #region Member Variables
        private ScriptEngine m_Engine = null;
        #endregion Static Member Variables

        #region Constructors
        public Rules(string storageConnectionString, string container)
            : this(storageConnectionString, container, GetBaseTypes())
        {
        }

        public Rules(string storageConnectionString, string container,  List<Assembly> assemblies)
        {

            if (!IsEngineInitialized())
            {
                List<object> hostStartUpParameters = new List<object>();
                hostStartUpParameters.Add(storageConnectionString);
                hostStartUpParameters.Add(container);
                
                var setup = Python.CreateRuntimeSetup(null);
                setup.HostType = typeof(AzureCloudScriptHost);
                setup.HostArguments = hostStartUpParameters;
                ScriptRuntime runtime = new ScriptRuntime(setup);
                m_Engine = runtime.GetEngineByTypeName(typeof(PythonContext).AssemblyQualifiedName);
                assemblies.ForEach(x => m_Engine.Runtime.LoadAssembly(x));
            }
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// creates a dynamic object that is the Implementation of a RuleSet
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public dynamic RequestNewRuleSet(string className)
        {
            #region validation
            if (!IsEngineInitialized())
            {
                throw new Exception("Engine has not been initialized.  Call InitializeScriptingEngine first.");
            }
            #endregion validation
            string createNewRuleSetCode = String.Format("import {0} \n\r{0}.{0}()", className);
            var source = m_Engine.CreateScriptSourceFromString(createNewRuleSetCode);
            return source.Execute();
        }

        public dynamic RequestNewClass(string className, params object[] args)
        {
            #region validation
            if (!IsEngineInitialized())
            {
                throw new Exception("Engine has not been initialized.  Call InitializeScriptingEngine first.");
            }
            #endregion validation
            ScriptScope scope = m_Engine.CreateScope();
            List<string> pyArgs = new List<string>();
            for (int count = 0; count < args.Count(); count++)
            {
                string name = string.Format("arg{0}", count);
                pyArgs.Add(name);
                scope.SetVariable(name, args[count]);
            }
            string createNewRuleSetCode = String.Format("import {0} \n\r{0}.{0}({1})", className, string.Join(", ", pyArgs));
            var source = m_Engine.CreateScriptSourceFromString(createNewRuleSetCode);
            return source.Execute(scope);
        }

        public bool IsEngineInitialized()
        {
            return !(m_Engine == null);
        }

        private static List<Assembly> GetBaseTypes()
        {
            List<Assembly> types = new List<Assembly>();
            types.Add(typeof(string).Assembly);
            types.Add(typeof(XElement).Assembly);
            types.Add(typeof(List).Assembly);

            return types;
        }

        #endregion Methods


    }
}