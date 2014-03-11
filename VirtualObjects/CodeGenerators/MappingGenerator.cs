using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CodeGenerators
{

    public class MappingGenerator : CodeCompiler, IMappingGenerator
    {

        private IEntityInfo entityInfo;

        /// <summary>
        /// Generates the mapper.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns></returns>
        public Action<object, Object[]> GenerateMapper(IEntityInfo entityInfo)
        {
            this.entityInfo = entityInfo;

            if ( !entityInfo.EntityType.IsPublic && !entityInfo.EntityType.IsNestedPublic )
            {
                throw new CodeCompilerException("The entity type {Name} is not public.", entityInfo.EntityType);
            }
            
            string[] References = new[] { entityInfo.EntityType.Module.Name };
            var results = Compile(References);

            Type binaryFunction = results.CompiledAssembly.GetType("Mapping" + entityInfo.EntityType.Name);
            var function = binaryFunction.GetMethod("MapObject");
            return (Action<Object, Object[]>)Delegate.CreateDelegate(typeof(Action<Object, Object[]>), function);
        }

        protected override string GenerateCode()
        {
            return GenerateCode(entityInfo);
        }

        private static String GenerateCode(IEntityInfo entityInfo)
        {

            var result = new StringBuffer();

            result += @"
using System;
using {Namespace};

public class Mapping{TypeShortName}
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
            result = result.Replace("{TypeName}", entityInfo.EntityType.FullName.Replace("+","."));
            result = result.Replace("{TypeShortName}", entityInfo.EntityType.Name);
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
            }

            return result;
        }

    }
}
