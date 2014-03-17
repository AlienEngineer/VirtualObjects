using System;
using System.Linq;
using System.Reflection;
using Fasterflect;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections;

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
            AddReference(typeof(Uri));
            AddReference(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException));
            AddReference(typeof(ExpandoObject));
            AddReference(typeof(ISession));
            AddReference(typeof(IDictionary<Object, Object>));
            AddReference(typeof(IQueryable));
            AddReference(type);

            AddNamespace("VirtualObjects");
            AddNamespace("System");
            AddNamespace("System.Linq");
            AddNamespace("System.Dynamic");
            AddNamespace("System.Collections.Generic");
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
        return new ExpandoObject();
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
        return new ExpandoObject();
    }}
".FormatWith(new
 {
     Name = TypeName
 });
        }
        protected override string GenerateOtherMethodsCode()
        {
            return @"
    private static System.Reflection.ConstructorInfo ctor;
    private static System.Reflection.ParameterInfo[] parameters;

    public static void Init(Type type) 
    {{
        ctor = type.GetConstructors().Single();
        parameters = ctor.GetParameters();
    }}

    public static Object EntityCast(Object source)
    {{
        IDictionary<string, object> dict = (ExpandoObject)source;

        var parameterValues = parameters.Select(p => dict[p.Name]).ToArray();

        return ctor.Invoke(parameterValues);
    }}

    public static Object Map(dynamic entity, Object[] data)
    {{
        {Body}
        
        return entity;
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
       UnderlyingType = _type.FullName.Replace('+', '.')
   });
        }

        private static String GenerateMapBody(Type type)
        {
            var result = new StringBuffer();

            var properties = type.GetProperties();
            for ( var i = 0; i < properties.Length; i++ )
            {
                var propertyInfo = properties[i];
                const string setter = @"
        entity.{FieldName} = {Value};";

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
                if ( propertyInfo.PropertyType.InheritsOrImplements<IEnumerable>() && propertyInfo.PropertyType != typeof(String) )
                {
                    //
                    // In case of a model type create a new instance of that model and set its values.
                    //
                    result += "new List<{Type}>()".FormatWith(new
                    {
                        Type = propertyInfo.PropertyType.GetGenericArguments().First().FullName.Replace('+', '.')
                    });
                }
                else
                {
                    result += "({Type})(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = propertyInfo.PropertyType.Name });
                }   
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
