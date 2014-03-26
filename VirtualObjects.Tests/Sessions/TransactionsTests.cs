using System;
using FluentAssertions;
using Machine.Specifications;
using NUnit.Framework;

namespace VirtualObjects.Tests.Sessions
{
    public abstract class TransactionSpecs : UtilityBelt
    {
        protected static ISession session;
        protected static ITransaction transaction;
    }

    [TestFixture]
    [Tags("Transactions")]
    [Subject(typeof(ITransaction))]
    public class When_an_error_occurs_the_transaction : TransactionSpecs
    {
        Establish context = () => { session = new Session(connectionName: "northwind"); };

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





}
