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
            builder = new TypeBuilder("Internal_Builder_" + info.EntityType.Name);
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

#if DEBUG
                const string setter = @"
                try
                {{
                    {Comment}if (data[{i}] != DBNull.Value)
                        entity.{FieldName} {Value}
                }}
                catch ( Exception ex)
                {{
                    throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value."", ex);
                }}
";
#else
                string setter = @"
                {Comment}if (data[{i}] != DBNull.Value)
                    entity.{FieldName} {Value}
";
#endif
                result += setter.FormatWith(new
                 {
                     FieldName = column.Property.Name,
                     i,
                     Value = GenerateFieldAssignment(i, column),
                     Comment = column.ForeignKey == null ? "//" : String.Empty
                 });

            }

            return result;
        }

        private static StringBuffer GenerateFieldAssignment(int i, IEntityColumnInfo column, bool finalize = true)
        {
            StringBuffer result = " = ";
            if ( column.Property.PropertyType.IsFrameworkType() )
            {
                result += "(";
                result += column.Property.PropertyType.Name;
                result += ")";

                if ( column.Property.PropertyType == typeof(DateTime) )
                {
                    
                    result += "(Parse(data[";
                    result += i;
                    result += "]) ?? default(";
                    result += column.Property.PropertyType.Name;
                    result += "))";
                }
                else
                {
                    result += "Parse(data[";
                    result += i;
                    result += "])";
                }
            }
            else
            {
                result += "new {Type} {{ {BoundField} {Value} }}".FormatWith(new
                {
                    Type = column.Property.PropertyType.FullName.Replace('+', '.'),
                    BoundField = column.ForeignKey.Property.Name,
                    Value = GenerateFieldAssignment(i, column.ForeignKey, false)
                });
            }

            if ( finalize )
            {
                result += ";";
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
