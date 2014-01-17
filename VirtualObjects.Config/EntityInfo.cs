using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Config
{
    class EntityInfo : IEntityInfo
    {
        private IEnumerable<IEntityColumnInfo> columns;

        public string EntityName { get; set; }

        public IEnumerable<IEntityColumnInfo> Columns
        {
            get { return this.columns; }
            set
            {
                this.columns = value;
                columnsDictionary = value.ToDictionary(e => e.ColumnName);
            }
        }
        
        public IEnumerable<IEntityColumnInfo> KeyColumns { get; set; }

        private IDictionary<string, IEntityColumnInfo> columnsDictionary;

        public IEntityColumnInfo this[string columnName]
        {
            get
            {
                IEntityColumnInfo column;
                return columnsDictionary.TryGetValue(columnName, out column) ? column : null;
            }
        }

    }
}