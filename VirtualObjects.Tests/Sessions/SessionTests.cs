using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Connections;
using VirtualObjects.Exceptions;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    /// <summary>
    /// 
    /// Unit-Tests for session. 
    /// This will also test the IOC Configuration and all dependencies.
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Session")]
    public class SessionTests : TimedTests
    {

        private ISession CreateSession()
        {
            return new Session(new SessionConfiguration
            {
                Logger = Console.Out
            }, "northwind");
        }

        [Test, Repeat(Repeat)]
        public void Session_Should_Be_Created_And_Disposed()
        {
            using (CreateSession())
            {

            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Transaction_Should_Be_Created_And_Disposed()
        {

            using (var session = CreateSession())
            {
                using (session.BeginTransaction())
                {


                }
            }

        }

        [Test, Repeat(Repeat)]
        public void Session_Crud_Operations()
        {
            using (var session = CreateSession())
            {
                session.WithinTransaction(() => Diagnostic.Timed(() =>
                {
                    var employee = session.Insert(new Employee
                    {
                        FirstName = "Sérgio",
                        LastName = "Ferreira"
                    });

                    employee = session.GetById(employee);

                    employee.BirthDate = new DateTime(1983, 4, 16);

                    session.Update(employee);

                    session.Delete(employee);
                }));
            }

        }

        [Test, Repeat(Repeat)]
        public void Session_Queries()
        {
            using (var session = CreateSession())
            {
                var employees = Diagnostic.Timed(() => session.Query<Employee>().Where(e => e.EmployeeId > 0).ToList());
                employees.Count.Should().Be(9);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Entity_Lazy_Load()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });
                var reportsTo = Diagnostic.Timed(() => employee.ReportsTo);

                reportsTo.Should().NotBeNull();
                reportsTo.EmployeeId.Should().Be(2);
                reportsTo.LastName.Should().Be("Fuller");
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Collection_Fields_Lazy_Load()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });
                var territories = Diagnostic.Timed(() => employee.Territories).ToList();

                territories.Should().NotBeNull();
                territories.Count.Should().Be(2);

                var employeeTerritories = territories.First();
                employeeTerritories.Territories.Region.Should().NotBeNull();

                var employee1 = employeeTerritories.Employee;

                employee1.FirstName.Should().Be(employee.FirstName);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>();

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>();
                var i = 0;

                foreach (Employee employee in employees)
                {
                    Assert.That(employee.EmployeeId, Is.GreaterThan(0));
                    ++i;
                }

                Assert.AreEqual(9, i);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees1()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>();
                var i = 0;

                foreach (Employee employee in employees)
                {
                    Assert.That(employee.EmployeeId, Is.GreaterThan(0));
                    ++i;
                }

                Assert.AreEqual(9, i);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees_Projected()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Select(e => new { e.EmployeeId, e.City, e.Country });

                var i = 0;

                foreach (var employee in employees)
                {
                    Assert.That(employee.EmployeeId, Is.GreaterThan(0));
                    Assert.That(employee.City, Is.Not.Null);
                    Assert.That(employee.Country, Is.Not.Null);
                    ++i;
                }

                Assert.AreEqual(9, i);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees_Projected1()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Select(e => new { e.EmployeeId, e.City, e.Country });

                var i = 0;

                foreach (var employee in employees)
                {
                    Assert.That(employee.EmployeeId, Is.GreaterThan(0));
                    Assert.That(employee.City, Is.Not.Null);
                    Assert.That(employee.Country, Is.Not.Null);
                    ++i;
                }

                Assert.AreEqual(9, i);
            }
        }

        /// <summary>
        ///
        /// Assert the reportsTo field is loaded with the proper value after a query.
        ///
        /// </summary>
        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees_ReportsTo()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>();
                var reportsTo = employees.ToList().Select(employee => employee.ReportsTo).ToList();

                Assert.AreEqual(1, reportsTo.Count(e => e.EmployeeId == 1));
                Assert.AreEqual(5, reportsTo.Count(e => e.EmployeeId == 2));

                Assert.That(reportsTo.Count, Is.EqualTo(9));
            }
        }

        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Session_GetAll_Employees_From_Orders()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee.EmployeeId)
                        .Contains(e.ReportsTo.EmployeeId));


                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Employees_From_Orders_Simpler()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee)
                        .Contains(e.ReportsTo));

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Employees_From_Orders_Count_Twice()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee)
                        .Contains(e.ReportsTo));


                Assert.AreEqual(9, employees.Count());

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Session_GetAll_Shippers_From_Orders()
        {
            using (var session = CreateSession())
            {
                var shippers = session.GetAll<Shippers>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Shipper.ShipperId)
                        .Contains(e.ShipperId));

                Assert.AreEqual(3, shippers.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Orders_Skip_Take()
        {
            using (var session = CreateSession())
            {
                var orders = session.GetAll<Orders>()
                    .Where(e => e.OrderId > 10)
                    .Skip(1).Take(10).ToList();

                Assert.AreEqual(10, orders.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Orders_With_Specific_Shippers()
        {
            using (var session = CreateSession())
            {
                var orders = session.GetAll<Orders>()
                    .Where(e => session.GetAll<Shippers>()
                        .Where(o => o.ShipperId > 0)
                        .Select(o => o.ShipperId)
                        .Contains(e.Shipper.ShipperId));

                Assert.AreEqual(830, orders.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Orders_With_Specific_Shippers_Simpler()
        {
            using (var session = CreateSession())
            {
                var orders = session.GetAll<Orders>()
                   .Where(e => session.GetAll<Shippers>()
                       .Where(o => o.ShipperId > 0)
                       .Contains(e.Shipper));

                Assert.AreEqual(830, orders.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllOrders()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Orders>();

                Assert.AreEqual(830, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_WhereCondition_ShouldBeKept_ByResult()
        {
            using (var session = CreateSession())
            {
                var query1 = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId > 4);

                var query2 = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId == 2);

                var query3 = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId == 2 || m.EmployeeId == 3);


                Assert.AreEqual(5, query1.Count());
                Assert.AreEqual(1, query2.Count());
                Assert.AreEqual(2, query3.Count());

                Assert.AreEqual(5, query1.Count());
                Assert.AreEqual(1, query2.Count());
                Assert.AreEqual(2, query3.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_LikeCondition()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName.Contains("an"));

                Assert.AreEqual(2, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_LikeCondition_Left()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName.StartsWith("d"));

                Assert.AreEqual(2, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_OrderedByFirstName()
        {
            using (var session = CreateSession())
            {
                var employee = session
                    .GetAll<Employee>().OrderBy(e => e.FirstName)
                    .First();

                EntitiesAsserts.Assert_Employee_2(employee);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Count_Predicated()
        {
            using (var session = CreateSession())
            {
                Assert.That(session.GetAll<Products>().Count(e => e.Discontinued), Is.EqualTo(8));
                Assert.That(session.GetAll<Products>().Count(e => !e.Discontinued), Is.EqualTo(69));
            }
        }


        //[Test, Repeat(Repeat)]
        //public void Session_GetAll_With_String_Where()
        //{
        //    using ( var session = CreateSession() )
        //    {
        //        Assert.That(session.GetAll<Employee>().WhereString("EmployeeId = 1").Count(), Is.EqualTo(1));
        //        Assert.That(session.GetAll<Employee>().WhereString("EmployeeId > 2").Count(), Is.EqualTo(7));
        //    }
        //}

        [Test, Repeat(Repeat)]
        public void Session_GetAll_ToString_Method()
        {
            using (var session = CreateSession())
            {
                var foo = new
                {
                    LastName = "test",
                    ToString = (Func<String>)(() => "Andrew")
                };

                var employees = session.GetAll<Employee>()
                    .Where(e => e.FirstName == foo.ToString());


                Assert.That(employees.Count(), Is.EqualTo(1));
            }
        }

        private enum Testing
        {
            Andrew
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Enum_ToString_Method()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>().Where(e => e.FirstName == Testing.Andrew.ToString());

                Assert.That(employees.Count(), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_New_Object()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.BirthDate == new DateTime(1948, 12, 8));

                Assert.That(employees.Count(), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetEmployeeList_ReportsToFilter()
        {
            using (var session = CreateSession())
            {
                var employees1 = session
                    .GetAll<Employee>().Where(e => e.ReportsTo.FirstName == "Andrew");

                Assert.AreEqual(5, employees1.Count());

                EntitiesAsserts.Assert_Employee_1(employees1.First());

                var employees2 = session
                    .GetAll<Employee>().Where(e => e.ReportsTo.EmployeeId == 1);

                EntitiesAsserts.Assert_Employee_2(employees2.First());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetEmployee_ReportsTo_First()
        {
            using (var session = CreateSession())
            {
                EntitiesAsserts.Assert_Employee_1(session
                    .GetAll<Employee>()
                    .First(e => e.ReportsTo.FirstName == "Andrew"));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_OrderedByFirstName_TSql()
        {
            using (var session = CreateSession())
            {
                var employee = session
                    .GetAll<Employee>().OrderBy(e => e.FirstName)
                    .First();

                EntitiesAsserts.Assert_Employee_2(employee);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_OrderedByCity_Descending()
        {
            using (var session = CreateSession())
            {
                var employee2 = session
                    .GetAll<Employee>()
                    .OrderByDescending(e => e.City)
                    .FirstOrDefault();

                EntitiesAsserts.Assert_Employee_2(employee2);

                var employee3 = session
                    .GetAll<Employee>()
                    .OrderBy(e => e.City)
                    .First();

                EntitiesAsserts.Assert_Employee_3(employee3);
            }
        }


        [Test, Repeat(Repeat)]
        public void Session_GetAll_QueryWith_OrCondition()
        {
            using (var session = CreateSession())
            {
                var employees = session
                    .GetAll<Employee>()
                    .Where(m => m.EmployeeId == 1 ||
                                m.EmployeeId == 4 ||
                                m.EmployeeId == 2 ||
                                m.EmployeeId == 5)
                    .Where(m => m.EmployeeId > 3);

                Assert.AreEqual(2, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_MoreComplexQuery2()
        {
            using (var session = CreateSession())
            {
                var employees =
                    session.GetAll<Employee>().Where(e => e.LastName.Contains("r") || e.LastName.Contains("a"));

                Assert.AreEqual(8, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_MoreComplexQuery()
        {
            using (var session = CreateSession())
            {
                var employees = session
                    .GetAll<Employee>()
                    .Where(m => m.EmployeeId > 0)
                    .Where(m => m.EmployeeId < 10);

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_withConditionExpression()
        {
            using (var session = CreateSession())
            {
                var id = 1;
                var employees = session
                    .GetAll<Employee>().Where(m => m.EmployeeId == id);

                Assert.AreEqual(1, employees.Count());
            }
        }

        //[Test, Repeat(Repeat)]
        //public void Session_GetAll_withInConditionExpression()
        //{
        //    using ( var session = CreateSession() )
        //    {
        //        var employees = session
        //            .GetAll<Employee>()
        //            .WhereField(m => m.EmployeeId).In(new[] { 1, 2, 3 });

        //        Assert.AreEqual(3, employees.Count());
        //    }
        //}

        //[Test, Repeat(Repeat)]
        //public void Session_GetAll_withInConditionExpression_Complex()
        //{
        //    using ( var session = CreateSession() )
        //    {
        //        var employees = session
        //            .GetAll<Employee>()
        //            .WhereField(m => m.EmployeeId).In(new[] { 1, 2, 3, 4, 5, 6, 7 })
        //            .Where(m => m.EmployeeId >= 2);

        //        Assert.AreEqual(6, employees.Count());
        //    }
        //}

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Between()
        {
            using (var session = CreateSession())
            {
                var employees = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId >= 1 && m.EmployeeId <= 5);

                Assert.AreEqual(5, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_In_using_collection()
        {
            using (var session = CreateSession())
            {
                var collection = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId <= 5);

                var employees = session.GetAll<Employee>()
                    .Where(e => collection.Select(o => o.EmployeeId).Contains(e.EmployeeId));

                Assert.AreEqual(5, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_In_using_collection_SameNames()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 5 });

                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Employee>()
                            .Where(m => m.ReportsTo.EmployeeId == employee.EmployeeId)
                            .Select(o => o.EmployeeId).Contains(e.EmployeeId));


                Assert.AreEqual(3, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_In_using_collection_diffNames()
        {
            using (var session = CreateSession())
            {
                var orders = session.GetAll<Orders>()
                    .Where(o => session.GetAll<Shippers>().Where(s => s.ShipperId == 2).Contains(o.Shipper))
                    .Where(o => o.Employee.EmployeeId == 4);

                Assert.AreEqual(70, orders.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_withConditionExpression_IdConst()
        {
            using (var session = CreateSession())
            {
                var employees = session
                    .GetAll<Employee>().Where(m => m.EmployeeId == 1);

                Assert.AreEqual(1, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Count()
        {
            using (var session = CreateSession())
            {
                Assert.AreEqual(9, session.Count<Employee>());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById()
        {
            using (var session = CreateSession())
            {
                using (var s = CreateSession())
                {
                    var employee1 = s.GetById(new Employee { EmployeeId = 1 });

                    EntitiesAsserts.Assert_Employee_1(employee1);
                }

                var employee = session.GetById(new Employee { EmployeeId = 1 });

                EntitiesAsserts.Assert_Employee_1(employee);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Exists()
        {
            using (var session = CreateSession())
            {
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 1 }));
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 2 }));
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 3 }));

                Assert.IsFalse(session.Exists(new Employee { EmployeeId = 123123 }));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_ReportsTo()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                EntitiesAsserts.Assert_Employee_2(employee.ReportsTo);
                EntitiesAsserts.Assert_Employee_1(employee.ReportsTo.ReportsTo);
                EntitiesAsserts.Assert_Employee_2(employee.ReportsTo.ReportsTo.ReportsTo);
                EntitiesAsserts.Assert_Employee_1(employee);
            }
        }


        [Test, Repeat(Repeat)]
        public void Session_GetById_Territories()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Count(), Is.EqualTo(2));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_AnyTerritories()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Any(), Is.True);
            }
        }

        /// <summary>
        ///
        /// Asserts that the ANY clause can be used with a lambda predicate
        ///
        /// </summary>
        [Test, Repeat(Repeat)]
        public void Session_GetById_Any_Filtered_Territories()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Any(e => e.Territories.TerritoryId == "06897"), Is.True);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Where(e => e.Territories.TerritoryId == "06897").Count(), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Count(e => e.Territories.TerritoryId == "06897"), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count_EntityParameter()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                var territories = new Territories { TerritoryId = "06897" };

                Assert.That(
                    employee.Territories.Count(e => e.Territories == territories),
                    Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count_EntityParameter_Inline_Init()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                Assert.That(
                    employee.Territories.Count(e => e.Territories == new Territories { TerritoryId = "06897" }),
                    Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById_With_Setted_Territories()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                employee.Territories = session.GetAll<EmployeeTerritories>();

                var territories = employee.Territories;

                Assert.That(territories.Count(), Is.EqualTo(49));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Reuse_Returned_Entity()
        {
            using (var session = CreateSession())
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                var reportsTo = session.GetById(employee.ReportsTo);

                Assert.That(reportsTo, Is.Not.Null);
                EntitiesAsserts.Assert_Employee_2(reportsTo);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Insert_Employee()
        {
            using (var session = CreateSession())
            {
                session.WithRollback(() =>
                {
                    var count = session.GetAll<Employee>().Count();
                    var employee = session.Insert(new Employee
                    {
                        FirstName = "Sérgio",
                        LastName = "Ferreira"
                    });

                    Assert.IsNotNull(employee);
                    Assert.AreEqual(count + 1, session.GetAll<Employee>().Count());
                });
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Delete_Employee()
        {
            using (var session = CreateSession())
            {
                session.WithRollback(() =>
                {
                    var count = session.GetAll<Employee>().Count();
                    var sergio = session.Insert(new Employee
                    {
                        FirstName = "Sérgio",
                        LastName = "Ferreira"
                    });

                    session.Delete(sergio);

                    Assert.AreEqual(count, session.GetAll<Employee>().Count());
                });
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Update_Old_Employee()
        {
            using (var session = CreateSession())
            {
                session.WithRollback(() =>
                {
                    var sergio = session.Insert(new Employee
                    {
                        FirstName = "Sérgio",
                        LastName = "Ferreira"
                    });

                    var id = sergio.EmployeeId;
                    
                    session.Update(new Employee
                    {
                        EmployeeId = sergio.EmployeeId,
                        FirstName = "Alien",
                        LastName = sergio.LastName
                    });

                    var alien = session.GetById(sergio);

                    Assert.That(alien, Is.Not.Null);
                    Assert.AreEqual("Alien", alien.FirstName);
                });
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Update_Employee()
        {
            using (var session = CreateSession())
            {
                session.WithRollback(() =>
                {
                    var sergio = session.Insert(new Employee
                    {
                        FirstName = "Sérgio",
                        LastName = "Ferreira"
                    });

                    sergio.FirstName = "Alien";

                    session.Update(sergio);

                    var alien = session.GetById(sergio);

                    Assert.That(alien, Is.Not.Null);
                    Assert.AreEqual("Alien", alien.FirstName);
                });
            }
        }

        [Test]
        public void Session_GetOrders_Join_OrderDetails()
        {
            using (var session = CreateSession())
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             select new { Order = o, OrderDetail = od };

                foreach (var order in orders)
                {
                    Assert.That(order, Is.Not.Null);
                    Assert.That(order.Order, Is.Not.Null);
                    Assert.That(order.OrderDetail, Is.Not.Null);

                    Assert.That(order.OrderDetail.Order.OrderId, Is.EqualTo(order.Order.OrderId));
                }

                Assert.That(orders.Count(), Is.EqualTo(2155));
            }
        }

        [Test]
        public void Session_GetOrders_2Joins_OrderDetails()
        {
            using (var session = CreateSession())
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             join e in session.GetAll<Employee>() on o.Employee equals e
                             select new { Order = o, OrderDetail = od, Employee = e };

                foreach (var order in orders)
                {
                    Assert.That(order, Is.Not.Null);
                    Assert.That(order.Order, Is.Not.Null);
                    Assert.That(order.OrderDetail, Is.Not.Null);

                    Assert.That(order.OrderDetail.Order.OrderId, Is.EqualTo(order.Order.OrderId));
                }

                Assert.That(orders.Count(), Is.EqualTo(2155));
            }
        }

        [Test]
        public void Session_GetOrders_GroupedJoin_OrderDetails()
        {
            using (var session = CreateSession())
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order into god
                             select new { Order = o, OrderDetails = god };

                int i = 0;

                foreach (var order in orders)
                {
                    Assert.That(order, Is.Not.Null);
                    Assert.That(order.Order, Is.Not.Null);
                    Assert.That(order.OrderDetails, Is.Not.Null);

                    Assert.That(order.OrderDetails, Is.Not.Empty);
                    Assert.That(order.OrderDetails.Count(), Is.Not.EqualTo(2155));
                    Assert.That(order.OrderDetails.First().Order.OrderId, Is.EqualTo(order.Order.OrderId));
                    ++i;
                }

                Assert.That(i, Is.EqualTo(830));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetOrders_Join_OrderDetails_Projected()
        {
            using (var session = CreateSession())
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             select new { o.OrderId, od.Quantity, od.UnitPrice, o.OrderDate };

                var i = 0;

                foreach (var order in orders)
                {
                    Assert.That(order.OrderId, Is.GreaterThan(0));
                    Assert.That(order.Quantity, Is.GreaterThan(0));
                    Assert.That(order.UnitPrice, Is.GreaterThan(0));
                    Assert.That(order.OrderDate, Is.InstanceOf<DateTime>());
                    ++i;
                }

                Assert.AreEqual(2155, i);
            }
        }

    }
}
