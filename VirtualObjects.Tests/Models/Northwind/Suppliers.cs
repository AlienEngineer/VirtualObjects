﻿using System;
using VirtualObjects.Config;
using VirtualObjects.Mappings;


namespace VirtualObjects.Tests.Models.Northwind
{
    public class Suppliers
    {
        [Identity]
        public int SupplierId { get; set; }

        public String CompanyName { get; set; }

        public String ContactName { get; set; }

        public String ContactTitle { get; set; }

        public String Address { get; set; }

        public String City { get; set; }

        public String Region { get; set; }

        public String PostalCode { get; set; }

        public String Country { get; set; }

        public String Phone { get; set; }

        public String Fax { get; set; }

        public String HomePage { get; set; }
    }
}
