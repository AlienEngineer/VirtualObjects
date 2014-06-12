using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;
using System.Dynamic;
using System.Collections.Generic;
using VirtualObjects.Config;
using System.Data;
using VirtualObjects.Queries;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        private readonly Type _type;
        private readonly IEntityBag entityBag;
        private readonly IQueryInfo queryInfo;
        private int projectionIndex = 0;
        
        public DynamicModelCodeGenerator(Type type, IEntityBag entityBag, IQueryInfo queryInfo, SessionConfiguration configuration)
            : base(type.Namespace + "_Internal_Builder_Dynamic_" + MakeDynamicSafeName(type), type, configuration, IsDynamic: true)
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
            AddNamespace("System.Reflection");
        }

        private static String MakeDynamicSafeName(Type type)
        {
            var result = new StringBuilder();
            
            for (int index = 0; index < type.Properties().Count; index++)
            {
                var property = type.Properties()[index];
                result = result.Append(index).Append(property.PropertyType.FullName).Append(property.Name);
            }

            return result
                .GetHashCode()
                .ToString(CultureInfo.InvariantCulture).Replace('-','0');
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
            projectionIndex = 0;
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
       Body = GenerateMapBody(_type, queryInfo),
       Name = TypeName,
       UnderlyingType = _type.FullName.Replace('+', '.'),
       SpecificMethods = GenerateSpecificMethods(_type, entityBag, queryInfo)
   });
        }

        private string GenerateSpecificMethods(Type type, IEntityBag bag, IQueryInfo queryinfo)
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
           Body = GenerateFillEntity(property.PropertyType.GetGenericArguments().First(), bag, queryinfo),
           StopCondition = GenerateStopCondition(property, queryinfo) 
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
           Body = GenerateFillEntity(property.PropertyType, bag, queryinfo),
           Type = property.PropertyType.FullName.Replace('+', '.')
       });
                }
            }

            return result;
        }

        private static object GenerateStopCondition(PropertyInfo property, IQueryInfo queryinfo)
        {
            var type = property.PropertyType.GetGenericArguments().First();

            var clause = queryinfo.OnClauses.FirstOrDefault(e => e.Column2.EntityInfo.EntityType == type);


            return "(id = reader[{FieldIndex1}]) != null && (hasMoreData = reader.Read()) && id.ToString() == reader[{FieldIndex2} + index].ToString()"
                .FormatWith(new { 
                    FieldIndex1 = clause.Column1.Index,
                    FieldIndex2 = clause.Column2.Index 
                });
        }

        private string GenerateFillEntity(Type propertyType, IEntityBag bag, IQueryInfo queryInfo)
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
                        entityInfo.Columns[i].Property,
                        queryInfo, 
                        withMethodCall: false, 
                        column: entityInfo.Columns[i])
 });
                ++projectionIndex;
            }

            result.Replace(" --i; ++i;", "");
            return result;
        }

        private String GenerateMapBody(Type type, IQueryInfo queryInfo)
        {
            var result = new StringBuffer();

            var properties = type.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                
                var propertyInfo = properties[i];
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
        ++i;
";

                String value = GenerateFieldAssignment(
                    propertyInfo, 
                    queryInfo, 
                    withMethodCall: true, 
                    column: projectionIndex < queryInfo.PredicatedColumns.Count ? queryInfo.PredicatedColumns[projectionIndex] : null);
                value = value.Substring(3, value.Length - 3);

                result += setter.FormatWith(new
                {
                    i,
                    FieldName = propertyInfo.Name,
                    Value = value,
                    Comment = propertyInfo.PropertyType.IsFrameworkType() || propertyInfo.PropertyType.IsGenericCollection() ? "//" : String.Empty,
                    NotComment = propertyInfo.PropertyType.IsFrameworkType() && !propertyInfo.PropertyType.IsGenericCollection() ? String.Empty : "//",
                    ValueNoType = value
                       .Replace(String.Format("({0})", propertyInfo.PropertyType.Name), "")
                       .Replace("default", String.Format("default({0})", propertyInfo.PropertyType.Name)),
                    Type = propertyInfo.PropertyType.Name
                });


                if ( propertyInfo.PropertyType.IsFrameworkType() )
                {
                    if ( propertyInfo.PropertyType.IsGenericCollection() )
                    {
                        projectionIndex += entityBag[propertyInfo.PropertyType.GetGenericArguments().First()].Columns.Count;
                    }
                    else
                    {
                        ++projectionIndex;
                    }
                }
                else
                {
                    projectionIndex += entityBag[propertyInfo.PropertyType].Columns.Count;
                }
            }

            result.Replace(" --i; ++i;", "");
            return result;
        }

        private StringBuffer GenerateFieldAssignment(PropertyInfo propertyInfo, IQueryInfo queryInfo, Boolean withMethodCall = false, IEntityColumnInfo column = null)
        {
            StringBuffer result = " = ";
            if (propertyInfo.PropertyType.IsFrameworkType())
            {
                if ( propertyInfo.PropertyType.IsGenericCollection() )
                {
                    //
                    // In case of a model type create a new instance of that model and set its values.
                    //
                    result += "FillCollection_{PropertyName}(reader, out i, i, out hasMoreData); --i".FormatWith(new
                    {
                        Type = propertyInfo.PropertyType.GetGenericArguments().First().FullName.Replace('+', '.'),
                        PropertyName = propertyInfo.Name
                    });
                }
                else
                {
                    result += "({Type})(Parse(data[i]) ?? default({Type}))".FormatWith(new { Type = propertyInfo.PropertyType.Name });
                }
            }
            else
            {

                if ( !withMethodCall && column != null && column.ForeignKey != null)
                {
                    result += "new {Type} {{ {BoundField} {Value} }}".FormatWith(new
                    {
                        Type = column.Property.PropertyType.FullName.Replace('+', '.'),
                        BoundField = column.ForeignKey.Property.Name,
                        Value = GenerateFieldAssignment(column.ForeignKey.Property, queryInfo, withMethodCall, column.ForeignKey)
                    });
                }
                else
                {
                    var entityInfo = entityBag[propertyInfo.PropertyType];

                    if ( !queryInfo.Sources.Contains(entityInfo) && projectionIndex + entityInfo.Columns.Count > queryInfo.PredicatedColumns.Count )
                    {
                        throw new TranslationException("\nUnable to use the full entity of type [{Name}] on projection without a proper join statement.", propertyInfo.PropertyType);
                    }

                    //
                    // In case of a model type create a new instance of that model and set its values.
                    //
                    result += "FillEntity_{PropertyName}(data, out i, i); --i".FormatWith(new
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
