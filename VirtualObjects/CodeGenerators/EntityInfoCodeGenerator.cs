﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Fasterflect;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGenerator : EntityCodeGenerator
    {
        private readonly IEntityInfo _entityInfo;
        private readonly ITranslationConfiguration _configuration;
        private readonly IEntityBag _entityBag;
        private readonly string _properName;

        public EntityInfoCodeGenerator(IEntityInfo info, IEntityBag entityBag, ITranslationConfiguration configuration, SessionConfiguration sessionConfiguration)
            : base(info.EntityType.Namespace.Replace(".", "_") + "_Internal_Builder_" + info.EntityType.Name, info.EntityType, sessionConfiguration)
        {
            _configuration = configuration;
            _entityBag = entityBag;
            _entityInfo = info;
            _properName = _entityInfo.EntityType.FullName.Replace("+", ".");


            AddReference(_entityInfo.EntityType);
            AddReference(typeof(object));
            AddReference(typeof(ISession));
            AddReference(typeof(IQueryable));
            AddReference(typeof(IDataReader));

            AddNamespace(_entityInfo.EntityType.Namespace);
            AddNamespace("VirtualObjects");
            AddNamespace("VirtualObjects.Queries.Mapping");
            AddNamespace("System");
            AddNamespace("System.Linq");
            AddNamespace("System.Data");
            AddNamespace("System.Reflection");
            AddNamespace("System.Globalization");
        }

        protected override string GenerateMapObjectCode()
        {
            return @"
    public static MapResult MapObject(Object entity, IDataReader reader)
    {{
        return Map(({TypeName})entity, reader);
    }}
".FormatWith(new { TypeName = _properName });
        }

        protected override string GenerateMakeCode()
        {
            return @"
    public static Object Make()
    {{
        return new {TypeName}();
    }}
".FormatWith(new { TypeName = _properName });
        }

        protected override string GenerateMakeProxyCode()
        {
            return @"
    public static {TypeName} MakeProxy(ISession session)
    {{
        return new {Name}(session);
    }}
".FormatWith(new
 {
     TypeName = _properName,
     Name = _entityInfo.EntityType.Name + "Proxy"
 });
        }

        protected override string GenerateOtherMethodsCode()
        {
            if (!_entityInfo.EntityType.IsPublic && !_entityInfo.EntityType.IsNestedPublic)
            {
                throw new CodeCompilerException("The entity type {Name} is not public.", _entityInfo.EntityType);
            }

            return @"
    public class {Name} : {TypeName}
    {{
        private ISession Session {{ get; set; }}

        public {Name}(ISession session) 
        {{
            Session = session;
        }}

        {OverridableMembers}
    }}

    public static void Init(Type type) 
    {{
    }}

    public static MapResult Map({TypeName} entity, IDataReader reader)
    {{
        object[] data;
        try 
        {{

{DataFetchDiagnostics} Diagnostic.Timed(() => {{
            data = reader.GetValues();
{DataFetchDiagnostics} }}, ""Mapping: {{1}}"");

        }}
        catch (Exception ex)
        {{
            throw new Exception(""Unable to fetch data from data source."", ex);
        }}
{EntityMappingDiagnostics} Diagnostic.Timed(() => {{
        {Body}
{EntityMappingDiagnostics} }}, ""Mapping: {{1}}"");

        return new MapResult {{
            Entity = entity
        }};
    }}

    public static Object EntityCast(Object source)
    {{
        return source;
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
     DataFetchDiagnostics = !Configuration.PerformanceDiagnosticOptions.DataFetch ? "//" : string.Empty,
     EntityMappingDiagnostics = !Configuration.PerformanceDiagnosticOptions.EntityMapping ? "//" : string.Empty,
     TypeName = _properName,
     Name = _entityInfo.EntityType.Name + "Proxy",
     OverridableMembers = GenerateOverridableMembers(_entityInfo),
     Body = GenerateBody(_entityInfo)
 });

        }

        private string GenerateWhereClause(IEntityInfo entityInfo, PropertyInfo property)
        {
            var result = new StringBuffer();

            var entityType = property.PropertyType.GetGenericArguments().First();
            var filterFields = _configuration.CollectionFilterGetters
                .Select(g => g(property))
                .Where(f => f != null)
                .Select(f => f.ToLower())
                .ToList();

            var foreignTable = _entityBag[entityType];

            foreach (var key in entityInfo.KeyColumns)
            {
                //
                // If there's no filter set use the key as convention.
                //
                if (!filterFields.Any())
                {
                    filterFields.Add(key.ColumnName.ToLower());
                }

                if ((foreignTable.ForeignKeys == null || !foreignTable.ForeignKeys.Any()) && !filterFields.Any())
                {
                    throw new MappingException(Errors.Mapping_CollectionNeedsBindedField,
                      new
                      {
                          TargetName = foreignTable.EntityName,
                          FieldName = key.ColumnName,
                          CurrentName = entityInfo.EntityName
                      });
                }

                IEntityColumnInfo foreignField = null;

                if (filterFields.Any())
                {
                    foreach (var filterField in filterFields)
                    {
                        foreignField = foreignTable.KeyColumns
                            .FirstOrDefault(e => e.ColumnName.ToLower() == filterField);
                    }
                }

                if (foreignField == null && foreignTable.ForeignKeys != null)
                {
                    foreignField = foreignTable.ForeignKeys
                        .SelectMany(f => f.ForeignKeyLinks).Select(pair => pair.Value)
                        .FirstOrDefault(f => f.BindOrName.Equals(key.ColumnName, StringComparison.InvariantCultureIgnoreCase));
                }

                if (foreignField != null)
                {

                    if (foreignField.Property.PropertyType.IsAssignableFrom(entityInfo.EntityType))
                    {

                        result += "e.{Field} == this && ".FormatWith(new { Field = foreignField.Property.Name });
                    }
                    else
                    {

                        result += "e.{Field} == this.{KeyField} && "
                            .FormatWith(new
                            {
                                Field = foreignField.Property.Name,
                                KeyField = key.Property.Name
                            });
                    }

                }
                else
                {
                    throw new MappingException("\nThe model for [{TargetName}] needs a bind to the field [{FieldName}] in the [{CurrentName}] model.",
                        new
                        {
                            TargetName = foreignTable.EntityName,
                            FieldName = key.ColumnName,
                            CurrentName = entityInfo.EntityName
                        });
                }
            }


            result.RemoveLast(" && ");

            return result;
        }

        private string GenerateOverridableMembers(IEntityInfo entityInfo)
        {
            var result = new StringBuffer();

            foreach (var column in entityInfo.ForeignKeys.Where(e => e.Property.IsVirtual()))
            {


                result += @"
        {Type} _{Name};
        Boolean _{Name}Loaded;

        public override {Type} {Name}
        {{
            get
            {{
                if ( !_{Name}Loaded )
                {{
                    {FillLinks}
                    _{Name} = Session.GetById(_{Name});
                    _{Name}Loaded = _{Name} != null;
                }}

                return _{Name};
            }}
            set
            {{
                _{Name} = value;
            }}
        }}
".FormatWith(new
 {
     Type = column.Property.PropertyType.FullName.Replace('+', '.'),
     column.Property.Name,
     FillLinks = GenerateCodeForLinks(column)
 });

            }

            foreach (var property in entityInfo.EntityType.GetProperties().Where(e => e.IsVirtual() && e.PropertyType.IsCollection()))
            {
                var entityType = property.PropertyType.GetGenericArguments().First();
                result += @"
        {Type} _{Name};

        public override {Type} {Name}
        {{
            get
            {{
                return _{Name} ?? Session
                                    .GetAll<{EntityType}>()
                                    .Where(e => {WhereClause}){ToList};
            }}
            set
            {{
                _{Name} = value;
            }}
        }}
".FormatWith(new
 {
     Type = $"{property.PropertyType.Namespace}.{property.PropertyType.Name.Replace("`1", "")}<{entityType.FullName.Replace('+', '.')}>",
     EntityType = entityType.FullName.Replace('+', '.'),
     ToList = property.PropertyType.Name.Contains("ICollection") ? ".ToList()" : string.Empty,
     property.Name,
     WhereClause = GenerateWhereClause(entityInfo, property)
 });
            }

            return result;
        }

        private string GenerateCodeForLinks(IEntityColumnInfo column)
        {
            if (!column.ForeignKeyLinks.Any())
            {
                return string.Empty;
            }

            var result = new StringBuffer();

            result += "_{Name} = new {Name} {{\n".FormatWith(new { column.Property.PropertyType.Name });

            foreach (var foreignKeyLink in column.ForeignKeyLinks)
            {
                result += "         {Dependency} = {Value},\n"
                    .FormatWith(new
                                {
                                    column.Property.Name,
                                    Dependency = foreignKeyLink.Value.Property.Name,
                                    Value = GenerateDependencyValue(column, foreignKeyLink)
                                });
            }

            return result + "   };\n";
        }

        private string GenerateDependencyValue(IEntityColumnInfo column, KeyValuePair<IEntityColumnInfo, IEntityColumnInfo> foreignKeyLink)
        {
            return "this." + foreignKeyLink.Key.Property.Name;

        }

        private  string GenerateBody(IEntityInfo entityInfo)
        {
            var result = new StringBuffer();

            for (var i = 0; i < entityInfo.Columns.Count; i++)
            {
                var column = entityInfo.Columns[i];
                
                const string setter = @"
{FieldMappingDiagnostics} Diagnostic.Timed(() => {{
                try
                {{

                    {IsBoolean} if (data[{i}] is string) {{ entity.{FieldName} = ""-1"".Equals(data[{i}]); }}
                    {IsBoolean} else
                    {Comment}if (data[{i}] != DBNull.Value)
                        entity.{FieldName} = {Value};
                    
                }}
                catch (InvalidCastException) 
                {{ 
                     try
                     {{
                        {IsNotBoolean} {NotComment} entity.{FieldName} = ({Type})Convert.ChangeType({ValueNoType}, typeof({Type}));
                     }}
                     catch ( Exception ex)
                     {{
                        throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value of type ["" + data[{i}].GetType() + ""]."", ex);
                     }}
                }}
                catch ( Exception ex)
                {{
                    throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value of type ["" + data[{i}].GetType() + ""]."", ex);
                }}
{FieldMappingDiagnostics} }}, ""{FieldName} -> Mapping {Type}: {{1}} with value "" + data[{i}] + "" "" + data[{i}].GetType());
";

                string value = GenerateFieldAssignment(i, column);
                value = value.Substring(3, value.Length - 3);

                result += setter.FormatWith(new
                 {
                     FieldMappingDiagnostics = !Configuration.PerformanceDiagnosticOptions.FieldMapping ? "//" : string.Empty,
                     FieldName = column.Property.Name,
                     i,
                     Value = value,
                     ValueNoType = value
                        .Replace($"({column.Property.PropertyType.Name})", "")
                        .Replace("default", $"default({column.Property.PropertyType.Name})"),
                     Comment = column.ForeignKey == null ? "//" : string.Empty,
                     NotComment = column.ForeignKey == null ? string.Empty : "//",
                     Type = column.Property.PropertyType.Name,
                     IsBoolean = column.Property.PropertyType == typeof(bool) ? string.Empty : "//",
                     IsNotBoolean = column.Property.PropertyType != typeof(bool) ? string.Empty : "//"
                 });

            }

            return result;
        }

        private static StringBuffer GenerateFieldAssignment(int i, IEntityColumnInfo column)
        {
            StringBuffer result = " = ";
            if (column.Property.PropertyType.IsFrameworkType() || column.Property.PropertyType.IsEnum)
            {
                if (column.HasFormattingStyles && column.Property.PropertyType == typeof(DateTime))
                {
                    result += "DateTime.ParseExact((Parse(data[{i}]) ?? default({Type})).ToString(), new[] {{ {Formats} }}, CultureInfo.InvariantCulture, DateTimeStyles.None)"
                        .FormatWith(new
                        {
                            i,
                            Formats = string.Join(", ", column.Formats.Select(e => "\"{e}\"".FormatWith(new { e }))),
                            Type = column.Property.PropertyType.Name
                        });
                    return result;
                }
                
                if (column.HasFormattingStyles && column.Property.PropertyType == typeof(double))
                {
                    result += "Convert.ToDouble(Parse(data[{i}]), new NumberFormatInfo {{ NumberDecimalSeparator = \"{DecimalSeparator}\", NumberGroupSeparator = \"{GroupSeparator}\", NumberGroupSizes = new[] {{ {GroupSizes} }} }})"
                        .FormatWith(new
                        {
                            i,
                            DecimalSeparator = column.NumberFormat.NumberDecimalSeparator,
                            GroupSeparator = column.NumberFormat.NumberGroupSeparator,
                            GroupSizes = string.Join(", ", column.NumberFormat.NumberGroupSizes)
                        });
                    return result;
                }

                if (column.Property.PropertyType == typeof (bool))
                {
                    result += "Convert.ToBoolean(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = column.Property.PropertyType.Name });    
                    return result;
                }

                result += "({Type})(Parse(data[{i}]) ?? default({Type}))".FormatWith(new { i, Type = column.Property.PropertyType.Name });
            }
            else
            {
                result += "new {Type} {{ {BoundField} {Value} }}".FormatWith(new
                {
                    Type = column.Property.PropertyType.FullName.Replace('+', '.'),
                    BoundField = column.ForeignKey.Property.Name,
                    Value = GenerateFieldAssignment(i, column.ForeignKey)
                });
            }

            return result;
        }

    }
}
