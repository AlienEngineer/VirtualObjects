
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
}
