using Machine.Specifications;
using VirtualObjects.Config;

namespace VirtualObjects.Tests
{
    public abstract class SpecsUtilityBelt
    {
        Establish context =
            () =>
            {
                Session = new Session(new SessionConfiguration(), "northwind");

                Mapper = ((InternalSession)Session.InternalSession).Mapper;
            };

        protected static IMapper Mapper;
        protected static Session Session;
    }
}