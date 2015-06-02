using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Tests.Repositories;

namespace VirtualObjects.Tests.Sessions
{
    [Tags("Repository")]
    public abstract class RepositorySpecs
    {

    }

    [Subject(typeof (IRepository))]
    public class When_creating_a_new_repository_with_named_connection : RepositorySpecs
    {
        Establish context = () =>
        {
            testing = new Repository("TESTING");
        };

        Because of = () =>
        {
            var session = (Session) testing.Session;
            connectionString = ((InternalSession)session.InternalSession).Context.Connection.DbConnection.ConnectionString;
        };

        It should_be_equal_to_testing = () => connectionString.Should().Be("Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\northwnd.mdf;Integrated Security=True;Connect Timeout=30");
        
        static Repository testing;
        static string connectionString;
    }

    [Subject(typeof (IRepository))]
    public class When_creating_a_repository_from_another : RepositorySpecs
    {
        Establish context = () =>
        {
            testing = new Repository("TESTING");
            
        };

        private Because of = () =>
        {
            northwind = testing.CreateNewRepository("NORTHWIND");
            var session = (Session)((Repository)northwind).Session;
            connectionString = session.ConnectionString;
        };

        private It should_be_equal_to_northwind = () => connectionString.Should().Be("                    Data Source=(LocalDB)\\v11.0;                                                         AttachDbFilename=|DataDirectory|\\northwnd.mdf;                                                         Integrated Security=True;                                                         Connect Timeout=60;          MultipleActiveResultSets=True");
        
        private static IRepository northwind;
        private static IRepository testing;
        private static string connectionString;
    }
}
