using System.Linq;
using FluentAssertions;
using VirtualObjects.Exceptions;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;

    /// <summary>
    /// 
    /// Unit tests for aggregation functions.
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Query Aggregate Functions")]
    public class QueryAggregateFunctionsTests : UtilityBelt
    {
        
        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Count()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Count());

            count.Should().Be(9);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_PredicatedCount()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Count(e => e.EmployeeId > 5));

            count.Should().Be(4);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Contains()
        {
            var employee = new Employee { EmployeeId = 1 };

            var count = Diagnostic.Timed(() => Query<Employee>().Contains(employee));

            count.Should().BeTrue();
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Contains_MultipleKeys()
        {

            var employeeTerritory = new EmployeeTerritories
            {
                Employee = new Employee { EmployeeId = 1 },
                Territories = new Territories { TerritoryId = "06897" }
            };

            var count = Diagnostic.Timed(() => Query<EmployeeTerritories>().Contains(employeeTerritory));

            count.Should().BeTrue();
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Contains_False()
        {
            var employee = new Employee { EmployeeId = 50 };

            var count = Diagnostic.Timed(() => Query<Employee>().Contains(employee));

            count.Should().BeFalse();
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Any()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Any());

            count.Should().BeTrue();
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_PredicatedAny()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Any(e => e.EmployeeId > 5));

            count.Should().BeTrue();
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_PredicatedAny_False()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Any(e => e.EmployeeId == 50));

            count.Should().BeFalse();
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Sum()
        {
            var sum = Diagnostic.Timed(() => Query<Employee>().Sum(e => e.EmployeeId));

            sum.Should().Be(45);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Avg()
        {
            var avg = Diagnostic.Timed(() => Query<Employee>().Average(e => e.EmployeeId));

            avg.Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_LongCount()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().LongCount());

            count.Should().Be(9);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_PredicatedLongCount()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().LongCount());

            count.Should().Be(9);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Max()
        {
            var max = Diagnostic.Timed(() => Query<Employee>().Max(e => e.EmployeeId));

            max.Should().Be(9);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Min()
        {
            var min = Diagnostic.Timed(() => Query<Employee>().Min(e => e.EmployeeId));

            min.Should().Be(1);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_First()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().First());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_First_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().First(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_FirstOrDefault()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().FirstOrDefault());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_FirstOrDefault_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().FirstOrDefault(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Single()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Single());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Single_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Single(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_SingleOrDefault()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().SingleOrDefault());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_SingleOrDefault_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().SingleOrDefault(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }


        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Aggregate_Query_Min_Entity()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Min());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }

        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Aggregate_Query_Max_Entity()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Max());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(9);
        }


        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Aggregate_Query_GroupBy_Unsupported()
        {
            Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Employees = e })
                    .ToList());
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>().ToList()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Employees = e })
                    .ToList());


            // Should this be supported?! This will be done locally... 
            // Support this if there is any way to group on the SQL Server side.

            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_Sum()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Sum = e.Sum(o => o.EmployeeId) })
                    .ToList());

            // Select [T0].[City], Sum([T0].[EmployeeId]) as N'Sum' from Employees [T0] Group By [T0].[City]
            

            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
            employee.First().City.Should().Be("Kirkland");
            employee.First().Sum.Should().Be(3);
        }



        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_Avg()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Average = e.Average(o => o.EmployeeId) })
                    .ToList());

            // Select [T0].[City], Avg([T0].[EmployeeId]) as Average from Employees [T0] Group By [T0].[City]


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_Max()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Max = e.Max(o => o.EmployeeId) })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_Min()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Min = e.Min(o => o.EmployeeId) })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_Count()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Count = e.Count() })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }


        [Test, Repeat(Repeat), ExpectedException(typeof(TranslationException))]
        public void Aggregate_Query_GroupBy_With_PredicatedCount_OnProjection_Unsupported()
        {
            Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Count = e.Count(o => o.EmployeeId == 1) })
                    .ToList());

        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_GroupBy_With_LongCount()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { City = e.Key, Count = e.LongCount() })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_ManyAggreagates()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new { 
                        City = e.Key, 
                        Min = e.Min(o => o.EmployeeId),
                        Max = e.Max(o => o.EmployeeId),
                        Sum = e.Sum(o => o.EmployeeId),
                        Count = e.Count(),
                        Average = e.Average(o => o.EmployeeId)
                    })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }



        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Calced_Average()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new
                    {
                        City = e.Key,
                        Sum = e.Sum(o => o.EmployeeId),
                        Count = e.Count(),
                        Average = e.Sum(o => o.EmployeeId) / e.Count()
                    })
                    .ToList());


            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Calced_With_Factor()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>()
                    .GroupBy(e => e.City)
                    .Select(e => new
                    {
                        City = e.Key,
                        Sum = e.Sum(o => o.EmployeeId),
                        Count = e.Count(),
                        Average = e.Sum(o => o.EmployeeId) / (e.Count()*1.0) * 100.0
                    })
                    .ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(5);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Distinct()
        {
            var employee = Diagnostic.Timed(() => 
                Query<Employee>()
                    .Distinct()
                    .Select(e => new { e.City, e.TitleOfCourtesy })
                    .ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(6);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Last()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Last());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(9);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Last_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().Last(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_LastOrDefault()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().LastOrDefault());

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(9);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_LastOrDefault_Predicated()
        {
            var employee = Diagnostic.Timed(() => Query<Employee>().LastOrDefault(e => e.EmployeeId == 2));

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(2);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union()
        {
            var employee = Diagnostic.Timed(() => 
                Query<Employee>().Select(e => new { e.EmployeeId }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId })).ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(18);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Multiple_Union()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId })).Union(
                Query<Employee>().Select(e => new { e.EmployeeId })).ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(27);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Count()
        {
            var count = Diagnostic.Timed(() =>
                Query<Employee>().Union(
                Query<Employee>()).Count());

            count.Should().Be(18);
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Count()
        {
            var count = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Count());

            count.Should().Be(18);
        }
        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Any()
        {
            var any = Diagnostic.Timed(() =>
                Query<Employee>().Union(
                Query<Employee>()).Any());

            any.Should().BeTrue();
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Any()
        {
            var any = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Any());

            any.Should().BeTrue();
        }


        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Min()
        {
            var min = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Min(e => e.EmployeeId));

            min.Should().Be(1);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Max()
        {
            var max = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Max(e => e.EmployeeId));

            max.Should().Be(9);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Sum()
        {
            var max = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Sum(e => e.EmployeeId));

            max.Should().Be(90);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Projected_Average()
        {
            var max = Diagnostic.Timed(() =>
                Query<Employee>().Select(e => new { e.EmployeeId, e.City }).Union(
                Query<Employee>().Select(e => new { e.EmployeeId, e.City })).Average(e => e.EmployeeId));

            max.Should().Be(5);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_Union_Predicated()
        {
            var employee = Diagnostic.Timed(() =>
                Query<Employee>().Where(e=> e.EmployeeId > 5).Select(e => new { e.EmployeeId }).Union(
                Query<Employee>().Where(e => e.EmployeeId <= 5).Select(e => new { e.EmployeeId })).ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(9);
        }

        [Test, Repeat(Repeat)]
        public void Aggregate_Query_YearOfField_Projection()
        {
            
            var employee = Diagnostic.Timed(() => Query<Employee>()
                .Select(e => new { e.BirthDate.Year })
                .ToList());

            employee.Should().NotBeNull();
            employee.Count().Should().Be(9);
        }

    }
}