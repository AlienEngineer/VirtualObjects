using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CodeGenerators
{



    /// <summary>
    /// 
    /// </summary>
    public abstract class CodeCompiler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompiler" /> class.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="configuration">The configuration.</param>
        protected CodeCompiler(Type baseType, SessionConfiguration configuration)
        {
            BaseType = baseType;
            Configuration = configuration;
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
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public SessionConfiguration Configuration { get; private set; }

        /// <summary>
        /// Compiles the specified references.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <returns></returns>
        protected virtual CompilerResults Compile(string[] references)
        {
            try
            {
                return InternalCompile(references);
            }
            catch (Exception e)
            {
                throw new MappingException(e.Message + "\n" + e.StackTrace, e);
            }
        }

        private CompilerResults InternalCompile(IEnumerable<string> references)
        {
            using (var provider = new CSharpCodeProvider())
            {
                var cp = new CompilerParameters();

                foreach (var reference in references.Distinct())
                {
                    cp.ReferencedAssemblies.Add(reference);
                }

                cp.WarningLevel = 3;
                cp.IncludeDebugInformation = false;
                cp.GenerateExecutable = false;
                cp.CompilerOptions = "/optimize";
                cp.GenerateInMemory = true;


                Code = GenerateCode();

                SaveCodeToFile();

                var cr = provider.CompileAssemblyFromSource(cp, Code);

                if (cr.Errors.Count > 0)
                {
                    var sb = new StringBuffer();

                    sb += "Unable to compile generated code.\n";
                    sb += "See " + BaseType.Name + ".cs for more information.\n\n";

                    // sb += Code;

                    sb = cr.Errors.Cast<CompilerError>().Aggregate(sb, (current, ce) => current + string.Format(@"  {0}\n", ce));

                    throw new MappingException(sb);
                }

                return cr;
            }
        }

        private void SaveCodeToFile()
        {
            if (Configuration.SaveGeneratedCode)
            {
                try
                {
                    var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        BaseType.Name.Replace("<>", "_").Replace('`', '_') + ".cs");

                    File.WriteAllText(fullpath, Code);
                }
                    // This is not an important output for the framework to run.
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                     /* Ignore exceptions. */

                }
            }
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateCode();
    }
}
