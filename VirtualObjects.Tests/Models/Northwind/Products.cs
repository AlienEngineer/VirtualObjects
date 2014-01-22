using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Products
    {
        [Db.Identity]
        public int ProductId { get; set; }

        public String ProductName { get; set; }

        [Db.Association("SupplierId", "SupplierId")]
        public virtual Suppliers Supplier { get; set; }

        [Db.Association("CategoryId", "CategoryId")]
        public virtual Categories Category { get; set; }

        public String QuantityPerUnit { get; set; }

        public Decimal UnitPrice { get; set; }

        public Int16 UnitsInStock { get; set; }

        public Int16 UnitsOnOrder { get; set; }

        public Int16 ReorderLevel { get; set; }

        public Boolean Discontinued { get; set; }
    }
}
