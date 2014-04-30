
using VirtualObjects.Config;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class EmployeeTerritories
    {
        [Key(FieldName = "EmployeeId")]
        [Association(OtherKey = "EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Key(FieldName = "TerritoryID")]
        [Association(OtherKey = "TerritoryId")]
        public virtual Territories Territories { get; set; }
    }

    [Table(TableName = "EmployeeTerritories")]
    public class EmployeeTerritoriesSimplified
    {
        [Key]
        public int EmployeeId { get; set; }

        [Key]
        public int TerritoryID { get; set; }
    }
}
