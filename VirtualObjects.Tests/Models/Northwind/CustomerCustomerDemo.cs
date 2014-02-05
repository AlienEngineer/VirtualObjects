
namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerCustomerDemo
    {
        [Key(FieldName = "CustomerId")]
        public virtual Customers Customer { get; set; }

        [Key(FieldName = "CustomerTypeId")]
        public virtual CustomerDemographics Demographics { get; set; }
    }
}
