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
    public class When_insert_record_within_transaction : TransactionSpecs
    {
        private const int NumberOfThreads = 100;
        private Because of = () =>
        {

            cdEvent = new CountdownEvent(NumberOfThreads);
            mrEventS = new ManualResetEventSlim();
            exceptions = new List<Exception>();
            tasks = new Task[NumberOfThreads];
            unsafeCount = 0;

            for (int i = 0; i < NumberOfThreads; i++)
            {
                var j = i+1;
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var session = new Session(connectionName: "northwind"))
                        {
                           // cdEvent.Signal(); // Signal threads ready.
                            mrEventS.Wait();  // Wait for all threads to be ready.
                            session.WithinTransaction(
                                transaction =>
                                {
                                    // Acquire lock.
                                    session.Insert(new DataLock { Id = 100 });
                                    locked.Should().BeFalse("Haven't acquired the lock, so it should be locked.");

                                    locked = true;

                                    unsafeCount++;
                                    Thread.Sleep(100);

                                    locked = false;
                                    // Release lock.
                                    session.Delete(new DataLock { Id = 100 });
                                    
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        exceptions.Add(ex);
                    }

                });
            }

            //cdEvent.Wait(); // Wait until all threads are in the same position.
            mrEventS.Set(); // Signal all threads to execute.
            Task.WaitAll(tasks);

        };

        private It should_not_fail = () => exceptions.Count.Should().Be(0);

        It should_have_counted_all_ocurrences_on_unsafe_count = () => unsafeCount.Should().Be(NumberOfThreads);

        private static ManualResetEventSlim mrEventS;
        private static Task[] tasks;
        private static List<Exception> exceptions;
        private static CountdownEvent cdEvent;
        private static int unsafeCount;
        private static volatile Boolean locked;
    }
}
