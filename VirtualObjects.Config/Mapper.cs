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
        public IEnumerable<Func<Type, String>> EntityNameGetters { get; set; }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            return new EntityInfo
            {
                EntityName = GetName(entityType),
                Columns = MapColumns(entityType.GetProperties())
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
                ColumnName = GetName(propertyInfo)
            };
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