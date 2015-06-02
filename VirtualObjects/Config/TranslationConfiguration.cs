using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace VirtualObjects.Config
{
    class TranslationConfiguration : ITranslationConfiguration
    {
        public IList<Func<PropertyInfo, bool>> ColumnIgnoreGetters { get; set; }
        public IList<Func<PropertyInfo, string>> ColumnNameGetters { get; set; }
        public IList<Func<PropertyInfo, string>> ColumnFormattersGetters { get; set; }
        public IList<Func<PropertyInfo, NumberFormatInfo>> ColumnNumberFormattersGetters { get; set; }
        public IList<Func<PropertyInfo, bool>> ColumnKeyGetters { get; set; }
        public IList<Func<PropertyInfo, bool>> ColumnIdentityGetters { get; set; }
        public IList<Func<Type, string>> EntityNameGetters { get; set; }
        public IList<Func<PropertyInfo, string>> ColumnForeignKeyGetters { get; set; }
        public IList<Func<PropertyInfo, string>> ColumnForeignKeyLinksGetters { get; set; }
        public IList<Func<PropertyInfo, bool>> ColumnVersionFieldGetters { get; set; }
        public IList<Func<PropertyInfo, bool>> ComputedColumnGetters { get; set; }
        public IList<Func<PropertyInfo, string>> CollectionFilterGetters { get; set; }
        public IList<Func<PropertyInfo, bool>> IsForeignKeyGetters { get; set; }
    }
}
