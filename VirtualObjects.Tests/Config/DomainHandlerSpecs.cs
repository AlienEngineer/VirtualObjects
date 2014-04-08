using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Domains;

namespace VirtualObjects.Tests.Config
{
    [Subject(typeof(DomainHandler)), Tags("AppDomain")]
    public class When_creating_a_new_AppDomain
    {
        Establish context = () => { domain = new DomainHandler(""); };

        Because of = () => domain.LoadAssemblies();

        Cleanup after = () => domain.Unload();


        It should_not_be_null = () => domain.Should().NotBeNull();
        It should_be_loaded = () => domain.IsLoaded.Should().BeTrue();
        It should_have_assemblies_loaded = () => domain.Assemblies.Count.Should().Be(12);

        static DomainHandler domain;
    }
}
