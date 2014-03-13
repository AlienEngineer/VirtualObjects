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
            builder.References.Add("VirtualObjects.dll");
            builder.Namespaces.Add(entityInfo.EntityType.Namespace);
            builder.Namespaces.Add("VirtualObjects");

            var properName = entityInfo.EntityType.FullName.Replace("+", ".");

            builder.Body.Add(@"
    public class {Name} : {TypeName}
    {{
        private ISession Session {{ get; set; }}

        public {Name}(ISession session) 
        {{
            Session = session;
        }}

        {OverridableMembers}
    }}

    public static {TypeName} MakeProxy(ISession session)
    {{
        return new {Name}(session);
    }}
".FormatWith(new { 
     TypeName = properName,
     Name = entityInfo.EntityType.Name + "Proxy",
     OverridableMembers = ""
 }));

            builder.Body.Add(@"
    public static void MapObject(Object entity, Object[] data)
    {{
        Map(({TypeName})entity, data);
    }}
".FormatWith(new { TypeName = properName }));

            builder.Body.Add(@"
    public static Object Make()
    {{
        return new {TypeName}();
    }}
".FormatWith(new { TypeName = properName }));

            builder.Body.Add(@"
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

                const string setter = @"
                try
                {{
                    {Comment}if (data[{i}] != DBNull.Value)
                        entity.{FieldName} = {Value};
                }}
                catch (InvalidCastException) 
                {{ 
                     {NotComment}entity.{FieldName} = ({Type})Convert.ChangeType({ValueNoType}, typeof({Type}));
                }}
                catch ( Exception ex)
                {{
                    throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value."", ex);
                }}
";

                String value = GenerateFieldAssignment(i, column);
                value = value.Substring(3, value.Length - 3);

                result += setter.FormatWith(new
                 {
                     FieldName = column.Property.Name,
                     i,
                     Value = value,
                     ValueNoType = value
                        .Replace(String.Format("({0})", column.Property.PropertyType.Name), "")
                        .Replace("default", String.Format("default({0})", column.Property.PropertyType.Name)),
                     Comment = column.ForeignKey == null ? "//" : String.Empty,
                     NotComment = column.ForeignKey == null ? String.Empty : "//",
                     Type = column.Property.PropertyType.Name
                 });

            }

            return result;
        }

        private static StringBuffer GenerateFieldAssignment(int i, IEntityColumnInfo column, bool finalize = true)
        {
            StringBuffer result = " = ";
            if ( column.Property.PropertyType.IsFrameworkType() )
            {
                result += "({Type})(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = column.Property.PropertyType.Name });
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

        public Func<ISession, Object> GetEntityProxyProvider()
        {
            return (Func<ISession, Object>)builder.GetDelegate<Func<ISession, Object>>("MakeProxy");
        }
    }
}
