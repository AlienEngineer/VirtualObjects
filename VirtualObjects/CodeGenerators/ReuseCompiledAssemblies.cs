using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CodeGenerators
{
    abstract class ReuseCompiledAssemblies : CodeCompiler
    {
        protected ReuseCompiledAssemblies(Type baseType)
            : base(baseType)
        {

        }

        protected override CompilerResults Compile(string[] references)
        {
            try
            {

                string fileName = BaseType.Assembly.CodeBase.Replace("file:///", "");
                var assemblyPath = Path.Combine(Directory.GetParent(fileName).FullName, "VirtualObjects\\" + AssemblyName + ".dll");

                if (!IsDynamic && File.Exists(assemblyPath))
                {
                    CompilerResults compile;
                    if (LoadAssembly(assemblyPath, fileName, out compile)) return compile;

                    assemblyPath = Path.Combine(Path.GetTempPath(),"VirtualObjects\\" + AssemblyName + ".dll");
                    if (LoadAssembly(assemblyPath, fileName, out compile)) return compile;
                }
            }
            catch (Exception)
            {
                throw new MappingException("\nUnable to get the generated assembly for type {Name}.", BaseType);
            }

            return base.Compile(references);
        }

        private static bool LoadAssembly(string assemblyPath, string fileName, out CompilerResults compile)
        {
            compile = null;
            var assemblyVersion = new Version(FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion);
            var currentVersion = new Version(FileVersionInfo.GetVersionInfo(fileName).FileVersion);

            if (assemblyVersion == currentVersion)
            {
                {
                    compile = new CompilerResults(new TempFileCollection())
                              {
                                  CompiledAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(assemblyPath))
                              };
                    return true;
                }
            }
            return false;
        }
    }
}