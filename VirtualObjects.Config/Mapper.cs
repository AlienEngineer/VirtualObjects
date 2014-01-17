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
        public IEnumerable<Func<PropertyInfo, String>> NameFromPropertyGetters { get; set; }

        #region IMapper Members

        public IEntityInfo Map(Type entityType)
        {
            return new EntityInfo
            {
                Columns = MapColumns(entityType.GetProperties())
            };
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
            foreach ( var nameGetter in NameFromPropertyGetters )
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