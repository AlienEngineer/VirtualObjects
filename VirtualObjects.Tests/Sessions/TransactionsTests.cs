using System;
using FluentAssertions;
using Machine.Specifications;
using NUnit.Framework;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    [Tags("Transactions")]
    public abstract class TransactionSpecs : UtilityBelt
    {
        Establish context = () => { session = new Session(connectionName: "northwind"); };

        protected static ISession session;
        protected static ITransaction transaction;
    }

    [Subject(typeof(ITransaction))]
    public class When_an_error_occurs_the_transaction : TransactionSpecs
    {
        private Because of =
            () =>
            {
                Exception = Catch.Exception(
                    () => session.WithinTransaction(
                        trans =>
                        {
                            transaction = trans;
                            throw new Exception("forced error.");
                        }));
            };



        private It should_fail =
            () => Exception.Should().NotBeNull();

        private static Exception Exception;

        It should_rollback =
            () => transaction.Rolledback.Should().BeTrue();
    }



    [Subject(typeof(ITransaction))]
    public class When_rolling_back : TransactionSpecs
    {

        Because of = 
            () => session.WithRollback(() =>
            {
                sergio = session.Insert(new Employee { FirstName = "Sérgio", LastName = "Ferreira" });
            });

        It should_not_exist_after_rollback = 
            () => session.Exists(sergio).Should().BeFalse();

        static Employee sergio;
    }

    [Subject(typeof(ITransaction))]
    public class When_rolling_back_with_exception : TransactionSpecs
    {

        Because of = 
            () => session.WithRollback(() =>
            {
                sergio = session.Insert(new Employee { FirstName = "Sérgio", LastName = "Ferreira" });
                Exception = Catch.Exception(() => 
                {
                    throw new Exception("forced error.");
                });
            });

        It should_not_exist_after_rollback = 
            () => session.Exists(sergio).Should().BeFalse();

        private It should_fail =
            () => Exception.Should().NotBeNull();

        private static Exception Exception;

        static Employee sergio;
    }
}
