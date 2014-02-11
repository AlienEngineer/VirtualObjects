using System;
using VirtualObjects.Config;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class CustomerDemographics
    {
        [Key]
        public String CustomerTypeId { get; set; }

        public String CustomerDesc { get; set; }
    }
}
