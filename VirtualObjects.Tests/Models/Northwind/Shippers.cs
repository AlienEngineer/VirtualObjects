using System;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Shippers
    {
        [Identity]
        public int ShipperId { get; set; }

        public String CompanyName { get; set; }

        public String Phone { get; set; }
    }
}
