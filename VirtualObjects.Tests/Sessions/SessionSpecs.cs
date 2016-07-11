using System.Data;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    [Tags("Session")]
    public abstract class SessionSpecs : UtilityBelt
    {
        Establish context = () =>
                            {
                                session = new Session(connectionName: "northwind");
                                InternalSession = (session as Session).InternalSession as InternalSession;
                            };

        protected static ISession session;
        protected static ITransaction transaction;
        protected static InternalSession InternalSession;
    }

    [Subject(typeof (ISession))]
    public class When_executing_under_keepalive_protection : SessionSpecs
    {

        private Because of =
            () => session.KeepAlive(
                () =>
                {
                    var c1 = session.GetAll<Employee>().Count();

                    InternalSession.Context.Connection.KeepAlive.Should().BeTrue();

                    var c2 = session.GetAll<OrderDetails>().Count();
                });

        It should_close_the_connection_after_operations = 
            () => InternalSession.Context.Connection.DbConnection.State.Should().NotBe(ConnectionState.Open);

        It should_have_keepalive_flag_set_to_false =
            () => InternalSession.Context.Connection.KeepAlive.Should().BeFalse();
    }
    
}