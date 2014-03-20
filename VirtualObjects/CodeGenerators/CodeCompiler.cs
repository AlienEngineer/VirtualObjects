using System.IO;
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
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public String Code { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract String AssemblyName { get; }

        /// <summary>
        /// Compiles the specified references.
        /// </summary>
        /// <param name="References">The references.</param>
        /// <returns></returns>
        protected CompilerResults Compile(string[] References)
        {
            using ( var provider = new CSharpCodeProvider() )
            {
                var cp  = new CompilerParameters();

                foreach ( var reference in References )
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;

                //cp.CompilerOptions = "/optimize";
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;
                cp.IncludeDebugInformation = true;
                //cp.CoreAssemblyFileName = AssemblyName;

                Code = GenerateCode();

                var cr = provider.CompileAssemblyFromSource(cp, Code);


                if (cr.Errors.Count > 0)
                {
                    Console.WriteLine(Code);

                    Console.WriteLine("Errors building of {0}", cr.PathToAssembly);

                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
#if DEBUG
                    // c:\Users\sérgio\AppData\Local\Temp\w1tyysbq.0.cs
                    File.WriteAllText(cr.TempFiles.BasePath + ".0.cs", Code);    
                
#endif
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
