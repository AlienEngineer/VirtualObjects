using System;
using Ninject;
using VirtualObjects.Connections;

namespace VirtualObjects
{
    public interface IOcContainer
    {

        object Get(Type type);
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

        public object Get(Type type)
        {
            return _kernel.Get(type);
        }

        public TResult Get<TResult>()
        {
            return _kernel.Get<TResult>();
        }
    }
}
