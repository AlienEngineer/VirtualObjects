using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerCustomerDemo
    {
        [Db.Key("CustomerId")]
        public virtual Customers Customer { get; set; }

        [Db.Key("CustomerTypeId")]
        public virtual CustomerDemographics Demographics { get; set; }
    }
}
