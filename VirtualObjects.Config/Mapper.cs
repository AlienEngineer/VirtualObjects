using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    class Mapper : IMapper
    {
        public IEnumerable<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public IEnumerable<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public IEnumerable<Func<Type, String>> EntityNameGetters { get; set; }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            var columns = MapColumns(entityType.GetProperties()).ToList();

            return new EntityInfo
            {
                EntityName = GetName(entityType),
                EntityType = entityType,
                Columns = columns,
                KeyColumns = columns.Where(e => e.IsKey).ToList()
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

        private IEnumerable<IEntityColumnInfo> MapColumns(IEnumerable<PropertyInfo> properties)
        {
            return properties.Select(MapColumn);
        }

        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo)
        {
            return new EntityColumnInfo
            {
                ColumnName = GetName(propertyInfo),
                IsKey = GetIsKey(propertyInfo),
                IsIdentity = GetIsIdentity(propertyInfo),
                Property = propertyInfo
            };
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