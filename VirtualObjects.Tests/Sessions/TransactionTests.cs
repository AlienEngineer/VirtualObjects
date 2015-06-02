using System;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    [TestFixture, Category("Session")]
    public class TransactionTests : UtilityBelt
    {
        private static void ThrowExceptionWithinTransaction(ISession safeSession)
        {
            safeSession.WithinTransaction(transaction =>
            {
                safeSession.Update(new Employee
                {
                    EmployeeId = 1,
                    LastName = "Ferreira",
                    FirstName = "Sérgio",
                    Version = BitConverter.GetBytes(long.MaxValue)
                });

                safeSession.Insert(new Employee
                {
                    EmployeeId = 1
                });
            });
        }

        /// <summary>
        ///
        /// Assert that we can reuse the session object even after an exception and a rollback.
        ///
        /// Flow:
        /// Make some invalid operation x times
        /// Create a new operation and rollback
        ///
        /// Author: Sérgio Ferreira
        ///
        /// </summary>
        [Test]
        public void Transaction_Reusing_Session()
        {
            using ( var session = new Session(connectionName: "northwind") )
            {
                for ( var i = 0; i < 10; i++ )
                {
                    try
                    {
                        ThrowExceptionWithinTransaction(session);
                    }
                    catch ( Exception ex )
                    {
                        StringAssert.Contains("Cannot insert the value NULL into column 'LastName'",
                                              ex.Message);
                    }
                }

                var transaction = session.BeginTransaction();

                session.Update(new Employee
                {
                    EmployeeId = 1,
                    LastName = "Ferreira",
                    FirstName = "Sérgio",
                    Version = BitConverter.GetBytes(long.MaxValue)
                });

                transaction.Rollback();
            }
        }

        /// <summary>
        ///
        /// Assertion to make sure the transaction is rolledback when an exception is throwned within a transaction.
        ///
        /// Flow:
        /// Change employee 1
        /// Throw exception -> Should rollback
        /// Assert database employee 1
        ///
        /// Data: 19-06-2013
        /// Author: Sérgio Ferreira
        ///
        /// </summary>
        [Test]
        public void Transaction_Rollback_On_Exception()
        {
            var session = new Session(connectionName: "northwind");

            try
            {
                session.WithinTransaction(transaction =>
                {
                    session.Update(new Employee
                    {
                        EmployeeId = 1,
                        LastName = "Ferreira",
                        FirstName = "Sérgio",
                        Version = BitConverter.GetBytes(long.MaxValue)
                    });

                    throw new Exception("Forced Exception");
                });
            }
            catch ( Exception ex )
            {
                Assert.That(ex.Message, Is.EqualTo("Forced Exception"));
            }

            var employee = session.GetById(new Employee { EmployeeId = 1 });

            Assert.AreEqual("Nancy", employee.FirstName);
            Assert.AreEqual("Davolio", employee.LastName);

            Assert.That(employee.LastName, Is.Not.EqualTo("Ferreira"));
            Assert.That(employee.FirstName, Is.Not.EqualTo("Sérgio"));
        }
        
        /// <summary>
        ///
        /// Assert that the rollback takes afect using the Employee Count.
        ///
        /// Author: Sérgio Ferreira
        ///
        /// </summary>
        [Test]
        public void Transaction_Rollback_Test()
        {
            using ( var session = new Session(connectionName: "northwind") )
            {
                var count = session.GetAll<Employee>().Count();
                using ( var trans = session.BeginTransaction() )
                {
                    session.Insert(new Employee
                    {
                        LastName = "asd",
                        FirstName = "asdad"
                    });

                    Assert.That(session.GetAll<Employee>().Count(), Is.EqualTo(count + 1));

                    trans.Rollback();
                }

                Assert.That(session.GetAll<Employee>().Count(), Is.EqualTo(count));
            }
        }
    }
}
