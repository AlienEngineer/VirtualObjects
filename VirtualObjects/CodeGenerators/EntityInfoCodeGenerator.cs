using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Exceptions;
using Fasterflect;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGenerator
    {

        readonly TypeBuilder builder;
        private readonly IEntityInfo entityInfo;
        
        public EntityInfoCodeGenerator(IEntityInfo info)
        {
            this.entityInfo = info;
            builder = new TypeBuilder("Internal_Builder_" + info.EntityName);
        }

        public void GenerateCode()
        {
            if ( !entityInfo.EntityType.IsPublic && !entityInfo.EntityType.IsNestedPublic )
            {
                throw new CodeCompilerException("The entity type {Name} is not public.", entityInfo.EntityType);
            }

            builder.References.Add(entityInfo.EntityType.Module.Name);
            builder.Namespaces.Add(entityInfo.EntityType.Namespace);

            var properName = entityInfo.EntityType.FullName.Replace("+", ".");

            builder.Functions.Add(@"
    public static void MapObject(Object entity, Object[] data)
    {{
        Map(({TypeName})entity, data);
    }}
".FormatWith(new { TypeName = properName }));

            builder.Functions.Add(@"
    public static Object Make()
    {{
        return new {TypeName}();
    }}
".FormatWith(new { TypeName = properName }));

            builder.Functions.Add(@"
    public static void Map({TypeName} entity, Object[] data)
    {{
        {Body}
    }}

    private static Object Parse(Object value)
    {{
        if ( value == null || value == DBNull.Value )
        {{
            return null;
        }}

        return value;
    }}
".FormatWith(new
 {
     TypeName = properName,
     Body = GenerateBody(entityInfo)
 }));

        }

        private static string GenerateBody(IEntityInfo entityInfo)
        {
            var result = new StringBuffer();

            for ( int i = 0; i < entityInfo.Columns.Count; i++ )
            {
                var column = entityInfo.Columns[i];

                if ( column.Property.PropertyType.IsFrameworkType() )
                {
                    result += "\n           entity.";
                    result += column.Property.Name;
                    result += " = ";
                    result += "(";
                    result += column.Property.PropertyType.Name;
                    result += ")";
                    result += "Parse(data[";
                    result += i;
                    result += "]);";
                }
            }

            return result;
        }

        public Action<Object, Object[]> GetEntityMapper()
        {
            return (Action<Object, Object[]>)builder.GetDelegate<Action<Object, Object[]>>("MapObject");
        }

        public Func<Object> GetEntityProvider()
        {
            return (Func<Object>)builder.GetDelegate<Func<Object>>("Make");
        }

    }
}
