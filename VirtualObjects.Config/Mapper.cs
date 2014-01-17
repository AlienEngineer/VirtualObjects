using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class Mapper : IMapper
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
                Columns = columns,
                KeyColumns = columns.Where(e => e.IsKey).ToList()
            };
        }


        #endregion

        #region Auxiliary Entity mapping methods

        private string GetName(Type entityType)
        {
            foreach ( var nameGetter in EntityNameGetters )
            {
                var name = nameGetter(entityType);
                if ( !String.IsNullOrEmpty(name) )
                {
                    return name;
                }
            }
            return null;
        }

        #endregion
        
        #region Auxilary column mapping methods

        private IEnumerable<IEntityColumnInfo> MapColumns(PropertyInfo[] properties)
        {
            return properties.Select(propertyInfo => MapColumn(propertyInfo));
        }

        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo)
        {
            return new EntityColumnInfo
            {
                ColumnName = GetName(propertyInfo),
                IsKey = GetIsKey(propertyInfo),
                IsIdentity = GetIsIdentity(propertyInfo)
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
            foreach ( var nameGetter in ColumnNameGetters )
            {
                var name = nameGetter(propertyInfo);
                if ( !String.IsNullOrEmpty(name) )
                {
                    return name;
                }
            }
            return null;
        }
        
        #endregion

    }
}