using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
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
    public class SessionTests : UtilityBelt
    {
        private ISession CreateSession()
        {
            return new Session(new SessionConfiguration(), "northwind");
        }

        [Test, Repeat(Repeat)]
        public void Session_Should_Be_Created_And_Disposed()
        {
            using (var session = CreateSession() )
            {

                

            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Transaction_Should_Be_Created_And_Disposed()
        {

            using ( var session = CreateSession() )
            {
                using ( session.BeginTransaction() )
                {


                }
            }
            
        }

        [Test, Repeat(Repeat)]
        public void Session_Crud_Operations()
        {
            var session = Session;
            {
                session.WithinTransaction(transaction => Diagnostic.Timed(() =>
                {
                    var employee = session.Insert(new Employee
                    {
                        FirstName = "S_C_O",
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
            var session = Session;
            {
                var employees = Diagnostic.Timed(() => session.Query<Employee>().Where(e => e.EmployeeId > 0).ToList());
                employees.Count.Should().Be(9);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Entity_Lazy_Load()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });
                var reportsTo = Diagnostic.Timed(() => employee.ReportsTo);

                reportsTo.Should().NotBeNull();
                reportsTo.EmployeeId.Should().Be(2);
                reportsTo.LastName.Should().Be("Fuller");
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>();

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAllEmployees()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>();
                var i = 0;

                foreach ( Employee employee in employees )
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>();
                var i = 0;

                foreach ( Employee employee in employees )
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Select(e => new { e.EmployeeId, e.City, e.Country });

                var i = 0;

                foreach ( var employee in employees )
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Select(e => new { e.EmployeeId, e.City, e.Country });

                var i = 0;

                foreach ( var employee in employees )
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>();
                var reportsTo = employees.ToList().Select(employee => employee.ReportsTo).ToList();

                Assert.AreEqual(1, reportsTo.Count(e => e.EmployeeId == 1));
                Assert.AreEqual(5, reportsTo.Count(e => e.EmployeeId == 2));

                Assert.That(reportsTo.Count, Is.EqualTo(9));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Employees_From_Orders()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee.EmployeeId)
                        .Contains(e.ReportsTo.EmployeeId));


                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Session_GetAll_Employees_From_Orders_MemberAccess_OnProjection()
        {
            using ( var session = CreateSession() )
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee.Country)
                        .Contains(e.ReportsTo.Country));


                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Employees_From_Orders_Simpler()
        {
            var session = Session;
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => session.GetAll<Orders>()
                        .Select(o => o.Employee)
                        .Contains(e.ReportsTo));


                Assert.AreEqual(9, employees.Count());

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Shippers_From_Orders()
        {
            var session = Session;
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
            var session = Session;
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
            var session = Session;
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
            var session = Session;
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
            var session = Session;
            {
                var employees = session.GetAll<Orders>();

                Assert.AreEqual(830, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_WhereCondition_ShouldBeKept_ByResult()
        {
            var session = Session;
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
        public void Session_GetAll_Employees_Null_Compare_Predicate()
        {
            var session = Session;
            {
                string lastName = null;
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName == lastName || lastName == null);

                Assert.AreEqual(9, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_Employees_Null_Compare_Predicate1()
        {
            var session = Session;
            {
                string lastName = "Leverling";
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName == lastName || lastName == null);

                Assert.AreEqual(1, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_LikeCondition()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName.Contains("an"));

                Assert.AreEqual(2, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_LikeCondition_Left()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.LastName.StartsWith("d"));

                Assert.AreEqual(2, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_OrderedByFirstName()
        {
            var session = Session;
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
            var session = Session;
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
            var session = Session;
            {
                var foo = new
                {
                    LastName = "test",
                    ToString = (Func<string>)(() => "Andrew")
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>().Where(e => e.FirstName == Testing.Andrew.ToString());

                Assert.That(employees.Count(), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_New_Object()
        {
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(e => e.BirthDate == new DateTime(1948, 12, 8));

                Assert.That(employees.Count(), Is.EqualTo(1));
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_OrderedByFirstName_TSql()
        {
            var session = Session;
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
            var session = Session;
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
            var session = Session;
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
            var session = Session;
            {
                var employees =
                    session.GetAll<Employee>().Where(e => e.LastName.Contains("r") || e.LastName.Contains("a"));

                Assert.AreEqual(8, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_MoreComplexQuery()
        {
            var session = Session;
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
            var session = Session;
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
            var session = Session;
            {
                var employees = session.GetAll<Employee>()
                    .Where(m => m.EmployeeId >= 1 && m.EmployeeId <= 5);

                Assert.AreEqual(5, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetAll_In_using_collection()
        {
            var session = Session;
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
            var session = Session;
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
            var session = Session;
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
            var session = Session;
            {
                var employees = session
                    .GetAll<Employee>().Where(m => m.EmployeeId == 1);

                Assert.AreEqual(1, employees.Count());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Count()
        {
            var session = Session;
            {
                Assert.AreEqual(9, session.Count<Employee>());
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_GetById()
        {
            var session = Session;
            {
                using ( var s = CreateSession() )
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
            var session = Session;
            {
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 1 }));
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 2 }));
                Assert.IsTrue(session.Exists(new Employee { EmployeeId = 3 }));

                Assert.IsFalse(session.Exists(new Employee { EmployeeId = 123123 }));
            }
        }


        [Test, Repeat(Repeat)]
        public void Session_Insert_Employee()
        {
            using ( var session = CreateSession() )
            {
                session.WithRollback(() =>
                {
                    var count = session.GetAll<Employee>().Count();
                    var employee = session.Insert(new Employee
                    {
                        FirstName = "S_I_E",
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
            var session = Session;
            {
                session.WithRollback(() =>
                {
                    var count = session.GetAll<Employee>().Count();
                    var sergio = session.Insert(new Employee
                    {
                        FirstName = "S_D_E",
                        LastName = "Ferreira"
                    });

                    session.Delete(sergio);

                    Assert.AreEqual(count, session.GetAll<Employee>().Count());
                });
            }
        }

        [Test, Repeat(Repeat), ExpectedException(typeof(ExecutionException))]
        public void Session_Update_Old_Employee()
        {
            var session = Session;

            session.WithRollback(() =>
            {
                var sergio = session.Insert(new Employee
                {
                    FirstName = "S_U_O_E",
                    LastName = "Ferreira"
                });

                //
                // Gets the current version of the entity.
                //
                // sergio = session.GetById(sergio);

                var oldVersion = sergio.Version;

                //
                // Update to create a new version of Employee.
                //
                sergio = session.Update(new Employee
                {
                    EmployeeId = sergio.EmployeeId,
                    FirstName = "S_U_O_E",
                    LastName = sergio.LastName,
                    Version = sergio.Version
                });

                //
                // Try to update with the old version.
                //
                session.Update(new Employee
                {
                    EmployeeId = sergio.EmployeeId,
                    FirstName = "S_U_O_E",
                    LastName = sergio.LastName,
                    Version = oldVersion
                });

            });
        }


        [Test, Repeat(Repeat), ExpectedException(typeof(ExecutionException))]
        public void Session_Update_Employee_Unversioned()
        {
            var session = Session;

            session.WithRollback(() =>
            {
                var sergio = session.Insert(new Employee
                {
                    FirstName = "S_U_E_U",
                    LastName = "Ferreira"
                });

                session.Update(new Employee
                {
                    EmployeeId = sergio.EmployeeId,
                    FirstName = "S_U_E_U",
                    LastName = sergio.LastName
                });

            });

        }

        [Test, Repeat(Repeat)]
        public void Session_Update_Employee()
        {
            var session = Session;

            session.WithRollback(() =>
            {
                var sergio = session.Insert(new Employee
                {
                    FirstName = "S_U_E",
                    LastName = "Ferreira"
                });

                // gets the version control
                // sergio = session.GetById(sergio);
                sergio.FirstName = "S_U_E_A";

                session.Update(sergio);

                var alien = session.GetById(sergio);

                Assert.That(alien, Is.Not.Null);
                Assert.AreEqual("S_U_E_A", alien.FirstName);
            });

        }

        [Test]
        public void Session_GetOrders_Join_OrderDetails()
        {
            var session = Session;
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             select new { Order = o, OrderDetail = od };

                foreach ( var order in orders )
                {
                    Assert.That(order, Is.Not.Null);
                    Assert.That(order.Order, Is.Not.Null);
                    Assert.That(order.OrderDetail, Is.Not.Null);

                    Assert.That(order.OrderDetail.Order.OrderId, Is.EqualTo(order.Order.OrderId));
                }

                Assert.That(orders.Count(), Is.EqualTo(2155));

                /*
                 * Select Count(*) From (
                 *      Select 1
                 *      From [Orders] [T0] 
                 *      Inner Join [Order Details] [T1] On ([T0].[OrderId] = [T1].[OrderId])
                 * )[Result]
                 */
            }
        }

        [Test]
        public void Session_GetOrders_2Joins_OrderDetails()
        {
            var session = Session;
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             join e in session.GetAll<Employee>() on o.Employee equals e
                             select new { Order = o, OrderDetail = od, Employee = e };

                foreach ( var order in orders )
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
            var session = Session;
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order into god
                             select new { Order = o, OrderDetails = god };

                int i = 0;

                foreach ( var order in orders )
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
            var session = Session;
            {
                var orders = from o in session.GetAll<Orders>()
                             join od in session.GetAll<OrderDetails>() on o equals od.Order
                             select new { o.OrderId, od.Quantity, od.UnitPrice, o.OrderDate };

                var i = 0;

                foreach ( var order in orders )
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
