using System;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    [Table(TableName = "Order Details")]
    public class OrderDetails
    {
        [Key(FieldName = "OrderId")]
        [Association(OtherKey = "OrderId")]
        public virtual Orders Order { get; set; }

        [Key(FieldName = "ProductId")]
        [Association(OtherKey = "ProductId")]
        public virtual Products Product { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 Quantity { get; set; }

        public Single Discount { get; set; }
    }

}
