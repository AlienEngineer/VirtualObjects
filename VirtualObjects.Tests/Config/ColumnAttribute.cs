using System;
namespace VirtualObjects.Tests.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(String name)
        {
            this.Name = name;
        }

        public String Name { get; private set; }
    }
}