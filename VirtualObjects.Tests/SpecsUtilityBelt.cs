using Machine.Specifications;
using VirtualObjects.Config;

namespace VirtualObjects.Tests
{
    public abstract class SpecsUtilityBelt
    {
        Establish context = () =>
                            {
                                var ioc = new NinjectContainer(new SessionConfiguration { }, "northwind");

                                //ConnectionManager = ioc.Get<IConnection>();
                                //Translator = ioc.Get<IQueryTranslator>();
                                //QueryProvider = ioc.Get<IQueryProvider>();
                                //SessionContext = ioc.Get<SessionContext>();

                                Session = new Session(ioc);

                                Mapper = ((InternalSession)Session.InternalSession).Mapper;
                            };

        protected static IMapper Mapper;
        protected static Session Session;
    }
}