using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Fasterflect;
using VirtualObjects.Exceptions;
using VirtualObjects.CodeGenerators;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.Config
{
    /// <summary>
    /// Maps a type into an IEntityInfo. Caches out the results.
    /// </summary>
    class Mapper : IMapper
    {
        private readonly IOperationsProvider _operationsProvider;
        private readonly IEntityInfoCodeGeneratorFactory _codeGeneratorFactory;
        private readonly ITranslationConfiguration _configuration;
        private readonly IEntityBag _entityBag;
        private readonly IEntityProvider entityProvider;

        public Mapper(IEntityBag entityBag, ITranslationConfiguration configuration, IEntityProvider entityProvider, IOperationsProvider operationsProvider, IEntityInfoCodeGeneratorFactory codeGeneratorFactory)
        {
            this.entityProvider = entityProvider;
            _configuration = configuration;
            _codeGeneratorFactory = codeGeneratorFactory;
            _operationsProvider = operationsProvider;
            _entityBag = entityBag;
        }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            if ( entityType.IsFrameworkType() || entityType.IsDynamic() )
            {
                return null;
            }

            if ( entityType.IsProxy() )
            {
                return Map(entityType.BaseType);
            }

            return MapType(entityType);
        }

        private void MapRelatedEntities(IEntityInfo entityInfo)
        {
            foreach ( var property in entityInfo.EntityType.GetProperties().Where(e => e.IsVirtual() && e.PropertyType.IsCollection()) )
            {
                var entityType = property.PropertyType.GetGenericArguments().First();
                Map(entityType);
            }
        }

        private IEntityInfo MapType(Type entityType)
        {
            IEntityInfo entityInfo;

            if ( _entityBag.TryGetValue(entityType, out entityInfo) )
            {
                return entityInfo;
            }

            _entityBag[entityType] = entityInfo = new EntityInfo
            {
                EntityName = GetName(entityType),
                EntityType = entityType
            };

            entityInfo.Columns = MapColumns(entityType.Properties(), entityInfo).ToList();
            entityInfo.KeyColumns = entityInfo.Columns.Where(e => e.IsKey).ToList();

            int i = 0;
            foreach ( var column in entityInfo.Columns )
            {
                column.Index = i++;
                column.ForeignKey = GetForeignKey(column.Property);
            }

            entityInfo.Columns = WrapColumns(entityInfo.Columns).ToList();
            entityInfo.KeyColumns = entityInfo.Columns.Where(e => e.IsKey).ToList();

#if DEBUG
            entityInfo.Columns.ForEach(e =>
            {
                if ( e.ForeignKey == null && !e.Property.PropertyType.IsFrameworkType() )
                {
                    throw new ConfigException("The column [{ColumnName}] returns a complex type but is not associated with another key.", e);
                }

                if ( e.ForeignKey != null && !(e is EntityBoundColumnInfo) )
                {
                    throw new ConfigException("The column [{ColumnName}] returns a complex type but is not associated with another key.", e);
                }
            });
#endif
            //
            // Calculation of the entity Key.
            //
            entityInfo.KeyHashCode = obj => entityInfo.KeyColumns
                .Aggregate(new StringBuffer(), (current, key) => current + key.GetFieldFinalValue(obj).ToString())
                .GetHashCode();

            entityInfo.Identity = entityInfo.KeyColumns.FirstOrDefault(e => e.IsIdentity);
            entityInfo.VersionControl = entityInfo.Columns.FirstOrDefault(e => e.IsVersionControl);

            entityInfo.ForeignKeys = entityInfo.Columns.Where(e => e.ForeignKey != null).ToList();

            foreach ( var column in entityInfo.Columns )
            {
                column.ForeignKeyLinks = GetForeignKeyLinks(column, column.ForeignKey, entityInfo).ToList();
            }

            MapRelatedEntities(entityInfo);

            entityInfo.Operations = _operationsProvider.CreateOperations(entityInfo);
            var codeGenerator = _codeGeneratorFactory.Make(entityInfo);

            codeGenerator.GenerateCode();

            entityInfo.MapEntity = codeGenerator.GetEntityMapper();
            entityInfo.EntityFactory = codeGenerator.GetEntityProvider();
            entityInfo.EntityProxyFactory = codeGenerator.GetEntityProxyProvider();
            entityInfo.EntityCast = codeGenerator.GetEntityCast();

            return entityInfo;
        }

        private static IEnumerable<IEntityColumnInfo> WrapColumns(IEnumerable<IEntityColumnInfo> columns)
        {
            return columns.Select(WrapColumn);
        }

        private static IEntityColumnInfo WrapColumn(IEntityColumnInfo column)
        {
            if ( column.ForeignKey != null )
            {
                return WrapWithBoundColumn(column);
            }
            
            if ( column.Property.PropertyType == typeof(DateTime) )
            {
                return WrapWithDatetimeColumn(column);
            }
            
            return column;
        }

        private static IEntityColumnInfo WrapWithDatetimeColumn(IEntityColumnInfo column)
        {
            return new EntityDateTimeColumnInfo
            {
                Property = column.Property,
                ColumnName = column.ColumnName,
                EntityInfo = column.EntityInfo,
                ForeignKey = column.ForeignKey,
                IsIdentity = column.IsIdentity,
                IsKey = column.IsKey,
                ValueGetter = column.ValueGetter,
                ValueSetter = column.ValueSetter,
                IsVersionControl = column.IsVersionControl
            };
        }

        private static IEntityColumnInfo WrapWithBoundColumn(IEntityColumnInfo column)
        {
            return new EntityBoundColumnInfo
            {
                Property = column.Property,
                ColumnName = column.ColumnName,
                EntityInfo = column.EntityInfo,
                ForeignKey = column.ForeignKey,
                ForeignKeyLinks = column.ForeignKeyLinks,
                IsIdentity = column.IsIdentity,
                IsKey = column.IsKey,
                ValueGetter = column.ValueGetter,
                ValueSetter = column.ValueSetter
            };
        }

        #endregion

        #region Auxiliary Entity mapping methods

        private string GetName(Type entityType)
        {
            return _configuration.EntityNameGetters
                .Select(nameGetter => nameGetter(entityType))
                .FirstOrDefault(name => !String.IsNullOrEmpty(name));
        }

        #endregion

        #region Auxiliary column mapping methods

        private IEnumerable<IEntityColumnInfo> MapColumns(IEnumerable<PropertyInfo> properties, IEntityInfo entityInfo)
        {
            return properties
                .Where(e => !e.PropertyType.IsGenericType || !e.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                .Where(e => !ShouldIgnore(e))
                .Select(e => MapColumn(e, entityInfo));
        }

        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo, IEntityInfo entityInfo)
        {
            var columnName = GetName(propertyInfo);
            
            var column = new EntityColumnInfo
            {
                EntityInfo = entityInfo,
                ColumnName = columnName,
                IsKey = GetIsKey(propertyInfo),
                IsIdentity = GetIsIdentity(propertyInfo),
                IsVersionControl = _configuration.ColumnVersionFieldGetters.Any(isVersion => isVersion(propertyInfo)),
                IsComputed = _configuration.ComputedColumnGetters.Any(isComputed => isComputed(propertyInfo)),
                Property = propertyInfo,
                ValueGetter = MakeValueGetter(columnName, propertyInfo.DelegateForGetPropertyValue()),
                ValueSetter = MakeValueSetter(columnName, propertyInfo.DelegateForSetPropertyValue())
            };

            return column;
        }

        private IEntityColumnInfo GetForeignKey(PropertyInfo propertyInfo)
        {
            if ( propertyInfo.PropertyType.IsFrameworkType() )
            {
                return null;
            }

            var entity = Map(propertyInfo.PropertyType);

            var keyName = _configuration.ColumnForeignKeyGetters
                .Select(keyGetter => keyGetter(propertyInfo))
                .FirstOrDefault();

            var foreignKey = String.IsNullOrEmpty(keyName) ?
                entity.KeyColumns.FirstOrDefault() :
                entity[keyName];

            if ( foreignKey == null && _configuration.ColumnForeignKeyGetters.Any() )
            {
                throw new ConfigException(Errors.Mapping_UnableToGetForeignKey, propertyInfo);
            }

            return foreignKey;
        }

        private IEnumerable<IEntityColumnInfo> GetForeignKeyLinks(IEntityColumnInfo column, IEntityColumnInfo foreignKey, IEntityInfo currentEntity) 
        {
            if ( column.ForeignKey != null )
            {
                var links = _configuration.ColumnForeignKeyLinksGetters
                    .Select(keyGetter => keyGetter(column.Property))
                    .FirstOrDefault();

                if ( String.IsNullOrEmpty(links) )
                {
                    foreach ( var item in foreignKey.EntityInfo.KeyColumns )
                    {
                        yield return item;
                    } 
                }
                else
                {
                    foreach ( var link in links.Split(';') )
                    {
                        var columnLink = currentEntity[link];
                        
                        yield return columnLink;
                    }
                }
            }
        }

        private bool ShouldIgnore(PropertyInfo propertyInfo)
        {
            return _configuration.ColumnIgnoreGetters.Any(e => e(propertyInfo));
        }

        private bool GetIsIdentity(PropertyInfo propertyInfo)
        {
            return _configuration.ColumnIdentityGetters.Any(e => e(propertyInfo));
        }

        private bool GetIsKey(PropertyInfo propertyInfo)
        {
            return _configuration.ColumnKeyGetters.Any(e => e(propertyInfo));
        }

        private string GetName(PropertyInfo propertyInfo)
        {
            return _configuration.ColumnNameGetters
                .Select(nameGetter => nameGetter(propertyInfo))
                .FirstOrDefault(name => !String.IsNullOrEmpty(name));
        }

        private static Func<Object, Object> MakeValueGetter(String fieldName, MemberGetter getter)
        {
            return entity =>
            {
                try
                {
                    return getter(entity);
                }
                catch ( Exception ex )
                {
                    throw new ConfigException(
                        Errors.Mapping_UnableToGetValue,
                        new
                        {
                            FieldName = fieldName
                        }, ex);
                }
            };
        }

        private static Action<Object, Object> MakeValueSetter(String fieldName, MemberSetter setter)
        {
            return (entity, value) =>
            {
                try
                {
                    setter(entity, value);
                }
                catch ( Exception ex )
                {
                    throw new ConfigException(
                        Errors.Mapping_UnableToSetValue,
                        new
                        {
                            FieldName = fieldName,
                            Value = value
                        }, ex);
                }
            };
        }

        #endregion

        #region IDisposable Members
        private bool _disposed;
        
        
        

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {   
                _disposed = true;
            }
        }

        #endregion
    }
}