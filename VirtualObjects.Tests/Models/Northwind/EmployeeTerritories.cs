using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Models.Northwind
{
    public class EmployeeTerritories
    {
        [Db.Key("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Db.Key("TerritoryID")]
        public virtual Territories Territories { get; set; }
    }
}
