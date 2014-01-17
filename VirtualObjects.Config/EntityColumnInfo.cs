namespace VirtualObjects.Config
{
    class EntityColumnInfo : IEntityColumnInfo
    {
        public string ColumnName { get; set; }

        public bool IsKey { get; set; }

        public bool IsIdentity { get; set; }

    }
}