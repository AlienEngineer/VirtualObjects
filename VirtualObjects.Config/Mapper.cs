using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Config
{

    /// <summary>
    /// Maps a type into an IEntityInfo. Caches out the results.
    /// </summary>
    class Mapper : IMapper
    {
        public IEnumerable<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public IEnumerable<Func<Type, String>> EntityNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, String>> ColumnForeignKey { get; set; }

        private readonly IDictionary<Type, EntityInfo> _cacheEntityInfos;

        public Mapper()
        {
            _cacheEntityInfos = new Dictionary<Type, EntityInfo>();
        }


        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            if ( entityType.IsFrameworkType() )
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
            entityInfo.KeyColumns = entityInfo.Columns.Where(e => e.IsKey);

            foreach (var column in entityInfo.Columns)
            {
                column.ForeignKey = GetForeignKey(column.Property);
            }

            return entityInfo;
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
            return properties.Select(e => MapColumn(e, entityInfo));
        }

        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo, EntityInfo entityInfo)
        {
            return new EntityColumnInfo
            {
                EntityInfo = entityInfo,
                ColumnName = GetName(propertyInfo),
                IsKey = GetIsKey(propertyInfo),
                IsIdentity = GetIsIdentity(propertyInfo),
                Property = propertyInfo
            };
        }

        private IEntityColumnInfo GetForeignKey(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsFrameworkType())
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

            if (foreignKey == null && ColumnForeignKey.Any())
            {
                throw new VirtualObjectsException(Errors.Mapping_UnableToGetForeignKey, propertyInfo);
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

        #endregion

    }
}