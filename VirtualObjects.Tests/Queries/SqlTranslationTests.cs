using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Translation;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Queries
{
    class CachingTranslatorProvider : IQueryTranslatorProvider
    {
        public IQueryTranslator CreateQueryTranslator(IFormatter formatter, IMapper mapper, IEntityBag entityBag, SessionConfiguration configuration)
        {
            return new CachingTranslator(formatter, mapper, entityBag, configuration);
        }
    }

    class TranslatorProvider : IQueryTranslatorProvider
    {
        public IQueryTranslator CreateQueryTranslator(IFormatter formatter, IMapper mapper, IEntityBag entityBag, SessionConfiguration configuration)
        {
            return new QueryTranslator(formatter, mapper, entityBag, configuration);
        }
    }

    /// <summary>
    /// 
    /// Unit-tests for query building
    /// 
    /// Author: Sérgio
    /// </summary>
    [Category("Query Building")]
    public class SqlTranslationTests : UtilityBelt
    {

        private readonly IQueryTranslator _translator;

        public SqlTranslationTests()
        {
            _translator = Translator;
        }

        private string Translate(IQueryable query)
        {
            var str = Diagnostic.Timed(() => _translator.TranslateQuery(query).CommandText);

            Trace.WriteLine(str);

            return str;
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_Query()
        {
            var query = Query<Employee>();

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0]")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Projected_Query()
        {
            var query = Query<Employee>()
                .Select(e => new { e.EmployeeId, e.LastName, e.FirstName });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName] From [Employees] [T0]")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Projected_Query_Top_N()
        {
            var query = Query<Employee>()
                .Select(e => new { e.EmployeeId, e.LastName, e.FirstName }).Take(10);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select TOP 10 [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName] From [Employees] [T0]")
            );
        }
        /// <summary>
        /// 
        /// Sql translation skip the 1 and take 1 row predicated.
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Projected_Query_Skip_Take_N()
        {
            var query = Query<Employee>()
                .Select(e => new { e.EmployeeId, e.LastName, e.FirstName })
                .Take(1).Skip(1);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName] From (Select ROW_NUMBER() OVER ( Order By [T100].[EmployeeId]) as [Internal_Row_Index], * From [Employees] [T100]) [T0] Where ([T0].[Internal_Row_Index] > 1 And [T0].[Internal_Row_Index] <= 2)")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Projected_Query_Skip_N()
        {
            var query = Query<Employee>()
                .Select(e => new { e.EmployeeId, e.LastName, e.FirstName }).Skip(10);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName] From (Select ROW_NUMBER() OVER ( Order By [T100].[EmployeeId]) as [Internal_Row_Index], * From [Employees] [T100]) [T0] Where ([T0].[Internal_Row_Index] > 10)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation get the 10 first rows.
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Ordered_Query_Skip_Take_N()
        {
            var query = Query<Employee>()
                .Take(1).Skip(1).OrderBy(e => e.LastName);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From (Select ROW_NUMBER() OVER ( Order By [T100].[LastName]) as [Internal_Row_Index], * From [Employees] [T100]) [T0] Where ([T0].[Internal_Row_Index] > 1 And [T0].[Internal_Row_Index] <= 2)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a simple nested query
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        /// Sql translation with a custom function predicate
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_call_on_where_clause()
        {
            var query = Query<Employee>().Where(e => Test(e.EmployeeId) == true).Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (dbo.Test([T0].[EmployeeId]) = @p0)")
            );
        }


        /// <summary>
        /// 
        /// Sql translation with a custom function predicate
        /// 
        /// </summary>
        //[Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_call_unary_way_on_where_clause()
        {
            var query = Query<Employee>().Where(e => Test(e.EmployeeId)).Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (dbo.Test([T0].[EmployeeId]) = @p0)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation with a custom function order
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_order_clause()
        {
            var query = Query<Employee>().OrderBy(e => SomeCalculation(e.EmployeeId)).Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By dbo.SomeCalculation([T0].[EmployeeId])")
            );
        }

        /// <summary>
        /// 
        /// Sql translation with a custom function order
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_with_custom_function_as_argument_where_clause()
        {
            var query = Query<Employee>()
                .Where(e => SomeCalculation(SomeOtherCalculation(e.EmployeeId, e.City)) > 0)
                .Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (dbo.SomeCalculation(dbo.SomeOtherCalculation([T0].[EmployeeId], [T0].[City])) > @p0)")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_with_custom_function_as_argument_order_clause()
        {
            var query = Query<Employee>()
                .OrderBy(e => SomeCalculation(SomeOtherCalculation(e.EmployeeId, e.City)))
                .Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By dbo.SomeCalculation(dbo.SomeOtherCalculation([T0].[EmployeeId], [T0].[City]))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_string_contains_method()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Contains("r") || e.LastName.Contains("a"))
                .Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] like '%' + @p0 + '%' Or [T0].[LastName] like '%' + @p1 + '%')")
            );
        }

        private int SomeOtherCalculation(int employeeId, string city)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// Sql translation with a custom function order
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Query_with_custom_function_with_two_parameters_order_clause()
        {
            var query = Query<Employee>().OrderBy(e => SomeCalculation(e.EmployeeId, e.City)).Select(e => e.EmployeeId);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By dbo.SomeCalculation([T0].[EmployeeId], [T0].[City])")
            );
        }

        private static bool Test(int employee)
        {
            throw new NotImplementedException();
        }

        private static int SomeCalculation(int employee)
        {
            throw new NotImplementedException();
        }

        private static int SomeCalculation(int employee, string sadas)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Member_Predicate_Inversed()
        {
            var query = Query<Employee>().Where(e => 1 == e.ReportsTo.EmployeeId);

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
        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void SqlTranslation_Two_Member_Predicate()
        {
            var query = Query<Employee>().Where(e => e.ReportsTo.City == e.ReportsTo.FirstName);

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
        [Test, Repeat(Repeat)]
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

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_CurrentDate()
        {
            var query = Query<Employee>().Where(e => e.BirthDate == DateTime.Now);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[BirthDate] = GetDate())")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberYearCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Year == DateTime.Now.Year);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Year([T0].[BirthDate]) = Year(GetDate()))")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberMonthCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Month == DateTime.Now.Month);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Month([T0].[BirthDate]) = Month(GetDate()))")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberDayCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Day == DateTime.Now.Day);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Day([T0].[BirthDate]) = Day(GetDate()))")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberHourCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Hour == DateTime.Now.Hour);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('h', [T0].[BirthDate]) = Datepart('h', GetDate()))")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberMinuteCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Minute == DateTime.Now.Minute);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('m', [T0].[BirthDate]) = Datepart('m', GetDate()))")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberSecondCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Second == DateTime.Now.Second);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('s', [T0].[BirthDate]) = Datepart('s', GetDate()))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberMillisecondCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Millisecond == DateTime.Now.Millisecond);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('ms', [T0].[BirthDate]) = Datepart('ms', GetDate()))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberDateCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.Date == DateTime.Now.Date);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Cast([T0].[BirthDate as Date]) = cast(GetDate() as Date))")
            );
        }
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberDayOfYearCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.DayOfYear == DateTime.Now.DayOfYear);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('dy', [T0].[BirthDate]) = Datepart('dy', GetDate()))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_With_MemberDayOfWeekCompare()
        {
            var query = Query<Employee>().Where(e => e.BirthDate.DayOfWeek == DateTime.Now.DayOfWeek);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (Datepart('dw', [T0].[BirthDate]) = Datepart('dw', GetDate()))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByYear()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Year == 2010)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Year([T0].[BirthDate]) = @p0)")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByMonth()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Month == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Month([T0].[BirthDate]) = @p0)")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByDay()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Day == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Day([T0].[BirthDate]) = @p0)")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByDayOfTheWeek()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.DayOfWeek == DayOfWeek.Monday)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Datepart('dw', [T0].[BirthDate]) = @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByDayOfTheYear()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.DayOfYear == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Datepart('dy', [T0].[BirthDate]) = @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByHour()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Hour == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Datepart('h', [T0].[BirthDate]) = @p0)")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_ByMinute()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Minute == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Datepart('m', [T0].[BirthDate]) = @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Dated_Predicate_BySecond()
        {
            var query = Query<Employee>()
                .Where(e => e.BirthDate.Second == 6)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Datepart('s', [T0].[BirthDate]) = @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_StartsWith()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.StartsWith("a"))
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] like @p0 + '%')")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_EndsWith()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.EndsWith("a"))
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] like '%' + @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_Contains()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Contains("a"))
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] like '%' + @p0 + '%')")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_Contains_Field()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Contains(e.City))
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] like '%' + [T0].[City] + '%')")
            );

        }

        //
        // Issue #36
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_ToUpper()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.ToUpper() == e.City)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Upper([T0].[LastName]) = [T0].[City])")
            );

        }

        //
        // Issue #36
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_ToLower()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.ToLower() == e.City)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Lower([T0].[LastName]) = [T0].[City])")
            );

        }

        //
        // Issue #38
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_SubString()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Substring(0, 10) == e.City)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Substring([T0].[LastName], 0, 10) = [T0].[City])")
            );
        }

        //
        // Issue #38
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_SubString_Start()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Substring(5) == e.City)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Substring([T0].[LastName], 5) = [T0].[City])")
            );
        }

        //
        // Issue #38
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_EmployeeId_ToString()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName == e.EmployeeId.ToString())
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] = Cast([T0].[EmployeeId] as nvarchar(max)))")
            );

        }

        // [Test, Repeat(Repeat)]
        public void SqlTranslation_Custom_Projection_Without_Translation()
        {
            var query = Query<Employee>()
                .Select(e => new { e.EmployeeId, DateTime = e.BirthDate.AddDays(1) });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[BirthDate] From [Employees] [T0]")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_EmployeeId_Convert()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName == Convert.ToString(e.EmployeeId))
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where ([T0].[LastName] = Cast([T0].[EmployeeId] as nvarchar(max)))")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Int_Predicate_EmployeeId_Convert()
        {
            var query = Query<Employee>()
                .Where(e => Convert.ToInt32(e.LastName) == e.EmployeeId)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Cast([T0].[LastName] as int) = [T0].[EmployeeId])")
            );

        }

        //
        // Issue #36
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Projection_ToUpper()
        {
            var query = Query<Employee>()
                .Select(e => e.LastName.ToUpper() );

            Assert.That(
                Translate(query),
                Is.EqualTo("Select Upper([T0].[LastName]) [LastName] From [Employees] [T0]")
            );

        }

        //
        // Issue #36
        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Projection_ToLower()
        {
            var query = Query<Employee>()
                .Select(e => e.LastName.ToLower());

            Assert.That(
                Translate(query),
                Is.EqualTo("Select Lower([T0].[LastName]) [LastName] From [Employees] [T0]")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_Length()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Length == 10)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Len([T0].[LastName]) = @p0)")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_String_Predicate_Length_Between_Members()
        {
            var query = Query<Employee>()
                .Where(e => e.LastName.Length == e.FirstName.Length)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Where (Len([T0].[LastName]) = Len([T0].[FirstName]))")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId()
        {
            var query = Query<Employee>()
                .OrderBy(e => e.EmployeeId)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId]")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_ThenBy_LastName()
        {
            var query = Query<Employee>()
                .OrderBy(e => e.EmployeeId).ThenBy(e => e.LastName)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId], [T0].[LastName]")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_And_LastName()
        {
            var query = Query<Employee>()
                .OrderBy(e => e.EmployeeId).OrderBy(e => e.LastName)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId], [T0].[LastName]")
            );

        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_And_LastName_Desc()
        {
            var query = Query<Employee>()
                .OrderBy(e => e.EmployeeId).OrderByDescending(e => e.LastName)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId], [T0].[LastName] Desc")
            );

        }



        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_Desc_And_LastName()
        {
            var query = Query<Employee>()
                .OrderByDescending(e => e.EmployeeId).OrderBy(e => e.LastName)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId] Desc, [T0].[LastName]")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_Desc_And_LastName_Desc()
        {
            var query = Query<Employee>()
                .OrderByDescending(e => e.EmployeeId).OrderByDescending(e => e.LastName)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId] Desc, [T0].[LastName] Desc")
            );

        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_OrderBy_EmployeeId_Desc()
        {
            var query = Query<Employee>()
                .OrderByDescending(e => e.EmployeeId)
                .Select(e => new { e.EmployeeId });

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId] From [Employees] [T0] Order By [T0].[EmployeeId] Desc")
            );

        }

        /// <summary>
        /// 
        /// Sql translation for a simple predicate
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_Members_Predicate()
        {
            var query = Query<Employee>().Where(e => e.LastName == e.City);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[LastName] = [T0].[City])")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Null_Compare_Predicate()
        {
            var query = Query<Employee>().Where(e => e.LastName == null);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[LastName] Is Null)")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Null_Compare_Predicate1()
        {
            string lastName = null;
            var query = Query<Employee>().Where(e => e.LastName == lastName || lastName == null);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where (([T0].[LastName] = @p0) Or (@p1 Is Null))")
            );
        }

        [Repeat(Repeat)]
        [TestCase("SomeName")]
        [TestCase(null)]
        public void SqlTranslation_FuncNull_Compare_Predicate(string lastName)
        {
            Func<string> getLastName = () => lastName;

            var query = Query<Employee>().Where(e => e.LastName == getLastName());

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[LastName] = @p0)")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Not_Null_Compare_Predicate()
        {
            var query = Query<Employee>().Where(e => e.LastName != null);

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[EmployeeId], [T0].[LastName], [T0].[FirstName], [T0].[Title], [T0].[TitleOfCourtesy], [T0].[BirthDate], [T0].[HireDate], [T0].[Address], [T0].[City], [T0].[Region], [T0].[PostalCode], [T0].[Country], [T0].[HomePhone], [T0].[Extension], [T0].[Notes], [T0].[Photo], [T0].[ReportsTo], [T0].[PhotoPath], [T0].[Version] From [Employees] [T0] Where ([T0].[LastName] Is Not Null)")
            );
        }

        /// <summary>
        /// 
        /// Sql translation for a multiple predicate where clauses
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
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
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_Join()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        select new { Order = o, Detail = od };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])")
            );
        }


        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_EntityProjection_Join()
        {
            var query = from o in Query<OrderSimplified>()
                        join od in Query<OrderDetailsSimplified>() on o.OrderId equals od.OrderId
                        where od.OrderId > 10
                        select od;

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Where ([T1].[OrderId] > @p0)")
            );
        }

        public class Projection
        {
            public int OrderId { get; set; }
            public decimal PrecUnit { get; set; }
            public DateTime OrderDate { get; set; }
            public string ShipName { get; set; }
            public int EmployeeId { get; set; }
            
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_CustomEntityProjection_Join()
        {
            var query = from o in Query<OrderSimplified>()
                        join od in Query<OrderDetailsSimplified>() on o.OrderId equals od.OrderId
                        where od.OrderId > 10
                        select new Projection
                               {
                                   OrderId = od.OrderId,
                                   PrecUnit = od.UnitPrice,
                                   OrderDate = o.OrderDate,
                                   ShipName = o.ShipName,
                                   EmployeeId = o.EmployeeId
                               };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T1].[OrderId], [T1].[UnitPrice], [T0].[OrderDate], [T0].[ShipName], [T0].[EmployeeId] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Where ([T1].[OrderId] > @p0)")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Simple_NonKey_Join()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o.Freight equals od.UnitPrice
                        select new { o.OrderId, od.UnitPrice, o.ShipCity };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T1].[UnitPrice], [T0].[ShipCity] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[Freight] = [T1].[UnitPrice])")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Joins_Predicated()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        where o.Freight > od.UnitPrice
                        select new { Order = o, Detail = od };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Where ([T0].[Freight] > [T1].[UnitPrice])")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Joins_Projection_Unnamed()
        {
            var query = from Order in Query<Orders>()
                        join Detail in Query<OrderDetails>() on Order equals Detail.Order
                        select new { Order, Detail };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_2Joins()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        select new { Order = o, Detail = od, Employee = e };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount], [T2].[EmployeeId], [T2].[LastName], [T2].[FirstName], [T2].[Title], [T2].[TitleOfCourtesy], [T2].[BirthDate], [T2].[HireDate], [T2].[Address], [T2].[City], [T2].[Region], [T2].[PostalCode], [T2].[Country], [T2].[HomePhone], [T2].[Extension], [T2].[Notes], [T2].[Photo], [T2].[ReportsTo], [T2].[PhotoPath], [T2].[Version] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Left Join [Employees] [T2] On ([T0].[EmployeeId] = [T2].[EmployeeId])")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_2Joins_Predicated()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        where o.Employee.Title == e.Region
                        select new { Order = o, Detail = od, e };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount], [T2].[EmployeeId], [T2].[LastName], [T2].[FirstName], [T2].[Title], [T2].[TitleOfCourtesy], [T2].[BirthDate], [T2].[HireDate], [T2].[Address], [T2].[City], [T2].[Region], [T2].[PostalCode], [T2].[Country], [T2].[HomePhone], [T2].[Extension], [T2].[Notes], [T2].[Photo], [T2].[ReportsTo], [T2].[PhotoPath], [T2].[Version] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Left Join [Employees] [T2] On ([T0].[EmployeeId] = [T2].[EmployeeId]) Where ([T0].[EmployeeId] In (Select [T3].[EmployeeId] From [Employees] [T3] Where [T3].[Title] = [T2].[Region]))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_2Joins_Predicated_Reversed()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        where e.Region == o.Employee.Title
                        select new { Order = o, Detail = od, e };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount], [T2].[EmployeeId], [T2].[LastName], [T2].[FirstName], [T2].[Title], [T2].[TitleOfCourtesy], [T2].[BirthDate], [T2].[HireDate], [T2].[Address], [T2].[City], [T2].[Region], [T2].[PostalCode], [T2].[Country], [T2].[HomePhone], [T2].[Extension], [T2].[Notes], [T2].[Photo], [T2].[ReportsTo], [T2].[PhotoPath], [T2].[Version] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Left Join [Employees] [T2] On ([T0].[EmployeeId] = [T2].[EmployeeId]) Where ([T0].[EmployeeId] In (Select [T3].[EmployeeId] From [Employees] [T3] Where [T3].[Title] = [T2].[Region]))")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Translate_GroupedJoins()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order into ods
                        select new { Order = o, Details = ods };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])")
            );
        }
        
        [Test, Repeat(Repeat)]
        public void SqlTranslation_Translate_Joined_And_GroupBy_With_Many_Fields()
        {
            var query = from order in Query<Orders>()
                        join detail in Query<OrderDetails>() on order equals detail.Order into details
                        join employee in Query<Employee>() on order.Employee equals employee
                        join customer in Query<Customers>() on order.Customer equals customer
                        group details by new
                        {
                            order.OrderId,
                            employee.FirstName,
                            employee.LastName,
                            order.Customer,
                            details
                        } into grouped
                        select new
                        {
                            grouped.Key.OrderId,
                            grouped.Key.FirstName,
                            grouped.Key.LastName,
                            grouped.Key.details
                        };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T2].[FirstName], [T2].[LastName], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Left Join [Employees] [T2] On ([T0].[EmployeeId] = [T2].[EmployeeId]) Left Join [Customers] [T3] On ([T0].[CustomerId] = [T3].[CustomerId]) Group By [T0].[OrderId], [T2].[FirstName], [T2].[LastName], [T0].[CustomerId], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount]")
            );
        }

        [Test, Repeat(Repeat)]
        public void SqlTranslation_Translate_GroupedJoins_Predicated()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order into ods
                        where o.Freight > 0
                        select new { Order = o, Details = ods };

            Assert.That(
                Translate(query),
                Is.EqualTo("Select [T0].[OrderId], [T0].[CustomerId], [T0].[EmployeeId], [T0].[OrderDate], [T0].[RequiredDate], [T0].[ShippedDate], [T0].[ShipVia], [T0].[Freight], [T0].[ShipName], [T0].[ShipAddress], [T0].[ShipCity], [T0].[ShipRegion], [T0].[ShipPostalCode], [T0].[ShipCountry], [T1].[OrderId], [T1].[ProductId], [T1].[UnitPrice], [T1].[Quantity], [T1].[Discount] From [Orders] [T0] Left Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId]) Where ([T0].[Freight] > @p0)")
            );
        }
    }
}
