using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerCustomerDemo
    {
        [Key("CustomerId")]
        public virtual Customers Customer { get; set; }

        [Key("CustomerTypeId")]
        public virtual CustomerDemographics Demographics { get; set; }
    }
}
