using Ninject;

namespace VirtualObjects
{
    public interface IOcContainer
    {

        TResult Get<TResult>();

    }

    class NinjectContainer : IOcContainer
    {
        readonly IKernel _kernel;

        public NinjectContainer(SessionConfiguration configuration)
        {
            _kernel = new StandardKernel(new VirtualObjectsModule(configuration));
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
