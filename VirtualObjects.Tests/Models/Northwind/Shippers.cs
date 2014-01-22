using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Shippers
    {
        [Db.Identity]
        public int ShipperId { get; set; }

        public String CompanyName { get; set; }

        public String Phone { get; set; }
    }
}
