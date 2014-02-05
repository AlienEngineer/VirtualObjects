using System.Configuration;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    public class FirstConnectionDbConnectionProvider : NamedDbConnectionProvider
    {
        public FirstConnectionDbConnectionProvider()
            : base(GetFirstConnectionString())
        {

        }

        private static string GetFirstConnectionString()
        {
            var settings = ConfigurationManager.ConnectionStrings;
            
            if (settings.Count == 0)
            {
                throw new ConnectionProviderException(Errors.ConnectionProvider_NoConnectionConfigured);
            }

            return settings[0].Name;
        }
    }
}