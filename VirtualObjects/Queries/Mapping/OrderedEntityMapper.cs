using System;
using System.Data;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// Maps an entity based on the EntityInfo.
    /// Assumes that the order of the columns is the same as the result in the datareader.
    /// </summary>
    class OrderedEntityMapper : IEntityMapper
    {
        public virtual object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var i = 0;
            foreach ( var column in mapContext.EntityInfo.Columns )
            {
                var value = reader.GetValue(i++);
                try
                {
                    column.SetFieldFinalValue(buffer, value);
                }
                catch ( ConfigException ex)
                {
                    TrySet(buffer, column, value, ex);
                }
            }

            return buffer;
        }

        public virtual bool CanMapEntity(MapperContext context)
        {
            return context.EntityInfo != null && context.OutputType == context.EntityInfo.EntityType;
        }

        public virtual void PrepareMapper(MapperContext context)
        {
            // no prepare needed.
        }

        static void TrySet(object entity, IEntityColumnInfo field, object value, Exception ex)
        {
            try
            {
                if ( field.Property.PropertyType == typeof(Guid) )
                {
#if NET35
                    if ( value != null )
                        field.SetFieldFinalValue(entity, new Guid(value.ToString()));
#else
                    if ( value != null )
                        field.SetFieldFinalValue(entity, Guid.Parse(value.ToString()));
#endif
                }
                else
                {
                    field.SetFieldFinalValue(entity, Convert.ChangeType(value, field.Property.PropertyType));
                }
            }
            catch ( Exception )
            {
                throw ex;
            }
        }
    }
}
