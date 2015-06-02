using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Connections;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Tests.Connection
{
    /// <summary>
    /// 
    /// Connection Provider Unit-Tests
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Connection Providers")]
    public class ConnectionProviderTests : UtilityBelt
    {

        [Test]
        public void NamedDbConnectionProviderTest()
        {

            var provider = new NamedDbConnectionProvider("northwind");

            var connection = provider.CreateConnection();
            
            connection.Should().NotBeNull();

            connection.Open();
            connection.Close();
        }

        [Test, ExpectedException(typeof(ConnectionProviderException))]
        public void NamedDbConnectionProvider_NonExisting_Test()
        {

            var provider = new NamedDbConnectionProvider("northwind1");

            var connection = provider.CreateConnection();
        }
    }
}
