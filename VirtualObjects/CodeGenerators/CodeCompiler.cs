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
                //F:\Projectos\VirtualObjects\VirtualObjects.Tests\bin\Release
                cp.WarningLevel = 3;

                cp.CompilerOptions = "/optimize";
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;

                string code = GenerateCode();

                Console.WriteLine(code);

                return provider.CompileAssemblyFromSource(cp, code);
            }
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateCode();
    }
}
