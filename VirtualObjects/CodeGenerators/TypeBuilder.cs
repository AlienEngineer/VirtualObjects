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
        private readonly String typeName;

        public ICollection<String> Body { get; private set; }
        public ICollection<String> References { get; private set; }
        public ICollection<String> Namespaces { get; private set; }

        public TypeBuilder(String typeName, Type baseType, SessionConfiguration configuration)
            : base(baseType, configuration)
        {
            this.typeName = typeName;
            Namespaces = new Collection<String>();
            Body = new Collection<String>();
            References = new Collection<String>();
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

        private String GenerateFunctions()
        {
            var code = new StringBuffer();

            foreach ( string s in Body )
            {
                code = code + s;
            }

            return code;
        }

        private String BeginClass(string className)
        {
            var code = new StringBuffer();

            code += "public class ";
            code += className;
            code += "\n{\n";

            return code;
        }

        private static String EndClass()
        {
            return "}\n";
        }

        private String GenerateUsings()
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
        public Delegate GetDelegate<TFunc>(String functionName)
        {
            results = results ?? Compile(References.ToArray());

            Type binaryFunction = results.CompiledAssembly.GetType(TypeName);
            var function = binaryFunction.GetMethod(functionName);
            return Delegate.CreateDelegate(typeof(TFunc), function);
        }

        public String TypeName { get { return typeName; } }
    }
}
