using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Shippers
    {
        [Identity]
        public int ShipperId { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }
    }
}
