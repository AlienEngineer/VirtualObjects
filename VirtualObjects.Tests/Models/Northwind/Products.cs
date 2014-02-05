using System;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Products
    {
        [Identity]
        public int ProductId { get; set; }

        public String ProductName { get; set; }

        [Association(FieldName = "SupplierId", OtherKey = "SupplierId")]
        public virtual Suppliers Supplier { get; set; }

        [Association(FieldName = "CategoryId", OtherKey = "CategoryId")]
        public virtual Categories Category { get; set; }

        public String QuantityPerUnit { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 UnitsInStock { get; set; }

        public Int16 UnitsOnOrder { get; set; }

        public Int16 ReorderLevel { get; set; }

        public Boolean Discontinued { get; set; }
    }
}
