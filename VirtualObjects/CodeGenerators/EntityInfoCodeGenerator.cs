using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VirtualObjects.Exceptions;
using Fasterflect;
using System.Reflection;
using VirtualObjects.Config;
using System.Data;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGenerator : EntityCodeGenerator
    {
        private readonly IEntityInfo _entityInfo;
        private readonly ITranslationConfiguration _configuration;
        private readonly IEntityBag _entityBag;
        private readonly String _properName;

        public EntityInfoCodeGenerator(IEntityInfo info, IEntityBag entityBag, ITranslationConfiguration configuration, SessionConfiguration sessionConfiguration)
            : base(info.EntityType.Namespace.Replace(".", "_") + "_Internal_Builder_" + info.EntityType.Name, info.EntityType, sessionConfiguration)
        {
            _configuration = configuration;
            _entityBag = entityBag;
            _entityInfo = info;
            _properName = _entityInfo.EntityType.FullName.Replace("+", ".");


            AddReference(_entityInfo.EntityType);
            AddReference(typeof(Object));
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
            data = reader.GetValues();
        }}
        catch (Exception ex)
        {{
            throw new Exception(""Unable to fetch data from data source."", ex);
        }}
        {Body}

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
     TypeName = _properName,
     Name = _entityInfo.EntityType.Name + "Proxy",
     OverridableMembers = GenerateOverridableMembers(_entityInfo),
     Body = GenerateBody(_entityInfo)
 });

        }

        private String GenerateWhereClause(IEntityInfo entityInfo, PropertyInfo property)
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

        private String GenerateOverridableMembers(IEntityInfo entityInfo)
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
     Type = String.Format("{0}.{1}<{2}>", property.PropertyType.Namespace, property.PropertyType.Name.Replace("`1", ""), entityType.FullName.Replace('+', '.')),
     EntityType = entityType.FullName.Replace('+', '.'),
     ToList = property.PropertyType.Name.Contains("ICollection") ? ".ToList()" : String.Empty,
     property.Name,
     WhereClause = GenerateWhereClause(entityInfo, property)
 });
            }

            return result;
        }

        private String GenerateCodeForLinks(IEntityColumnInfo column)
        {
            var result = new StringBuffer();

            foreach (var foreignKeyLink in column.ForeignKeyLinks)
            {
                result += "_{Name}.{Dependency} = {Value};"
                    .FormatWith(new
                                {
                                    column.Property.Name,
                                    Dependency = foreignKeyLink.Value.Property.Name,
                                    Value = GenerateDependencyValue(column, foreignKeyLink)
                                });
            }

            return result;
        }

        private String GenerateDependencyValue(IEntityColumnInfo column, KeyValuePair<IEntityColumnInfo, IEntityColumnInfo> foreignKeyLink)
        {
            return "this." + foreignKeyLink.Key.Property.Name;

        }

        private static string GenerateBody(IEntityInfo entityInfo)
        {
            var result = new StringBuffer();

            for (int i = 0; i < entityInfo.Columns.Count; i++)
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
                     try
                     {{
                        {NotComment}entity.{FieldName} = ({Type})Convert.ChangeType({ValueNoType}, typeof({Type}));
                     }}
                     catch ( Exception ex)
                     {{
                        throw new Exception(""Error setting value to [{FieldName}] with ["" + data[{i}] + ""] value."", ex);
                     }}
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

        private static StringBuffer GenerateFieldAssignment(int i, IEntityColumnInfo column)
        {
            StringBuffer result = " = ";
            if (column.Property.PropertyType.IsFrameworkType())
            {
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
