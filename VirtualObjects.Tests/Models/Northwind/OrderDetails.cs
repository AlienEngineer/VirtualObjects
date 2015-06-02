using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    [Table(TableName = "Order Details")]
    public class OrderDetailsSimplified
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }
    }

    [Table(TableName = "Order Details")]
    public class OrderDetails
    {
        [Key(FieldName = "OrderId")]
        [Association(OtherKey = "OrderId")]
        public virtual Orders Order { get; set; }

        [Key(FieldName = "ProductId")]
        [Association(OtherKey = "ProductId")]
        public virtual Products Product { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }
    }

}
