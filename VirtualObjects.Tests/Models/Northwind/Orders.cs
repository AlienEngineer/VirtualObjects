using System;
using System.Collections.Generic;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Orders
    {
        [Identity]
        public int OrderId { get; set; }

        [Association("CustomerId", "CustomerId")]
        public virtual Customers Customer { get; set; }

        [Association("EmployeeId", "EmployeeId")]
        public virtual Employee Employee { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ShippedDate { get; set; }

        [Association("ShipVia", "ShipperId")]
        public virtual Shippers Shipper { get; set; }

        public Decimal Freight { get; set; }

        public String ShipName { get; set; }

        public String ShipAddress { get; set; }

        public String ShipCity { get; set; }

        public String ShipRegion { get; set; }

        public String ShipPostalCode { get; set; }

        public String ShipCountry { get; set; }

        public virtual IEnumerable<OrderDetails> Details { get; set; }
    }
}
