using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;
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

    [Subject(typeof(ITransaction))]
    public class When_concurrently_couting_using_a_Mutex_lock : TransactionSpecs
    {
        private const int NumberOfThreads = 100;
        private Because of = () =>
        {

            _cdEvent = new CountdownEvent(NumberOfThreads);
            _mrEventS = new ManualResetEventSlim();
            _exceptions = new List<Exception>();
            _tasks = new Task[NumberOfThreads];
            _unsafeCount = 0;

            for (int i = 0; i < NumberOfThreads; i++)
            {
                var j = i + 1;
                _tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var session = new Session(connectionName: "northwind"))
                        {
                            session.WithinTransaction(
                                trans =>
                                {
                                    var stopwatch = new Stopwatch();
                                    stopwatch.Start();

                                    trans.AcquireLock("MyResourceName").Should().BeTrue("The lock should be acquired");

                                    stopwatch.Stop();
                                    Console.WriteLine("Locked for {0} ms", stopwatch.ElapsedMilliseconds);
                                    
                                    var value = _unsafeCount;

                                    Thread.Sleep(20);

                                    _unsafeCount = value + 1;
                                });


                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        _exceptions.Add(ex);
                    }

                }, TaskCreationOptions.LongRunning);
            }

            Task.WaitAll(_tasks);

        };

        private It should_not_fail = () => _exceptions.Count.Should().Be(0);

        It should_have_counted_all_ocurrences_on_unsafe_count = () => _unsafeCount.Should().Be(NumberOfThreads);

        private static ManualResetEventSlim _mrEventS;
        private static Task[] _tasks;
        private static List<Exception> _exceptions;
        private static CountdownEvent _cdEvent;
        private static int _unsafeCount;
    }
}
