using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Categories
    {
        [Identity]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }
    }

    public class CategoriesBadName
    {
        [Identity]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}
