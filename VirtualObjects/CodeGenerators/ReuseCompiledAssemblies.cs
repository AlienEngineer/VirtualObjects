using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;

namespace VirtualObjects.CodeGenerators
{
    abstract class ReuseCompiledAssemblies : CodeCompiler
    {
        protected ReuseCompiledAssemblies(Type baseType) : base(baseType)
        {

        }

        protected override CompilerResults Compile(string[] References)
        {
            var assemblyPath = Path.GetTempPath() + "VirtualObjects\\" + AssemblyName + ".dll";

            var assemblyVersion = new Version(FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion);
            
            var currentVersion = new Version(FileVersionInfo.GetVersionInfo(BaseType.Assembly.Location).FileVersion);

            if (!IsDynamic && File.Exists(assemblyPath) && assemblyVersion == currentVersion)
            {
                return new CompilerResults(new TempFileCollection())
                {
                    CompiledAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(assemblyPath))
                };
            }

            return base.Compile(References);
        }
    }
}