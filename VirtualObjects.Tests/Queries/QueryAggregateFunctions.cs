using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
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

        int _count;

        [TearDown]
        public void FlushTime()
        {
            if ( !TestContext.CurrentContext.Test.Properties.Contains("Repeat") )
            {
                return;
            }

            var times = (int)TestContext.CurrentContext.Test.Properties["Repeat"];

            _count++;

            if ( _count % times != 0 ) return;

            Diagnostic.PrintTime(TestContext.CurrentContext.Test.Name + " => Aggregate execution in time :   {1} ms");
        }

        [Test, Repeat(REPEAT)]
        public void Aggregate_Query_Count()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Count());

            count.Should().Be(9);
        }

        [Test, Repeat(REPEAT)]
        public void Aggregate_Query_PredicatedCount()
        {
            var count = Diagnostic.Timed(() => Query<Employee>().Count(e => e.EmployeeId > 5));

            count.Should().Be(4);
        }

        [Test, Repeat(REPEAT)]
        public void Aggregate_Query_Sum()
        {
            var sum = Diagnostic.Timed(() => Query<Employee>().Sum(e => e.EmployeeId));

            sum.Should().Be(45);
        }


        [Test, Repeat(REPEAT)]
        public void Aggregate_Query_Avg()
        {
            var avg = Diagnostic.Timed(() => Query<Employee>().Average(e => e.EmployeeId));

            avg.Should().Be(5);
        }

        [Test, Repeat(REPEAT)]
        public void Aggregate_Query_Avg()
        {
            var avg = Diagnostic.Timed(() => Query<Employee>().Average(e => e.EmployeeId));

            avg.Should().Be(5);
        }
    }
}
