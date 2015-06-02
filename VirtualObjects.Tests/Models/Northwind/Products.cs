using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Products
    {
        [Identity]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        [Association(FieldName = "SupplierId", OtherKey = "SupplierId")]
        public virtual Suppliers Supplier { get; set; }

        [Association(FieldName = "CategoryId", OtherKey = "CategoryId")]
        public virtual Categories Category { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal UnitPrice { get; set; }

        public short UnitsInStock { get; set; }

        public short UnitsOnOrder { get; set; }

        public short ReorderLevel { get; set; }

        public bool Discontinued { get; set; }
    }
}
