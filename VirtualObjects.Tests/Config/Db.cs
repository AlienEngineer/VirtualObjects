using System;
namespace VirtualObjects.Tests.Config
{
    public static class Db
    {
        public class ColumnAttribute : Attribute
        {
            public ColumnAttribute(String name)
            {
                this.Name = name;
            }

            public String Name { get; private set; }
        }

        public class TableAttribute : Attribute
        {
            public TableAttribute(String name)
            {
                this.Name = name;
            }

            public String Name { get; private set; }
        }
    }
    
}