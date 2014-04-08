



using System;
namespace VirtualObjects.Models
{
	#pragma warning disable 1591
	[Table(TableName="Customers")]
	public class Customer
	{

		[Key]
		public String CustomerID { get; set; }
		public String CompanyName { get; set; }
		public String ContactName { get; set; }
		public String ContactTitle { get; set; }
		public String Address { get; set; }
		public String City { get; set; }
		public String Region { get; set; }
		public String PostalCode { get; set; }
		public String Country { get; set; }
		public String Phone { get; set; }
		public String Fax { get; set; }    

	}	
	#pragma warning restore 1591
}