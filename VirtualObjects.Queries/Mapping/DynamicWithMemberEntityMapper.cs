using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{
    class DynamicWithMemberEntityMapper : DynamicTypeEntityMapper
    {
        public override bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.Fields();

            return context.OutputType.IsDynamic() &&
                   properties.Any(e => !e.FieldType.IsFrameworkType()) &&
                   properties.Any(e => e.FieldType.IsFrameworkType());
        }

        public override void PrepareMapper(MapperContext context)
        {
            context.OutputTypeSetters = context.OutputType
                .Fields()
                .Select(e =>
                {
                    if (e.FieldType.IsFrameworkType())
                    {
                        return e.DelegateForSetFieldValue();    
                    }

                    var ctx = new MapperContext
                    {
                        EntityInfo = context.Mapper.Map(e.FieldType),
                        OutputType = e.FieldType,
                        EntityProvider = context.EntityProvider,
                        Mapper = context.Mapper
                    };

                    return (o, value) =>
                    {

                        var name = ParseName(e.Name);

                        var column = ctx.EntityInfo.KeyColumns.FirstOrDefault();

                        if (column != null)
                        {
                            column.SetValue(e.Get(o), value);
                            return;
                        }

                        throw new MappingException("Unable to map the field {Name} with {EntityName}.", new { Name = name, ctx.EntityInfo.EntityName });
                    };
                })
                .ToList();

        }

        private string ParseName(string name)
        {
            name = name.Substring(1, name.Length - 1);
            name = name.Substring(0, name.LastIndexOf('>'));

            return name;
        }
    }
}