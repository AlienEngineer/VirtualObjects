using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Fasterflect;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;
using VirtualObjects.CodeGenerators;

namespace VirtualObjects.Config
{
    /// <summary>
    /// Maps a type into an IEntityInfo. Caches out the results.
    /// </summary>
    class Mapper : IMapper
    {

        // private readonly SessionContext _sessionContext;
        private readonly IOperationsProvider _operationsProvider;
        private readonly IEntityProvider _entityProvider;
        private readonly IEntityMapper _entityMapper;
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnIgnoreGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public IEnumerable<Func<Type, String>> EntityNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, String>> ColumnForeignKey { get; set; }
        public IEnumerable<Func<PropertyInfo, String>> ColumnForeignKeyLinks { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnVersionField { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ComputedColumnGetters { get; set; }

        private IDictionary<Type, EntityInfo> _cacheEntityInfos;

        public Mapper(IOperationsProvider operationsProvider, IEntityProvider entityProvider, IEntityMapper entityMapper, SessionContext sessionContext)
        {
            _operationsProvider = operationsProvider;
            _entityProvider = entityProvider;
            _entityMapper = entityMapper;
            // _sessionContext = sessionContext;
            _cacheEntityInfos = new Dictionary<Type, EntityInfo>();
        }

        public Mapper(IOperationsProvider operationsProvider, IEntityProvider entityProvider, IEntityMapper entityMapper)
        {
            _operationsProvider = operationsProvider;
            _entityProvider = entityProvider;
            _entityMapper = entityMapper;
            _cacheEntityInfos = new Dictionary<Type, EntityInfo>();
        }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            if ( entityType.IsFrameworkType() || entityType.IsDynamic() )
            {
                return null;
            }

            if ( entityType.Name.EndsWith("Proxy") && entityType.Name.StartsWith(entityType.BaseType.Name) )
            {
                return Map(entityType.BaseType);
            }

            return MapType(entityType);
        }

        private IEntityInfo MapType(Type entityType)
        {
            EntityInfo entityInfo;

            if ( _cacheEntityInfos.TryGetValue(entityType, out entityInfo) )
            {
                return entityInfo;
            }

            _cacheEntityInfos[entityType] = entityInfo = new EntityInfo
            {
                EntityName = GetName(entityType),
                EntityType = entityType
            };

            entityInfo.Columns = MapColumns(entityType.Properties(), entityInfo).ToList();
            entityInfo.KeyColumns = entityInfo.Columns.Where(e => e.IsKey).ToList();

            foreach ( var column in entityInfo.Columns )
            {
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

            entityInfo.Operations = _operationsProvider.CreateOperations(entityInfo);
            entityInfo.EntityProvider = _entityProvider.GetProviderForType(entityType);
            // entityInfo.EntityProvider.PrepareProvider(entityType, _sessionContext);
            entityInfo.EntityMapper = _entityMapper;

            var codeGenerator = new EntityInfoCodeGenerator(entityInfo, this);

            codeGenerator.GenerateCode();

            entityInfo.MapEntity = codeGenerator.GetEntityMapper();
            entityInfo.EntityFactory = codeGenerator.GetEntityProvider();
            entityInfo.EntityProxyFactory = codeGenerator.GetEntityProxyProvider();

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
            return EntityNameGetters
                .Select(nameGetter => nameGetter(entityType))
                .FirstOrDefault(name => !String.IsNullOrEmpty(name));
        }

        #endregion

        #region Auxiliary column mapping methods

        private IEnumerable<IEntityColumnInfo> MapColumns(IEnumerable<PropertyInfo> properties, EntityInfo entityInfo)
        {
            return properties
                .Where(e => !e.PropertyType.IsGenericType || !e.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                .Where(e => !ShouldIgnore(e))
                .Select(e => MapColumn(e, entityInfo));
        }

        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo, EntityInfo entityInfo)
        {
            var columnName = GetName(propertyInfo);
            
            var column = new EntityColumnInfo
            {
                EntityInfo = entityInfo,
                ColumnName = columnName,
                IsKey = GetIsKey(propertyInfo),
                IsIdentity = GetIsIdentity(propertyInfo),
                IsVersionControl = ColumnVersionField.Any(isVersion => isVersion(propertyInfo)),
                IsComputed = ComputedColumnGetters.Any(isComputed => isComputed(propertyInfo)),
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

            var keyName = ColumnForeignKey
                .Select(keyGetter => keyGetter(propertyInfo))
                .FirstOrDefault();

            var foreignKey = String.IsNullOrEmpty(keyName) ?
                entity.KeyColumns.FirstOrDefault() :
                entity[keyName];

            if ( foreignKey == null && ColumnForeignKey.Any() )
            {
                throw new ConfigException(Errors.Mapping_UnableToGetForeignKey, propertyInfo);
            }

            return foreignKey;
        }

        private IEnumerable<IEntityColumnInfo> GetForeignKeyLinks(IEntityColumnInfo column, IEntityColumnInfo foreignKey, IEntityInfo currentEntity) 
        {
            if ( column.ForeignKey != null )
            {
                var links = ColumnForeignKeyLinks
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
            return ColumnIgnoreGetters.Any(e => e(propertyInfo));
        }

        private bool GetIsIdentity(PropertyInfo propertyInfo)
        {
            return ColumnIdentityGetters.Any(e => e(propertyInfo));
        }

        private bool GetIsKey(PropertyInfo propertyInfo)
        {
            return ColumnKeyGetters.Any(e => e(propertyInfo));
        }

        private string GetName(PropertyInfo propertyInfo)
        {
            return ColumnNameGetters
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
                if ( disposing )
                {
                    _cacheEntityInfos.Clear();
                }

                _cacheEntityInfos = null;
                ColumnNameGetters = null;
                ColumnKeyGetters = null;
                ColumnIdentityGetters = null;
                EntityNameGetters = null;
                ColumnForeignKey = null;
                ColumnVersionField = null;

                _disposed = true;
            }
        }

        #endregion
    }
}