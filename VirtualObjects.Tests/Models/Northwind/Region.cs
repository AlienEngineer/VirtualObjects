using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Region
    {
        [Db.Key("RegionID")]
        public int Id { get; set; }

        [Db.Column("RegionDescription")]
        public String Description { get; set; }
    }
}
