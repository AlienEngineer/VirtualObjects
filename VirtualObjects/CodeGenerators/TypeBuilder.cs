using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    class TypeBuilder : CodeCompiler
    {
        private CompilerResults results;
        private readonly string typeName;

        public ICollection<string> Body { get; private set; }
        public ICollection<string> References { get; private set; }
        public ICollection<string> Namespaces { get; private set; }

        public TypeBuilder(string typeName, Type baseType, SessionConfiguration configuration)
            : base(baseType, configuration)
        {
            this.typeName = typeName;
            Namespaces = new Collection<string>();
            Body = new Collection<string>();
            References = new Collection<string>();
        }

        protected override string AssemblyName { get { return typeName; } }

        public override bool IsDynamic { get; set; }

        protected override string GenerateCode()
        {
            var code = new StringBuffer();

            code += GenerateUsings();
            code += BeginClass(TypeName);

            code += GenerateFunctions();

            code += EndClass();

            return code;
        }

        private string GenerateFunctions()
        {
            var code = new StringBuffer();

            foreach ( string s in Body )
            {
                code = code + s;
            }

            return code;
        }

        private string BeginClass(string className)
        {
            var code = new StringBuffer();

            code += "public class ";
            code += className;
            code += "\n{\n";

            return code;
        }

        private static string EndClass()
        {
            return "}\n";
        }

        private string GenerateUsings()
        {
            var code = new StringBuffer();

            foreach ( var name in Namespaces )
            {
                code += "using ";
                code += name;
                code += ";\n";
            }

            code += "\n";

            return code;
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <typeparam name="TFunc">The type of the function.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public Delegate GetDelegate<TFunc>(string functionName)
        {
            results = results ?? Compile(References.ToArray());

            Type binaryFunction = results.CompiledAssembly.GetType(TypeName);
            var function = binaryFunction.GetMethod(functionName);
            return Delegate.CreateDelegate(typeof(TFunc), function);
        }

        public string TypeName { get { return typeName; } }
    }
}
