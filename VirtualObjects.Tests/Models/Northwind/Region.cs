using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class Region
    {
        [Key(FieldName = "RegionID")]
        public int Id { get; set; }

        [Column(FieldName = "RegionDescription")]
        public string Description { get; set; }
    }
}
