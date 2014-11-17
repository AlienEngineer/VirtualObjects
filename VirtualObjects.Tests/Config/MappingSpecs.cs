using System;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Config;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Config
{
    [Tags("Mapping")]
    public abstract class MappingSpecs : SpecsUtilityBelt
    {
        
    }

    [Subject(typeof (IMapper))]
    public class When_mapping_a_derived_type : MappingSpecs
    {
        Establish context = () => {  };

        private Because of = () =>
        {
            EntityInfo = Mapper.Map(typeof (DerivedType));
        };

        It should_not_be_null = () => EntityInfo.Should().NotBeNull();

        private static IEntityInfo EntityInfo;
    }

    public class DerivedType : Employee
    {

    }

    public class CustomEmployee : Employee
    {
        public int NewField { get; set; }
    }

    [Subject(typeof(IMapper))]
    [Tags("Entity Mapping")]
    public class When_mapping_a_derived_entity : SpecsUtilityBelt
    {
        private Because of = () =>
        {
            exception = Catch.Exception(() => entityInfo = Mapper.Map<CustomEmployee>());
        };

        It should_not_fail = () => exception.Should().BeNull();

        It should_not_be_null = () => entityInfo.Should().NotBeNull();

        It should_have_newfield_at_last_position = () => entityInfo.Columns.Last().ColumnName.Should().Be("NewField");

        It should_have_20_columns = () => entityInfo.Columns.Count.Should().Be(20);

        private static IEntityInfo entityInfo;
        private static Exception exception;
    }
}
