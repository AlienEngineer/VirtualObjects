﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Compilation;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;

    /// <summary>
    /// 
    /// Unit-tests for query building
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Query Building")]
    public class SqlTranslationTests : UtilityBelt
    {

        private static IQueryable<TEntity> Query<TEntity>()
        {
            return new List<TEntity>().AsQueryable();
        }

        private String Translate(IQueryable query)
        {

            var str =  Diagnostic.Timed(
                func: () => new QueryTranslator(new SqlFormatter(), Mapper).TranslateQuery(query).CommandText, 
                name: "Translation");

            Trace.WriteLine(str);

            return str;
        }


        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Query()
        {
            var query = Query<Employee>();

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0]")
            );
        }


        /// <summary>
        /// 
        /// Sql translation get the 10 first rows.
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Query_Top_N()
        {
            var query = Query<Employee>().Take(10);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select TOP 10 [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0]")
            );
        }

        /// <summary>
        /// 
        /// Sql translation skip the 10 first rows.
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Query_Skip_N()
        {
            var query = Query<Employee>().Skip(1);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From (Select ROW_NUMBER() OVER ( Order By [T100].[EmployeeId]) as [Internal_Row_Index], * From [Employees] [T100]) [T0] Where ([T0].[Internal_Row_Index] > 1)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation skip the 1 and take 1 row.
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Query_Skip_Take_N()
        {
            var query = Query<Employee>().Take(1).Skip(1);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From (Select ROW_NUMBER() OVER ( Order By [T100].[EmployeeId]) as [Internal_Row_Index], * From [Employees] [T100]) [T0] Where ([T0].[Internal_Row_Index] > 1 And [T0].[Internal_Row_Index] <= 2)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation skip the 1 and take 1 row predicated.
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Predicated_Query_Skip_Take_N()
        {
            var query = Query<Employee>()
                .Where(e => e.FirstName == "")
                .Take(1).Skip(1).Where(e => e.FirstName == "");

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From (Select ROW_NUMBER() OVER ( Order By [T100].[EmployeeId]) as [Internal_Row_Index], * From [Employees] [T100] Where ([T100].[FirstName] = @p0) And ([T100].[FirstName] = @p1)) [T0] Where ([T0].[FirstName] = @p0) And ([T0].[FirstName] = @p1) And ([T0].[Internal_Row_Index] > 1 And [T0].[Internal_Row_Index] <= 2)")
            );
        }


        /// <summary>
        /// 
        /// Sql translation for a simple nested query
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_NestedQuery()
        {
            var query = Query<Orders>()
                .Where(o => Query<OrderDetails>().Select(od => od.UnitPrice).Contains(o.Freight));

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry] From [Orders] [T0] Where ([T0].[Freight] In (Select [T1].[UnitPrice] From [Order Details] [T1]))")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple nested query
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_NestedQuery_Predicated()
        {
            var query = Query<Orders>()
                .Where(o => Query<OrderDetails>().Where(e => e.Quantity > 0).Select(od => od.UnitPrice).Contains(o.Freight));

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry] From [Orders] [T0] Where ([T0].[Freight] In (Select [T1].[UnitPrice] From [Order Details] [T1] Where ([T1].[Quantity] > @p0)))")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple nested query
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_NestedQuery_Predicated1()
        {
            var query = Query<Orders>()
                .Where(o =>
                    Query<OrderDetails>().Where(e => e.Order == o)
                    .Select(od => od.UnitPrice)
                    .Contains(o.Freight)
                );

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry] From [Orders] [T0] Where ([T0].[Freight] In (Select [T1].[UnitPrice] From [Order Details] [T1] Where ([T1].[OrderId] = [T0].[OrderId])))")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple nested query
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_NestedQuery_Predicated2()
        {
            var query = Query<Orders>()
                .Where(o =>
                    Query<OrderDetails>().Where(e => e.Order.OrderId == o.OrderId)
                    .Select(od => od.UnitPrice)
                    .Contains(o.Freight)
                );

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry] From [Orders] [T0] Where ([T0].[Freight] In (Select [T1].[UnitPrice] From [Order Details] [T1] Where ([T1].[OrderId] In (Select [T2].[OrderId] From [Orders] [T2] Where [T2].[OrderId] = [T0].[OrderId]))))")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Predicate()
        {
            var query = Query<Employee>().Where(e => e.EmployeeId == 1);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[EmployeeId] = @p0)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Member_Predicate()
        {
            var query = Query<Employee>().Where(e => e.ReportsTo.EmployeeId == 1);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[ReportsTo] In (Select [T1].[EmployeeId] From [Employees] [T1] Where [T1].[EmployeeId] = @p0))")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Dated_Predicate()
        {
            var query = Query<Employee>().Where(e => e.BirthDate == new DateTime(DateTime.Now.Ticks));

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[BirthDate] = @p0)")
            );

            query = Query<Employee>().Where(e => e.BirthDate == new DateTime());

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[BirthDate] = @p0)")
            );

            query = Query<Employee>().Where(e => e.BirthDate == DateTime.Now);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[BirthDate] = GetDate())")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Members_Predicate()
        {
            var query = Query<Employee>().Where(e => e.LastName == e.City);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[LastName] = [T0].[City])")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a multiple predicate where clauses
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_MultipleCalls_Predicate()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName == e.City || e.EmployeeId == 1)
                .Where(e => e.Extension == "351");

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (([T0].[LastName] = [T0].[City]) Or ([T0].[EmployeeId] = @p0)) And ([T0].[Extension] = @p1)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a multiple predicate where clauses
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_MultipleCalls_Predicate1()
        {
            var query = Query<Employee>()
                .Where(e => e.Extension == "351")
                .Where(e => e.LastName == e.City || e.EmployeeId == 1)
                .Where(e => e.Extension == "351")
                .Where(e => e.LastName == e.City || e.EmployeeId == 1)
                .Where(e => e.Extension == "351")
                .Where(e => e.Extension == "351");

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[Extension] = @p0) And (([T0].[LastName] = [T0].[City]) Or ([T0].[EmployeeId] = @p1)) And ([T0].[Extension] = @p2) And (([T0].[LastName] = [T0].[City]) Or ([T0].[EmployeeId] = @p3)) And ([T0].[Extension] = @p4) And ([T0].[Extension] = @p5)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Boolean_Predicate()
        {
            var query = Query<Products>().Where(e => e.Discontinued);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[ProductId], [T0].[ProductName], [T0].[SupplierId], [T0].[CategoryId], [T0].[QuantityPerUnit], [T0].[UnitPrice], [T0].[UnitsInStock], [T0].[UnitsOnOrder], [T0].[ReorderLevel], [T0].[Discontinued] From [Products] [T0] Where ([T0].[Discontinued] = @p0)")
            );

            query = Query<Products>().Where(e => !e.Discontinued);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[ProductId], [T0].[ProductName], [T0].[SupplierId], [T0].[CategoryId], [T0].[QuantityPerUnit], [T0].[UnitPrice], [T0].[UnitsInStock], [T0].[UnitsOnOrder], [T0].[ReorderLevel], [T0].[Discontinued] From [Products] [T0] Where ([T0].[Discontinued] != @p0)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(10)]
        public void SqlTranslation_Simple_Join()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        select new { Order = o, Detail = od };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Inner Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])")
            );
        }


        [Test]
        public void SqlTranslation_Joins_Predicated()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        where o.Freight > od.UnitPrice
                        select new { Order = o, Detail = od };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Inner Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Where ([T0].[Freight] > [T1].[UnitPrice])")
            );
        }

        [Test]
        public void SqlTranslation_2Joins_Predicated()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        where e.EmployeeId == o.Employee.EmployeeId
                        select new { Order = o, Detail = od, e };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount], [T2].[EmployeeId], [T2].[LastName], [T2].[FirstName], [T2].[Title], [T2].[TitleOfCourtesy], [T2].[BirthDate], [T2].[HireDate], [T2].[Address], [T2].[City], [T2].[Region], [T2].[PostalCode], [T2].[Country], [T2].[HomePhone], [T2].[Extension], [T2].[Notes], [T2].[Photo], [T2].[ReportsTo], [T2].[PhotoPath], [T2].[Version] From [Orders] [T0] Inner Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Inner Join [Employees] [T2] On ([T0].[EmployeeId] = [T2].[EmployeeId]) Where ([T0].[EmployeeId] = [T2].[EmployeeId])")
            );
        }

        [Test]
        public void SqlTranslation_Translate_GroupedJoins()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order into ods
                        select new { Order = o, Details = ods };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Inner Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])")
            );
        }
    }
}
