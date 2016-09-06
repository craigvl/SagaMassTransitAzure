using Nancy.Bootstrappers.Ninject;
using Ninject;

namespace Abt.Result.WebApi.Infrastructure
{
    public class NinjectNancyAppBootstrapper : NinjectNancyBootstrapper
    {
        private readonly IKernel _container;

        public NinjectNancyAppBootstrapper(IKernel existingContainer)
        {
            _container = existingContainer;
        }

        protected override IKernel GetApplicationContainer()
        {
            _container.Load<FactoryModule>();

            return _container;
        }

    }
}
