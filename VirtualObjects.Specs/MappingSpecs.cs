using System;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Config;
using VirtualObjects.Tests;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Specs
{
    public class CustomEmployee : Employee
    {
        public int NewField { get; set; }
    }

    [Subject(typeof (IMapper))]
    public class When_Context : SpecsUtilityBelt
    {
        private Because of = () =>
        {
            exception = Catch.Exception(() => entityInfo = Mapper.Map<CustomEmployee>());
        };

        It should_not_fail = () => exception.Should().BeNull();

        It should_not_be_null = () => entityInfo.Should().NotBeNull();

        It should_have_newfield_at_first_position = () => entityInfo.Columns.First().ColumnName.Should().Be("NewField");

        private static IEntityInfo entityInfo;
        private static Exception exception;
    }
}
