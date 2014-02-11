using System;
using VirtualObjects.Config;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Categories
    {
        [Identity]
        public int CategoryId { get; set; }

        public String CategoryName { get; set; }

        public String Description { get; set; }
    }

    public class CategoriesBadName
    {
        [Identity]
        public int CategoryId { get; set; }

        public String CategoryName { get; set; }

        public String Description { get; set; }
    }
}
