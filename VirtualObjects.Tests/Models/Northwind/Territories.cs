using System;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Territories
    {
        [Key]
        public String TerritoryId { get; set; }

        public String TerritoryDescription { get; set; }

        [Column("RegionId")]
        public virtual Region Region { get; set; }
    }
}
