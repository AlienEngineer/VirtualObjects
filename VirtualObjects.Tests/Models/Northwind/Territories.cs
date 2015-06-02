using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Territories
    {
        [Key]
        public string TerritoryId { get; set; }

        public string TerritoryDescription { get; set; }

        [Column(FieldName = "RegionId")]
        public virtual Region Region { get; set; }
    }
}
