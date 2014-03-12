using System;
using System.Data;
using System.Linq;
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
            try
            {
                mapContext.EntityInfo.MapEntity(buffer, reader.GetValues());
            }
            catch ( Exception ex)
            {
                throw;
            }
            
            //var i = 0;
            //foreach ( var column in mapContext.EntityInfo.Columns )
            //{
            //    var value = reader.GetValue(i++);
            //    try
            //    {
            //        var foreignKey = column.SetFieldFinalValue(buffer, value);

            //        //if ( foreignKey != null && column.ForeignKey.EntityInfo.KeyColumns.Count > 1 )
            //        //{
            //        //    MapForeignKeys(reader, column, foreignKey);
            //        //}

            //    }
            //    catch ( ConfigException ex )
            //    {
            //        TrySet(buffer, column, value, ex);
            //    }
            //}

            return buffer;
        }

        private static void MapForeignKeys(IDataReader reader, VirtualObjects.IEntityColumnInfo column, object buffer)
        {
            var mainKeys = column.EntityInfo.ForeignKeys;

            foreach ( var keyColumn in column.ForeignKey.EntityInfo.KeyColumns.Where(e => e.ColumnName != column.ColumnName) )
            {
                object value;
                try
                {
                    var key = mainKeys.First(e => e.BindOrName.Equals(keyColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase));

                    value = reader[key.ColumnName];
                }
                catch ( Exception ex1 )
                {
                    throw new ConfigException("The column [{ColumnName}] of type [{EntityName}] is marked as key but was not found on [{TargetName}].",
                        new
                        {
                            keyColumn.ColumnName,
                            keyColumn.EntityInfo.EntityName,
                            TargetName = column.EntityInfo.EntityName,
                        }, ex1);
                }

                var foreignKey = keyColumn.SetFieldFinalValue(buffer, value);

                if ( foreignKey != null && keyColumn.ForeignKey.EntityInfo.KeyColumns.Count > 1 )
                {
                    MapForeignKeys(reader, column, foreignKey);
                }
            }
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
