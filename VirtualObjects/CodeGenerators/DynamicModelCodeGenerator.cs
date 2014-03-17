using System;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        private readonly Type _type;

        public DynamicModelCodeGenerator(Type type)
            : base("Internal_Builder_Dynamic_" + type.Name.Replace("<>", "").Replace('`', '_'))
        {
            _type = type;

            AddReference(typeof(Object));
            AddReference(typeof(ISession));
            AddReference(typeof(IQueryable));

            AddNamespace("VirtualObjects");
            AddNamespace("System");
            AddNamespace("System.Linq");
        }

        protected override string GenerateMapObjectCode()
        {
            return @"
    public static Object MapObject(Object entity, Object[] data)
    {
        return Map(entity, data);
    }
";
        }

        protected override string GenerateMakeCode()
        {
            return @"
    public static Object Make()
    {{
        return new {Name}Model();
    }}
".FormatWith(new
 {
     Name = TypeName
 });
        }

        protected override string GenerateMakeProxyCode()
        {
            return @"
    public static Object MakeProxy(ISession session)
    {{
        return new {Name}Model();
    }}
".FormatWith(new
 {
     Name = TypeName
 });
        }
        protected override string GenerateOtherMethodsCode()
        {
            return @"
    public class {Name}Model
    {{
        {Members}
    }}

    public static Object Map(dynamic entity, Object[] data)
    {{
        return new {Name}Model {{
            {Body}
        }};
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
       Body = GenerateMapBody(_type),
       Name = TypeName,
       Members = GenerateMembers(_type)
   });
        }

        private static String GenerateMembers(Type type)
        {
            var result = new StringBuffer();

            var properties = type.GetProperties();
            for ( var i = 0; i < properties.Length; i++ )
            {
                var propertyInfo = properties[i];
                result += @"
        public {Type} {FieldName} {{ get; set; }}"
                    .FormatWith(new {
                        Type = propertyInfo.PropertyType.Name,
                        FieldName = propertyInfo.Name
                    });

            }

            return result;
        }

        private static String GenerateMapBody(Type type)
        {
            var result = new StringBuffer();

            var properties = type.GetProperties();
            for ( var i = 0; i < properties.Length; i++ )
            {
                var propertyInfo = properties[i];
                const string setter = @"
                    {FieldName} = {Value},
";

                String value = GenerateFieldAssignment(i, propertyInfo);
                value = value.Substring(3, value.Length - 3);

                result += setter.FormatWith(new
                {
                    FieldName = propertyInfo.Name,
                    i,
                    Value = value,
                    ValueNoType = value
                       .Replace(String.Format("({0})", propertyInfo.PropertyType.Name), "")
                       .Replace("default", String.Format("default({0})", propertyInfo.PropertyType.Name)),
                    Type = propertyInfo.PropertyType.Name
                });

            }

            // result.RemoveLast(",");
            return result;
        }

        private static StringBuffer GenerateFieldAssignment(int i, PropertyInfo propertyInfo)
        {
            StringBuffer result = " = ";
            if ( propertyInfo.PropertyType.IsFrameworkType() )
            {
                result += "({Type})(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = propertyInfo.PropertyType.Name });
            }
            else
            {
                //
                // In case of a model type create a new instance of that model and set its values.
                //
                result += "new {Type} {{ }}".FormatWith(new
                {
                    Type = propertyInfo.PropertyType.FullName.Replace('+', '.')
                });
            }

            return result;
        }
    }
}
