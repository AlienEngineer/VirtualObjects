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
        /// <param name="References">The references.</param>
        /// <returns></returns>
        protected virtual CompilerResults Compile(string[] References)
        {
            using (var provider = new CSharpCodeProvider())
            {
                var cp = new CompilerParameters();

                foreach (var reference in References)
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;

#if !DEBUG
                cp.CompilerOptions = "/optimize";    
#endif
                cp.IncludeDebugInformation = true;
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = false;

                if (!IsDynamic)
                {

                    try
                    {
                        string fileName = BaseType.Assembly.CodeBase.Replace("file:///", "");

                        var path = Directory.GetParent(fileName).FullName;

                        cp.OutputAssembly = GetAssemblyPath(path);
                        cp.GenerateInMemory = !IsDynamic;

                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        //
                        // If the path doesn't have access to write use %TMP%\VirtualObjects\
                        // 
                        cp.OutputAssembly = GetAssemblyPath(Path.GetTempPath());
                        cp.GenerateInMemory = true;
                    }

                }


                Code = GenerateCode();

                var cr = provider.CompileAssemblyFromSource(cp, Code);

                if (cr.Errors.Count > 0)
                {
                    Console.WriteLine(Code);

                    Console.WriteLine(@"Errors building of {0}", cr.PathToAssembly);

                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine(@"  {0}", ce);
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

        private string GetAssemblyPath(string path)
        {
            
            var assemblyPath = Path.Combine(path, "VirtualObjects");

            if (!Directory.Exists(assemblyPath))
            {
                Directory.CreateDirectory(assemblyPath);
            }
            return Path.Combine(assemblyPath, AssemblyName + ".dll");
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateCode();
    }
}
