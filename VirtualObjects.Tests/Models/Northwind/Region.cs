using System;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Region
    {
        [Key(FieldName = "RegionID")]
        public int Id { get; set; }

        [Column(FieldName = "RegionDescription")]
        public String Description { get; set; }
    }
}
