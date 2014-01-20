using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Config
{
    class EntityInfo : IEntityInfo
    {
        private IEnumerable<IEntityColumnInfo> _columns;
        private IDictionary<string, IEntityColumnInfo> _columnsDictionary;

        public string EntityName { get; set; }

        public IEnumerable<IEntityColumnInfo> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                _columnsDictionary = value.ToDictionary(e => e.Property.Name);
            }
        }
        
        public IEnumerable<IEntityColumnInfo> KeyColumns { get; set; }
        public Type EntityType { get; set; }


        public IEntityColumnInfo this[string propertyName]
        {
            get
            {
                IEntityColumnInfo column;
                return _columnsDictionary.TryGetValue(propertyName, out column) ? column : null;
            }
        }

    }
}