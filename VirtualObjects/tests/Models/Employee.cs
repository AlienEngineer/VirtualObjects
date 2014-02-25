
using System;
using VirtualObjects.Mappings;

namespace VirtualObjects.tests.Models
{
	[Table(TableName="Employees")]
	public class Employee
	{
	
		[Identity]
		public String EmployeeID { get; set; }
		public String LastName { get; set; }
		public String FirstName { get; set; }
		public String Title { get; set; }
		public String TitleOfCourtesy { get; set; }
		public String BirthDate { get; set; }
		public String HireDate { get; set; }
		public String Address { get; set; }
		public String City { get; set; }
		public String Region { get; set; }
		public String PostalCode { get; set; }
		public String Country { get; set; }
		public String HomePhone { get; set; }
		public String Extension { get; set; }
		public String Photo { get; set; }
		public String Notes { get; set; }
		[Association(FieldName="ReportsTo", OtherKey = "ReportsTo")]
		public virtual Employee ReportsTo { get; set; }
		public String PhotoPath { get; set; }
		public String Version { get; set; }
    

	}	
}