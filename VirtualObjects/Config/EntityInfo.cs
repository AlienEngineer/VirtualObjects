using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Queries;

namespace VirtualObjects.Config
{
    class EntityInfo : IEntityInfo
    {
        private IList<IEntityColumnInfo> _columns;
        private IDictionary<string, IEntityColumnInfo> _columnsDictionary;

        

        public string EntityName { get; set; }

        public IList<IEntityColumnInfo> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                _columnsDictionary = value.ToDictionary(e => e.Property.Name);
            }
        }

        public IList<IEntityColumnInfo> KeyColumns { get; set; }
        public IEntityColumnInfo Identity { get; set; }
        public Action<object, object[]> MapEntity { get; set; }
        public Func<object> EntityFactory { get; set; }
        public IEntityColumnInfo VersionControl { get; set; }

        public Type EntityType { get; set; }
        public Func<Object, int> KeyHashCode { get; set; }

        public int GetKeyHashCode(Object obj)
        {
            return KeyHashCode(obj);
        }

        public IOperations Operations { get; set; }
        public IList<IEntityColumnInfo> ForeignKeys { get; set; }
        public IEntityProvider EntityProvider { get; set; }
        public IEntityMapper EntityMapper { get; set; }

        public IEntityColumnInfo GetFieldAssociatedWith(string name)
        {
            return Columns
                .Where(e => e.ForeignKey != null)
                .FirstOrDefault(e => e.ForeignKey.ColumnName == name);
        }


        public IEntityColumnInfo this[string propertyName]
        {
            get
            {
                IEntityColumnInfo column;
                return _columnsDictionary.TryGetValue(propertyName, out column) ? column : null;
            }
        }

        public override string ToString()
        {
            if (Columns == null)
            {
                return EntityName + " => no columns";
            }
#if NET35
            return EntityName + " = {\n" +
                   String.Join(",\n", Columns.Select(e => e.ColumnName).ToArray()) +
                   "} ";
#else
            return EntityName + " = {\n" +
                   String.Join(",\n", Columns.Select(e => e.ColumnName)) +
                   "} ";
#endif
        }
    }
}