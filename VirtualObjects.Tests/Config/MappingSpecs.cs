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
}
