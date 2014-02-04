using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class EmployeeTerritories
    {
        [Key("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Key("TerritoryID")]
        public virtual Territories Territories { get; set; }
    }
}
