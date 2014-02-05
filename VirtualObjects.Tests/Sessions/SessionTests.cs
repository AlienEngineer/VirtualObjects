using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
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
                //   Logger = Console.Out
            }, "northwind");
        }

        [Test, Repeat(Repeat)]
        public void Session_Should_Be_Created_And_Disposed()
        {

            using ( var session = CreateSession() )
            {

            }

        }

        [Test, Repeat(Repeat)]
        public void Session_Transaction_Should_Be_Created_And_Disposed()
        {

            using ( var session = CreateSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {


                }
            }

        }

        [Test, Repeat(Repeat)]
        public void Session_Crud_Operations()
        {
            using ( var session = CreateSession() )
            {
                session.WithinTransaction(() =>
                {
                    Diagnostic.Timed(() =>
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
                    });
                });
            }

        }

        [Test, Repeat(Repeat)]
        public void Session_Queries()
        {
            using ( var session = CreateSession() )
            {
                var employees = Diagnostic.Timed(() => session.Query<Employee>().Where(e => e.EmployeeId > 0).ToList());
                employees.Count.Should().Be(9);
            }
        }

        [Test, Repeat(Repeat)]
        public void Session_Entity_Lazy_Load()
        {
            using ( var session = CreateSession() )
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
            using ( var session = CreateSession() )
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

    }
}
