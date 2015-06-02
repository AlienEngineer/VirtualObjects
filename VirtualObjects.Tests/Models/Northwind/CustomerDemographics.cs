using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerDemographics
    {
        [Key]
        public string CustomerTypeId { get; set; }

        public string CustomerDesc { get; set; }
    }
}
