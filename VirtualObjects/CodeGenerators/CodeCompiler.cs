using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CodeCompiler
    {
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public String Code { get; private set; }

        /// <summary>
        /// Compiles the specified references.
        /// </summary>
        /// <param name="References">The references.</param>
        /// <returns></returns>
        protected CompilerResults Compile(string[] References)
        {
            using ( var provider = new CSharpCodeProvider() )
            {
                CompilerParameters cp  = new CompilerParameters();

                foreach ( var reference in References )
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;

                cp.CompilerOptions = "/optimize";
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;

                Code = GenerateCode();

                var cr = provider.CompileAssemblyFromSource(cp, Code);

                if ( cr.Errors.Count > 0 )
                {
                    Console.WriteLine(Code);

                    Console.WriteLine("Errors building of {0}", cr.PathToAssembly);

                    foreach ( CompilerError ce in cr.Errors )
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
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
