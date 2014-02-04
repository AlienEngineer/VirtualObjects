using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Region
    {
        [Key("RegionID")]
        public int Id { get; set; }

        [Column("RegionDescription")]
        public String Description { get; set; }
    }
}
