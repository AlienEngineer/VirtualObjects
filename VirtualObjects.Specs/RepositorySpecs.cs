using System;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Specs.Repositories;

namespace VirtualObjects.Specs
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

        It should_be_equal_to_testing = () => connectionString.Should().Be("testing");
        
        static Repository testing;
        static String connectionString;
    }
}
