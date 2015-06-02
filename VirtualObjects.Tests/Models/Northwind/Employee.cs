using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{

    [Table(TableName = "Employees")]
    public class EmployeeSimple
    {
        [Identity]
        public int EmployeeId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime HireDate { get; set; }

        [ForeignKey]
        public int ReportsTo { get; set; }
    }

    [Table(TableName = "Employees")]
    public class Employee
    {
        [Identity]
        public int EmployeeId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        public string TitleOfCourtesy { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string HomePhone { get; set; }

        public string Extension { get; set; }

        public string Notes { get; set; }

        public byte[] Photo { get; set; }

        [Association(FieldName = "ReportsTo", OtherKey = "EmployeeId")]
        public virtual Employee ReportsTo { get; set; }

        public string PhotoPath { get; set; }

        public virtual IQueryable<EmployeeTerritories> Territories { get; set; }

        public virtual IQueryable<EmployeeTerritoriesSimplified> TerritoriesSimplified { get; set; }

        [FilterWith(FieldName = "EmployeeId")]
        public virtual ICollection<EmployeeTerritories> TerritoriesCollection { get; set; }

        [Ignore]
        public bool NonExistingField { get; set; }

        public byte[] Version { get; set; }
    }

}
