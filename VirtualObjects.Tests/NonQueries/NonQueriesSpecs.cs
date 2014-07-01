using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.NonQueries;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.NonQueries
{
    [Tags("NonQueries")]
    public class NonQueriesSpecs : SpecsUtilityBelt
    {

    }

    [Subject(typeof (IUpdate<>))]
    public class When_creating_a_simple_employees_update_operation : NonQueriesSpecs
    {
        Because of =
            () =>
            {
                update = Session.Update<Employee>()
                            .Set(e => e.LastName, "")
                            .Where(e => e.EmployeeId > 0);
            };

        It should_translate_to =
            () => update.ToString()
                .Should()
                .Be("Update [Employees] Set [LastName] = @p1 From [Northwind].[dbo].[Employees] [T0] Where ([T0].[EmployeeId] > @p0)");

        private static INonQuery<Employee> update;
    }
}
