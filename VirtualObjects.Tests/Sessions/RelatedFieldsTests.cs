using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    [TestFixture, Category("Related Fields")]
    public class RelatedFieldsTests : UtilityBelt
    {
        [TestCase(TestName ="Related field, navigate using related collection and then fields.")]
        [Repeat(Repeat)]
        public void Session_Collection_Fields_Lazy_Load()
        {
            var session = Session;
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

        [TestCase(TestName = "Related field, use it in a query more than once.")]
        [Repeat(Repeat)]
        public void Session_GetEmployeeList_ReportsToFilter()
        {
            var session = Session;
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

        [TestCase(TestName = "Related field, use it in a query.")]
        [Repeat(Repeat)]
        public void Session_GetEmployee_ReportsTo_First()
        {
            var session = Session;
            {
                EntitiesAsserts.Assert_Employee_1(session
                    .GetAll<Employee>()
                    .First(e => e.ReportsTo.FirstName == "Andrew"));
            }
        }

        [TestCase(TestName = "Related field, navigate into.")]
        [Repeat(Repeat)]
        public void Session_GetById_ReportsTo()
        {
            var session = Session;
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                EntitiesAsserts.Assert_Employee_2(employee.ReportsTo);
                EntitiesAsserts.Assert_Employee_1(employee.ReportsTo.ReportsTo);
                EntitiesAsserts.Assert_Employee_2(employee.ReportsTo.ReportsTo.ReportsTo);
                EntitiesAsserts.Assert_Employee_1(employee);
            }
        }

        [TestCase(TestName = "Related field, use an IQueryable field to get related entities.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Count(), Is.EqualTo(2));
            }
        }

        [TestCase(TestName = "Related field, use an ICollection field to get related entities.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories_Collection()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.TerritoriesCollection.Count(), Is.EqualTo(2));
            }
        }

        [TestCase(TestName = "Related field, filter on a collection field.")]
        [Test, Repeat(Repeat)]
        public void Session_GetById_AnyTerritories()
        {
            var session = Session;
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
        [TestCase(TestName = "Related field, filter on a collection field check if any with a lambda expression.")]
        [Test, Repeat(Repeat)]
        public void Session_GetById_Any_Filtered_Territories()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Any(e => e.Territories.TerritoryId == "06897"), Is.True);
            }
        }

        [TestCase(TestName = "Related field, filter on a collection field with where clause.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Where(e => e.Territories.TerritoryId == "06897").Count(), Is.EqualTo(1));
            }
        }

        [TestCase(TestName = "Related field, filter on a collection field that counts with a lambda expression.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count()
        {
            var session = Session;
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                Assert.That(employee.Territories.Count(e => e.Territories.TerritoryId == "06897"), Is.EqualTo(1));
            }
        }

        [TestCase(TestName = "Related field, filter on a collection field that counts with a lambda expression with complext types.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count_EntityParameter()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                var territories = new Territories { TerritoryId = "06897" };

                Assert.That(
                    employee.Territories.Count(e => e.Territories == territories),
                    Is.EqualTo(1));
            }
        }

        [TestCase(TestName = "Related field, filter on a collection field that counts with a lambda expression with a new statement.")]
        [Repeat(Repeat)]
        public void Session_GetById_Territories_Filtered_On_Count_EntityParameter_Inline_Init()
        {
            var session = Session;
            {
                var employee = session.GetById(
                    new Employee { EmployeeId = 1 });

                Assert.That(
                    employee.Territories.Count(e => e.Territories == new Territories { TerritoryId = "06897" }),
                    Is.EqualTo(1));
            }
        }

        [TestCase(TestName = "Related field, set the collection field with a new result.")]
        [Repeat(Repeat)]
        public void Session_GetById_With_Setted_Territories()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                employee.Territories = session.GetAll<EmployeeTerritories>();

                var territories = employee.Territories;

                Assert.That(territories.Count(), Is.EqualTo(49));
            }
        }

        [TestCase(TestName = "Related field, reuse it.")]
        [Repeat(Repeat)]
        public void Session_Reuse_Returned_Entity()
        {
            var session = Session;
            {
                var employee = session.GetById(new Employee { EmployeeId = 1 });

                var reportsTo = session.GetById(employee.ReportsTo);

                Assert.That(reportsTo, Is.Not.Null);
                EntitiesAsserts.Assert_Employee_2(reportsTo);
            }
        }
    }
}