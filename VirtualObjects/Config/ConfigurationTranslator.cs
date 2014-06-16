using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Config
{
    class ConfigurationTranslator : IConfigurationTranslator
    {
        public IList<Func<PropertyInfo, Boolean>> ColumnIgnoreGetters { get; set; }
        public IList<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public IList<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public IList<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public IList<Func<Type, String>> EntityNameGetters { get; set; }
        public IList<Func<Type, String>> EntitySchemaGetters { get; set; }
        public IList<Func<PropertyInfo, String>> ColumnForeignKeyGetters { get; set; }
        public IList<Func<PropertyInfo, String>> ColumnForeignKeyLinksGetters { get; set; }
        public IList<Func<PropertyInfo, Boolean>> ColumnVersionFieldGetters { get; set; }
        public IList<Func<PropertyInfo, Boolean>> ComputedColumnGetters { get; set; }
        public IList<Func<PropertyInfo, String>> CollectionFilterGetters { get; set; }
    }
}
