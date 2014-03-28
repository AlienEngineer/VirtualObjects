using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;

namespace VirtualObjects.CodeGenerators
{
    abstract class ReuseCompiledAssemblies : CodeCompiler
    {
        protected ReuseCompiledAssemblies(Type baseType)
            : base(baseType)
        {

        }

        protected override CompilerResults Compile(string[] References)
        {
            string fileName = BaseType.Assembly.CodeBase.Replace("file:///", "");
            var assemblyPath = Directory.GetParent(fileName).FullName + "\\VirtualObjects\\" + AssemblyName + ".dll";

            if ( !IsDynamic && File.Exists(assemblyPath) )
            {

                var assemblyVersion = new Version(FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion);
                var currentVersion = new Version(FileVersionInfo.GetVersionInfo(fileName).FileVersion);

                if (assemblyVersion == currentVersion)
                {
                    return new CompilerResults(new TempFileCollection())
                    {
                        CompiledAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(assemblyPath))
                    };    
                }     
            }

            return base.Compile(References);
        }
    }
}