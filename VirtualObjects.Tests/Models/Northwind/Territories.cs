using System;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Territories
    {
        [Key]
        public String TerritoryId { get; set; }

        public String TerritoryDescription { get; set; }

        [Column(FieldName = "RegionId")]
        public virtual Region Region { get; set; }
    }
}
