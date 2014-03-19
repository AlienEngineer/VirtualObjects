using System;
using System.Linq;
using System.Reflection;
using Fasterflect;
using System.Dynamic;
using System.Collections.Generic;
using VirtualObjects.Config;
using System.Data;
using VirtualObjects.Queries;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        private readonly Type _type;
        private readonly IEntityBag entityBag;
        private readonly IQueryInfo queryInfo;
        
        public DynamicModelCodeGenerator(Type type, IEntityBag entityBag, IQueryInfo queryInfo)
            : base("Internal_Builder_Dynamic_" + type.Name.Replace("<>", "").Replace('`', '_'))
        {
            this.queryInfo = queryInfo;
            this.entityBag = entityBag;
            _type = type;

            AddReference(typeof(Object));
            AddReference(typeof(Uri));
            AddReference(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException));
            AddReference(typeof(ExpandoObject));
            AddReference(typeof(ISession));
            AddReference(typeof(IDictionary<Object, Object>));
            AddReference(typeof(IQueryable));
            AddReference(typeof(IDataReader));
            AddReference(type);

            AddNamespace("VirtualObjects");
            AddNamespace("VirtualObjects.Queries.Mapping");
            AddNamespace("System");
            AddNamespace("System.Linq");
            AddNamespace("System.Dynamic");
            AddNamespace("System.Collections.Generic");
            AddNamespace("System.Data");            
        }

        protected override string GenerateMapObjectCode()
        {
            return @"
    public static MapResult MapObject(Object entity, IDataReader reader)
    {
        return Map(entity, reader);
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

    public static MapResult Map(dynamic entity, IDataReader reader)
    {{
        int i = 0;
        var data = reader.GetValues();
        var hasMoreData = false;
        {Body}
        
        return new MapResult {{
            Entity = entity,
            HasMore = hasMoreData
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

    {SpecificMethods}

   
".FormatWith(new
   {
       Body = GenerateMapBody(_type),
       Name = TypeName,
       UnderlyingType = _type.FullName.Replace('+', '.'),
       SpecificMethods = GenerateSpecificMethods(_type, entityBag, queryInfo)
   });
        }

        private static string GenerateSpecificMethods(Type type, IEntityBag bag, IQueryInfo queryinfo)
        {
            var result = new StringBuffer();

            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsFrameworkType())
                {
                    if (property.PropertyType.IsCollection())
                    {


                        result += @"
    private static IList<{Type}> FillCollection_{PropertyName}(IDataReader reader, out int i, int index, out Boolean hasMoreData)
    {{
        
        var list = new List<{Type}>();
        Object id = null;
        hasMoreData = false;

        do {{
            i = index;
            var data = reader.GetValues();
            var entity = new {Type}();

            {Body}

            list.Add(entity);
        }} while({StopCondition});
        
        return list;
    }}".FormatWith(new
       {
           PropertyName = property.Name,
           Type = property.PropertyType.GetGenericArguments().First().FullName.Replace('+', '.'),
           Body = GenerateFillEntity(property.PropertyType.GetGenericArguments().First(), bag),
           StopCondition = GenerateStopCondition(property, bag, queryinfo) 
       });

                    }
                }
                else
                {
                    result += @"
    private static {Type} FillEntity_{PropertyName}(Object[] data, out int i, int index)
    {{
        i = index;
        var entity = new {Type}();

        {Body}

        return entity;
    }}".FormatWith(new
       {
           PropertyName = property.Name,
           Body = GenerateFillEntity(property.PropertyType, bag),
           Type = property.PropertyType.FullName.Replace('+', '.')
       });
                }
            }

            return result;
        }

        private static object GenerateStopCondition(PropertyInfo property, IEntityBag bag, IQueryInfo queryinfo)
        {
            var type = property.PropertyType.GetGenericArguments().First();

            var clause = queryinfo.OnClauses.FirstOrDefault(e => e.Column2.EntityInfo.EntityType == type);


            return "(id = reader[{FieldIndex1}]) != null && (hasMoreData = reader.Read()) && id.ToString() == reader[{FieldIndex2} + index].ToString()"
                .FormatWith(new { 
                    FieldIndex1 = clause.Column1.Index,
                    FieldIndex2 = clause.Column2.Index 
                });
        }

        private static string GenerateFillEntity(Type propertyType, IEntityBag bag)
        {
            var result = new StringBuffer();
            var entityInfo = bag[propertyType];

            for (var i = 0; i < entityInfo.Columns.Count; i++)
            {
                result += @"
        entity.{FieldName} {Value}; ++i;
".FormatWith(new
 {
     FieldName = entityInfo.Columns[i].Property.Name,
     Value = GenerateFieldAssignment(
                        i,
                        entityInfo.Columns[i].Property,
                        withFillMethodCall: false)
 });
            }

            return result;
        }

        private static String GenerateMapBody(Type type)
        {
            var result = new StringBuffer();

            var properties = type.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var propertyInfo = properties[i];
                const string setter = @"
        entity.{FieldName} = {Value};
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

        private static StringBuffer GenerateFieldAssignment(int i, PropertyInfo propertyInfo, bool withFillMethodCall = true)
        {
            StringBuffer result = " = ";
            if (propertyInfo.PropertyType.IsFrameworkType())
            {
                if (propertyInfo.PropertyType.IsCollection())
                {
                    //
                    // In case of a model type create a new instance of that model and set its values.
                    //
                    result += "FillCollection_{PropertyName}(reader, out i, i, out hasMoreData)".FormatWith(new
                    {
                        Type = propertyInfo.PropertyType.GetGenericArguments().First().FullName.Replace('+', '.'),
                        PropertyName = propertyInfo.Name
                    });
                }
                else
                {
                    result += "({Type})(Parse(data[i]) ?? default({Type}))".FormatWith(new { i, Type = propertyInfo.PropertyType.Name });
                }
            }
            else
            {

                if (!withFillMethodCall)
                {
                    result += "new {Type}()".FormatWith(new
                    {
                        Type = propertyInfo.PropertyType.FullName.Replace('+', '.'),
                        PropertyName = propertyInfo.Name
                    });
                }
                else
                {
                    //
                    // In case of a model type create a new instance of that model and set its values.
                    //
                    result += "FillEntity_{PropertyName}(data, out i, i)".FormatWith(new
                    {
                        Type = propertyInfo.PropertyType.FullName.Replace('+', '.'),
                        PropertyName = propertyInfo.Name
                    });
                }

            }

            return result;
        }
    }
}
