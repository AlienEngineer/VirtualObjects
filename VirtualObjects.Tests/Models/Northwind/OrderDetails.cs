using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    [Db.Table("Order Details")]
    public class OrderDetails
    {
        [Db.Key("OrderId")]
        [Db.Association("OrderId", "OrderId")]
        public virtual Orders Order { get; set; }

        [Db.Key("ProductId")]
        [Db.Association("ProductId", "ProductId")]
        public virtual Products Product { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 Quantity { get; set; }

        public Single Discount { get; set; }
    }

    [Db.Table("Order Details")]
    public class OrderDetailsSimplified
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 Quantity { get; set; }

        public Single Discount { get; set; }
    }
}
