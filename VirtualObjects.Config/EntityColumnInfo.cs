using System.Reflection;

namespace VirtualObjects.Config
{
    class EntityColumnInfo : IEntityColumnInfo
    {
        public string ColumnName { get; set; }

        public bool IsKey { get; set; }

        public bool IsIdentity { get; set; }

        public PropertyInfo Property { get; set; }
        
        public IEntityInfo EntityInfo { get; set; }
        
        public IEntityColumnInfo ForeignKey { get; set; }
    }
}