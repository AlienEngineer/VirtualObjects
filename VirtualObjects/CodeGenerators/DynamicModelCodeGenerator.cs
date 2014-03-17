using System;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        private readonly Type _type;

        public DynamicModelCodeGenerator(Type type)
            : base("Internal_Builder_Dynamic_" + type.Name)
        {
            _type = type;
        }

        protected override string GenerateMapObjectCode()
        {
            return @"
    public static void MapObject(Object entity, Object[] data)
    {{
        Map(entity, data);
    }}
";
        }

        protected override string GenerateMakeCode()
        {
            return @"
    public static Object Make()
    {{
        return new {{ }};
    }}
";
        }

        protected override string GenerateMakeProxyCode()
        {
            return @"
    public static {TypeName} MakeProxy(ISession session)
    {{
        return new {{ }};
    }}
";
        }
        protected override string GenerateOtherMethodsCode()
        {
          return @"
    public static void Map(dynamic entity, Object[] data)
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
     Body = GenerateMapBody(_type)
 });
        }

        private static String GenerateMapBody(Type type)
        {
            var result = new StringBuffer();

            var fields = type.GetFields();
            for (var i = 0; i < fields.Length; i++)
            {
                var fieldInfo = fields[i];
                const string setter = @"
                try
                {{
                    if (data[{i}] != DBNull.Value)
                        entity.{FieldName} = {Value};
                }}
                catch (InvalidCastException) 
                {{ 
                     entity.{FieldName} = ({Type})Convert.ChangeType({ValueNoType}, typeof({Type}));
                }}
                catch ( Exception ex)
                {{
                    throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value."", ex);
                }}
";

                String value = GenerateFieldAssignment(i, fieldInfo);
                value = value.Substring(3, value.Length - 3);

                result += setter.FormatWith(new
                {
                    FieldName = fieldInfo.Name,
                    i,
                    Value = value,
                    ValueNoType = value
                       .Replace(String.Format("({0})", fieldInfo.FieldType.Name), "")
                       .Replace("default", String.Format("default({0})", fieldInfo.FieldType.Name)),
                    Type = fieldInfo.FieldType.Name
                });
    
            }

            return result;
        }

        private static StringBuffer GenerateFieldAssignment(int i, FieldInfo fieldInfo)
        {
            StringBuffer result = " = ";
            if (fieldInfo.FieldType.IsFrameworkType())
            {
                result += "({Type})(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = fieldInfo.FieldType.Name });
            }
            else
            {
                //
                // In case of a model type create a new instance of that model and set its values.
                //
                result += "new {Type} {{ }}".FormatWith(new
                {
                    Type = fieldInfo.FieldType.FullName.Replace('+', '.')
                });
            }

            return result;
        }
    }
}
