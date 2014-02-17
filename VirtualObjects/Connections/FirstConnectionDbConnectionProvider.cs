using System.Configuration;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class FirstConnectionDbConnectionProvider : NamedDbConnectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstConnectionDbConnectionProvider"/> class.
        /// </summary>
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