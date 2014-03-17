using System;
using System.Linq;
using VirtualObjects.Exceptions;
using Fasterflect;
using System.Reflection;
using VirtualObjects.Config;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGenerator : EntityCodeGenerator
    {
        private readonly IEntityInfo _entityInfo;
        private readonly IEntityBag entityBag;
        private readonly String properName;

        public EntityInfoCodeGenerator(IEntityInfo info, IEntityBag entityBag)
            : base("Internal_Builder_" + info.EntityType.Name)
        {
            this.entityBag = entityBag;
            _entityInfo = info;
            properName = _entityInfo.EntityType.FullName.Replace("+", ".");


            AddReference(_entityInfo.EntityType);
            AddReference(typeof(Object));
            AddReference(typeof(ISession));
            AddReference(typeof(IQueryable));

            AddNamespace(_entityInfo.EntityType.Namespace);
            AddNamespace("VirtualObjects");
            AddNamespace("System");
            AddNamespace("System.Linq");
        }

        protected override string GenerateMapObjectCode()
        {
            return @"
    public static void MapObject(Object entity, Object[] data)
    {{
        Map(({TypeName})entity, data);
    }}
".FormatWith(new { TypeName = properName });
        }

        protected override string GenerateMakeCode()
        {
            return @"
    public static Object Make()
    {{
        return new {TypeName}();
    }}
".FormatWith(new { TypeName = properName });
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
     TypeName = properName,
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
     Name = _entityInfo.EntityType.Name + "Proxy",
     OverridableMembers = GenerateOverridableMembers(_entityInfo),
     Body = GenerateBody(_entityInfo)
 });

        }


        private String GenerateWhereClause(IEntityInfo entityInfo, PropertyInfo property)
        {
            var result = new StringBuffer();

            var entityType = property.PropertyType.GetGenericArguments().First();

            var foreignTable = entityBag[entityType];

            foreach (var key in entityInfo.KeyColumns)
            {
                var foreignField = foreignTable.ForeignKeys
                    .FirstOrDefault(f => f.BindOrName.Equals(key.ColumnName, StringComparison.InvariantCultureIgnoreCase));

                if (foreignField != null)
                {

                    if (foreignField.Property.PropertyType == entityInfo.EntityType)
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
     column.Property.Name
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
                                    .Where(e => {WhereClause});
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
     property.Name,
     WhereClause = GenerateWhereClause(entityInfo, property)
 });
            }

            return result;
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
