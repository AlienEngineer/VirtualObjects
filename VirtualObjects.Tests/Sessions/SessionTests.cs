using NUnit.Framework;

namespace VirtualObjects.Tests.Sessions
{
    /// <summary>
    /// 
    /// Unit-Tests for session. 
    /// This will also test the IOC Configuration and all dependencies.
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Session")]
    public class SessionTests : TimedTests
    {

        /// <summary>
        /// 
        /// Session creation for ioc tests.
        /// 
        /// </summary>
        [Test, Repeat(Repeat)]
        public void Session_Should_Be_Created_And_Disposed()
        {

            using (var session = new Session(connectionName: "northwind"))
            {

            }

        }


        [Test, Repeat(Repeat)]
        public void Session_Transaction_Should_Be_Created_And_Disposed()
        {

            using ( var session = new Session(connectionName: "northwind") )
            {
                using (var transaction = session.BeginTransaction())
                {
                    
                    
                }
            }

        }

    }
}
