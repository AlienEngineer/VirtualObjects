using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.NonQueries;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.NonQueries
{
    public class NonQueriesSpecs : SpecsUtilityBelt
    {

    }

    [Subject(typeof (IUpdate<>))]
    public class When_creating_an_update_operation : NonQueriesSpecs
    {
        Because of =
            () =>
            {
                update = Session.Update<Employee>()
                            .Set(e => e.LastName, "")
                            .Where(c => c.Where(e => e.EmployeeId > 0));
                
            };

        It should_match =
            () => update.ToString()
                .Should()
                .Be("Update [Employee] Set [T0].[LastName] = @p0 From [Northwind].[dbo].[Employees] [T0] Where ([T0].[EmployeeId] > @p1)");

        private static INonQuery<Employee> update;
    }
}
