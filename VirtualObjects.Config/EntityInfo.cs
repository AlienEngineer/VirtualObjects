using System.Collections.Generic;

namespace VirtualObjects.Config
{
    class EntityInfo : IEntityInfo
    {
        public string EntityName { get; set; }

        public IEnumerable<IEntityColumnInfo> Columns { get; set; }
        
        public IEnumerable<IEntityColumnInfo> KeyColumns { get; set; }
    }
}