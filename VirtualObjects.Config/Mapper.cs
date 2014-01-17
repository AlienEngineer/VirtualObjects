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
            IEnumerable<IEntityColumnInfo> columns = MapColumns(entityType.GetProperties());
        }
  
        #endregion


        private IEnumerable<IEntityColumnInfo> MapColumns(PropertyInfo[] properties)
        {
            return properties.Select(propertyInfo => MapColumn(propertyInfo));
        }
  
        /// <summary>
        /// Maps the column.
        /// </summary>
        /// <param name="e">The e.</param>
        private IEntityColumnInfo MapColumn(PropertyInfo propertyInfo)
        {
            string name = GetName(propertyInfo);
        }
  
        private string GetName(PropertyInfo propertyInfo)
        {
            foreach (var nameGetter in NameFromPropertyGetters)
            {
                var name = nameGetter(propertyInfo);
                if (!String.IsNullOrEmpty(name))
                {
                    return name;
                }
            }
            return null;
        }


    }
}