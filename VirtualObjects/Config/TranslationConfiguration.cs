using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Config
{
    class TranslationConfiguration : ITranslationConfiguration
    {
        public ICollection<Func<PropertyInfo, Boolean>> ColumnIgnoreGetters { get; set; }
        public ICollection<Func<PropertyInfo, String>> ColumnNameGetters { get; set; }
        public ICollection<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; set; }
        public ICollection<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; set; }
        public ICollection<Func<Type, String>> EntityNameGetters { get; set; }
        public ICollection<Func<PropertyInfo, String>> ColumnForeignKeyGetters { get; set; }
        public ICollection<Func<PropertyInfo, String>> ColumnForeignKeyLinksGetters { get; set; }
        public ICollection<Func<PropertyInfo, Boolean>> ColumnVersionFieldGetters { get; set; }
        public ICollection<Func<PropertyInfo, Boolean>> ComputedColumnGetters { get; set; }
    }
}
