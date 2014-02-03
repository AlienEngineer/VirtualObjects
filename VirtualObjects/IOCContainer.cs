using Ninject;

namespace VirtualObjects
{
    public interface IOcContainer
    {

        TResult Get<TResult>();

    }

    class NinjectContainer : IOcContainer
    {
        readonly IKernel _kernel = new StandardKernel(new VirtualObjectsModule());

        public TResult Get<TResult>()
        {
            return _kernel.Get<TResult>();
        }
    }
}
