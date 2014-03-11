using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualObjects;
using Fasterflect;

namespace VirtualObjects.CodeGenerators
{
    public class MappingGenerator : IMappingGenerator
    {

        /// <summary>
        /// Generates the mapper.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns></returns>
        public Action<object, Object[]> GenerateMapper(IEntityInfo entityInfo)
        {

            var code = GenerateCode(entityInfo);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp  = new CompilerParameters();

            string[] References = new[] { entityInfo.EntityType.Module.Name.Replace(".dll", "") };
            foreach ( var reference in References )
            {
                cp.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory
                           + string.Format("\\{0}.dll", reference));
            }

            cp.WarningLevel = 3;

            cp.CompilerOptions = "/optimize";
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            CompilerResults results = provider.CompileAssemblyFromSource(cp, code);

            Type binaryFunction = results.CompiledAssembly.GetType("Mapping");
            var function = binaryFunction.GetMethod("MapObject");
            return (Action<Object, Object[]>)Delegate.CreateDelegate(typeof(Action<Object, Object[]>), function);

        }

        private static String GenerateCode(IEntityInfo entityInfo)
        {

            var result = new StringBuffer();

            result += @"
using System;
using {Namespace};

public class Mapping
{       
    public static void MapObject(Object entity, Object[] data)
    {
        Map(({TypeName})entity, data);
    }
    
    public static void Map({TypeName} entity, Object[] data)
    {
        {Body}
    }
}
";

            result = result.Replace("{Namespace}", entityInfo.EntityType.Namespace);
            result = result.Replace("{TypeName}", entityInfo.EntityType.Name);
            result = result.Replace("{Body}", GenerateBody(entityInfo));

            return result;
        }

        private static string GenerateBody(IEntityInfo entityInfo)
        {
            var result = new StringBuffer();

            for ( int i = 0; i < entityInfo.Columns.Count; i++ )
            {
                var column = entityInfo.Columns[i];

                if ( column.Property.PropertyType.IsFrameworkType() )
                {
                    result += "entity.";
                    result += column.Property.Name;
                    result += " = ";
                    result += "(";
                    result += column.Property.PropertyType.Name;
                    result += ")";
                    result += "data[";
                    result += i;
                    result += "];";
                }
                else
                {

                }
            }

            return result;
        }
    }
}
