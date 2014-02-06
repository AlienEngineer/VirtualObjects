using System;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    public static class EntitiesAsserts
    {
        public static void Assert_Employee_1(Employee employee)
        {
            Assert.AreEqual(1, employee.EmployeeId);
            Assert.AreEqual("Nancy", employee.FirstName);
            Assert.AreEqual("Davolio", employee.LastName);
            Assert.AreEqual("Sales Representative", employee.Title);
            Assert.AreEqual("Ms.", employee.TitleOfCourtesy);
            Assert.AreEqual(new DateTime(1948, 12, 8), employee.BirthDate);
            Assert.AreEqual(new DateTime(1992, 5, 1), employee.HireDate);
            Assert.AreEqual("507 - 20th Ave. E.\r\nApt. 2A", employee.Address);
            Assert.AreEqual("Seattle", employee.City);
            Assert.AreEqual("WA", employee.Region);
            Assert.AreEqual("98122", employee.PostalCode);
            Assert.AreEqual("USA", employee.Country);
            Assert.AreEqual("(206) 555-9857", employee.HomePhone);
            Assert.AreEqual("5467", employee.Extension);
            Assert.AreEqual(21626, employee.Photo.Count());
            Assert.AreEqual(
                "Education includes a BA in psychology from Colorado State University in 1970.  She also completed \"The Art of the Cold Call.\"  Nancy is a member of Toastmasters International.",
                employee.Notes);
            Assert.AreEqual("http://accweb/emmployees/davolio.bmp", employee.PhotoPath);
            Assert.IsNotNull(employee.ReportsTo);
            Assert.AreEqual(2, employee.ReportsTo.EmployeeId);
        }

        public static void Assert_Employee_2(Employee employee)
        {
            Assert.AreEqual(2, employee.EmployeeId);
            Assert.AreEqual("Andrew", employee.FirstName);
            Assert.AreEqual("Fuller", employee.LastName);
            Assert.AreEqual("Vice President, Sales", employee.Title);
            Assert.AreEqual("Dr.", employee.TitleOfCourtesy);
            Assert.AreEqual(new DateTime(1952, 2, 19), employee.BirthDate);
            Assert.AreEqual(new DateTime(1992, 8, 14), employee.HireDate);
            Assert.AreEqual("908 W. Capital Way", employee.Address);
            Assert.AreEqual("Tacoma", employee.City);
            Assert.AreEqual("WA", employee.Region);
            Assert.AreEqual("98401", employee.PostalCode);
            Assert.AreEqual("USA", employee.Country);
            Assert.AreEqual("(206) 555-9482", employee.HomePhone);
            Assert.AreEqual("3457", employee.Extension);
            Assert.AreEqual(21626, employee.Photo.Count());
            Assert.AreEqual(
                "Andrew received his BTS commercial in 1974 and a Ph.D. in international marketing from the University of Dallas in 1981.  He is fluent in French and Italian and reads German.  He joined the company as a sales representative, was promoted to sales manager in January 1992 and to vice president of sales in March 1993.  Andrew is a member of the Sales Management Roundtable, the Seattle Chamber of Commerce, and the Pacific Rim Importers Association.",
                employee.Notes);
            Assert.AreEqual("http://accweb/emmployees/fuller.bmp", employee.PhotoPath);
            Assert.IsNotNull(employee.ReportsTo);
            Assert_Employee_1(employee.ReportsTo);
        }

        public static void Assert_Employee_3(Employee employee)
        {
            Assert.AreEqual(3, employee.EmployeeId);
            Assert.AreEqual("Janet", employee.FirstName);
            Assert.AreEqual("Leverling", employee.LastName);
            Assert.AreEqual("Sales Representative", employee.Title);
            Assert.AreEqual("Ms.", employee.TitleOfCourtesy);
            Assert.AreEqual(new DateTime(1963, 08, 30), employee.BirthDate);
            Assert.AreEqual(new DateTime(1992, 4, 1), employee.HireDate);
            Assert.AreEqual("722 Moss Bay Blvd.", employee.Address);
            Assert.AreEqual("Kirkland", employee.City);
            Assert.AreEqual("WA", employee.Region);
            Assert.AreEqual("98033", employee.PostalCode);
            Assert.AreEqual("USA", employee.Country);
            Assert.AreEqual("(206) 555-3412", employee.HomePhone);
            Assert.AreEqual("3355", employee.Extension);
            Assert.AreEqual(21722, employee.Photo.Count());
            Assert.AreEqual(
                "Janet has a BS degree in chemistry from Boston College (1984).  She has also completed a certificate program in food retailing management.  Janet was hired as a sales associate in 1991 and promoted to sales representative in February 1992.",
                employee.Notes);
            Assert.AreEqual("http://accweb/emmployees/leverling.bmp", employee.PhotoPath);
            Assert.IsNotNull(employee.ReportsTo);
        }
    }
}