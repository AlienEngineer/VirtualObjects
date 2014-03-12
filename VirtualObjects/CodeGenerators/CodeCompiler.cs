using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    public abstract class CodeCompiler
    {
        protected CompilerResults Compile(string[] References)
        {
            using ( var provider = new CSharpCodeProvider() )
            {
                CompilerParameters cp  = new CompilerParameters();

                foreach ( var reference in References )
                {
                    cp.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + reference);
                }

                cp.WarningLevel = 3;

                cp.CompilerOptions = "/optimize";
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;

                string code = GenerateCode();

                var cr = provider.CompileAssemblyFromSource(cp, code);

                if ( cr.Errors.Count > 0 )
                {
                    Console.WriteLine(code);

                    Console.WriteLine("Errors building of {0}", cr.PathToAssembly);

                    foreach ( CompilerError ce in cr.Errors )
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine(code); 
#endif
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
