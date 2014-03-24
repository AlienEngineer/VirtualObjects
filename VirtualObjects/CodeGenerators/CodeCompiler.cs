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
        private string assemblyPath;

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
        /// Compiles the specified references.
        /// </summary>
        /// <param name="References">The references.</param>
        /// <returns></returns>
        protected virtual CompilerResults Compile(string[] References)
        {
            using ( var provider = new CSharpCodeProvider() )
            {
                var cp  = new CompilerParameters();
                                
                foreach ( var reference in References )
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;

#if !DEBUG
                cp.CompilerOptions = "/optimize";    
#endif
                cp.IncludeDebugInformation = true;
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = !IsDynamic;

                if (!IsDynamic)
                {
                    assemblyPath = System.IO.Path.GetTempPath() + "VirtualObjects\\";
                    cp.OutputAssembly = assemblyPath + AssemblyName + ".dll";

                    if ( !Directory.Exists(assemblyPath) )
                    {
                        Directory.CreateDirectory(assemblyPath);
                    }    
                }
                

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

#if !DEBUG
                    File.WriteAllText(cp.OutputAssembly + ".cs", Code);
#endif
                }
#if DEBUG
                    File.WriteAllText(cp.OutputAssembly + ".cs", Code);   
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
