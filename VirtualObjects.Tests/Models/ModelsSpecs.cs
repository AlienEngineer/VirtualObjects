using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Models
{
    [Tags("Models")]
    public class ModelsSpecs : SpecsUtilityBelt
    {
        Establish context = () =>
        {
            Console.WriteLine("This is the second context.");
        };
    }

    [Subject(typeof (Employee))]
    public class When_loading_a_collection_from_employee_model : ModelsSpecs
    {
        Establish context = () =>
        {
            Console.WriteLine("This is the third context.");
            employee = Session.GetById(new Employee { EmployeeId = 1 });
        };

        private Because of = () =>
        {
            territories = employee.TerritoriesSimplified.ToList();
        };

        private It should_have_more_than_0_territories = () => territories.Count.Should().BeGreaterThan(0);

        private static List<EmployeeTerritoriesSimplified> territories;
        private static Employee employee;
    }
}
