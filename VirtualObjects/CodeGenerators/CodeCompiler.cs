using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;

namespace VirtualObjects.CodeGenerators
{



    /// <summary>
    /// 
    /// </summary>
    public abstract class CodeCompiler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompiler"/> class.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        protected CodeCompiler(Type baseType)
        {
            BaseType = baseType;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public String Code { get; private set; }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        protected abstract String AssemblyName { get; }

        /// <summary>
        /// Gets a value indicating whether [is dynamic].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is dynamic]; otherwise, <c>false</c>.
        /// </value>
        public abstract Boolean IsDynamic { get; set; }

        /// <summary>
        /// The base type
        /// </summary>
        public Type BaseType { get; private set; }

        /// <summary>
        /// Compiles the specified references.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <returns></returns>
        protected virtual CompilerResults Compile(string[] references)
        {
            try
            {
                return InternalCompile(references);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace, e);
            }
        }

        private CompilerResults InternalCompile(IEnumerable<string> references)
        {
            using (var provider = new CSharpCodeProvider())
            {
                var cp = new CompilerParameters();

                foreach (var reference in references)
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;
                cp.IncludeDebugInformation = false;
                cp.GenerateExecutable = false;
                cp.CompilerOptions = "/optimize";    
                cp.GenerateInMemory = true;


                Code = GenerateCode();

                var cr = provider.CompileAssemblyFromSource(cp, Code);

                if (cr.Errors.Count > 0)
                {
                    Trace.WriteLine(Code);

                    Trace.WriteLine(string.Format(@"Errors building of {0}", cr.PathToAssembly));

                    foreach (CompilerError ce in cr.Errors)
                    {
                        Trace.WriteLine(string.Format(@"  {0}", ce));
                        Trace.WriteLine("");
                    }

                }

                return cr;
            }
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateCode();
    }
}
