using System;
using Ninject;
using VirtualObjects.Connections;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOcContainer
    {

        /// <summary>
        /// Gets an instance of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object Get(Type type);
        /// <summary>
        /// Gets an instance of TResult type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns></returns>
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

        public NinjectContainer(SessionConfiguration configuration)
        {
            _kernel = new StandardKernel(new VirtualObjectsModule(configuration));
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
