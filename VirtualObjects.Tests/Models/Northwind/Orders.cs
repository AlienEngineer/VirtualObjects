using System;
using System.Collections.Generic;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    [Table(TableName = "Orders")]
    public class OrderSimplified
    {
        [Identity]
        public int OrderId { get; set; }

        public int EmployeeId { get; set; }

        public string ShipName { get; set; }
        
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ShippedDate { get; set; }
    }

    public class Orders
    {
        [Identity]
        public int OrderId { get; set; }

        [Association(FieldName = "CustomerId", OtherKey = "CustomerId")]
        public virtual Customers Customer { get; set; }

        [Association(FieldName = "EmployeeId", OtherKey = "EmployeeId")]
        public virtual Employee Employee { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ShippedDate { get; set; }

        [Association(FieldName = "ShipVia", OtherKey = "ShipperId")]
        public virtual Shippers Shipper { get; set; }

        public decimal Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public virtual IEnumerable<OrderDetails> Details { get; set; }
    }

}
