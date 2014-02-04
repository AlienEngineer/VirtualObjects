using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerDemographics
    {
        [Key]
        public String CustomerTypeId { get; set; }

        public String CustomerDesc { get; set; }
    }
}
