using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Fasterflect;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Config
{

    /// <summary>
    /// Maps a type into an IEntityInfo. Caches out the results.
    /// </summary>
    class Mapper : IMapper
    {
        private readonly IOperationsProvider _operationsProvider;
        public IEnumerable<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public IEnumerable<Func<Type, String>> EntityNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, String>> ColumnForeignKey { get; set; }
        public IEnumerable<Func<PropertyInfo, bool>> ColumnVersionField { get; set; }

        private IDictionary<Type, EntityInfo> _cacheEntityInfos;

        public Mapper(IOperationsProvider operationsProvider)
        {
            _operationsProvider = operationsProvider;
            _cacheEntityInfos = new Dictionary<Type, EntityInfo>();
        }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            if ( entityType.IsFrameworkType() || entityType.IsDynamic() )
            {
                return null;
            }

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
            entityInfo.KeyHashCode = (obj) => entityInfo.KeyColumns
                .Aggregate(new StringBuffer(), (current, key) => current + key.GetFieldFinalValue(obj).ToString())
                .GetHashCode();

            entityInfo.Identity = entityInfo.KeyColumns.FirstOrDefault(e => e.IsIdentity);

            entityInfo.ForeignKeys = entityInfo.Columns.Where(e => e.ForeignKey != null).ToList();

            entityInfo.Operations = _operationsProvider.CreateOperations(entityInfo);

            return entityInfo;
        }

        private IEnumerable<IEntityColumnInfo> WrapColumns(IEnumerable<IEntityColumnInfo> columns)
        {
            foreach ( var column in columns )
            {
                if ( column.ForeignKey != null )
                {
                    yield return WrapWithBoundColumn(column);
                }
                else if ( column.Property.PropertyType == typeof(DateTime) )
                {
                    yield return WrapWithDatetimeColumn(column);
                }
                else
                {
                    yield return column;
                }
            }
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

        #region Auxilary column mapping methods

        private IEnumerable<IEntityColumnInfo> MapColumns(IEnumerable<PropertyInfo> properties, EntityInfo entityInfo)
        {
            return properties
                .Where(e => !e.PropertyType.IsGenericType || !e.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
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
            return (entity) =>
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