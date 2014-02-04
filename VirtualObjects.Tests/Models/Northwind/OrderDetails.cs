using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    [Table("Order Details")]
    public class OrderDetails
    {
        [Key("OrderId")]
        [Association("OrderId", "OrderId")]
        public virtual Orders Order { get; set; }

        [Key("ProductId")]
        [Association("ProductId", "ProductId")]
        public virtual Products Product { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 Quantity { get; set; }

        public Single Discount { get; set; }
    }

    [Table("Order Details")]
    public class OrderDetailsSimplified
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 Quantity { get; set; }

        public Single Discount { get; set; }
    }
}
