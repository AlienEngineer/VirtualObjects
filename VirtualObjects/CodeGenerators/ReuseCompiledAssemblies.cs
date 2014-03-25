using System;
using System.CodeDom.Compiler;
using System.IO;

namespace VirtualObjects.CodeGenerators
{
    abstract class ReuseCompiledAssemblies : CodeCompiler
    {
        protected override CompilerResults Compile(string[] References)
        {
            if (!IsDynamic && File.Exists(Path.GetTempPath() + "VirtualObjects\\" + AssemblyName + ".dll"))
            {
                return new CompilerResults(new TempFileCollection())
                {
                    CompiledAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.GetTempPath() + "VirtualObjects\\" + AssemblyName + ".dll"))
                };
            }

            return base.Compile(References);
        }
    }
}