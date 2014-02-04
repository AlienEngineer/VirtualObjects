using Ninject;
using VirtualObjects.Core.Connection;

namespace VirtualObjects
{
    public interface IOcContainer
    {

        TResult Get<TResult>();

    }

    class NinjectContainer : IOcContainer
    {
        readonly IKernel _kernel;

        public NinjectContainer(SessionConfiguration configuration, IDbConnectionProvider connectionProvider)
        {
            configuration = configuration ?? new SessionConfiguration();

            if ( connectionProvider != null )
            {
                configuration.ConnectionProvider = connectionProvider;
            }

            _kernel = new StandardKernel(new VirtualObjectsModule(configuration));
        }

        public NinjectContainer(SessionConfiguration configuration, string connectionName)
            :this(configuration, new NamedDbConnectionProvider(connectionName))
        {
            
        }

        public NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public TResult Get<TResult>()
        {
            return _kernel.Get<TResult>();
        }
    }
}
