using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{

    [Table(TableName = "Employees")]
    public class Employee
    {
        [Identity]
        public int EmployeeId { get; set; }

        public String LastName { get; set; }

        public String FirstName { get; set; }

        public String Title { get; set; }

        public String TitleOfCourtesy { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        public String Address { get; set; }

        public String City { get; set; }

        public String Region { get; set; }

        public String PostalCode { get; set; }

        public String Country { get; set; }

        public String HomePhone { get; set; }

        public String Extension { get; set; }

        public String Notes { get; set; }

        public Byte[] Photo { get; set; }

        [Association(FieldName = "ReportsTo", OtherKey = "EmployeeId")]
        public virtual Employee ReportsTo { get; set; }

        public String PhotoPath { get; set; }

        public virtual IQueryable<EmployeeTerritories> Territories { get; set; }

        public virtual IQueryable<EmployeeTerritoriesSimplified> TerritoriesSimplified { get; set; }

        [FilterWith(FieldName = "EmployeeId")]
        public virtual ICollection<EmployeeTerritories> TerritoriesCollection { get; set; }

        [Ignore]
        public bool NonExistingField { get; set; }

        public Byte[] Version { get; set; }
    }

}
