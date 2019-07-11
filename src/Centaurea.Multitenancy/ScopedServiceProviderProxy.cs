using System;

namespace Centaurea.Multitenancy
{
    class ScopedServiceProviderProxy : IServiceProvider
    {
        private IServiceProvider _provider;
        private IServiceProvider _scopedProvider;

        public ScopedServiceProviderProxy()
        {
        }

        public ScopedServiceProviderProxy(IServiceProvider provider, IServiceProvider scopedProxyProvider)
        {
            _provider = provider;
            _scopedProvider = scopedProxyProvider;
        }

        public object GetService(Type serviceType)
        {
            if (TypeHelper.IsServiceProvider(serviceType))
            {
                return this;
            }

            return TypeHelper.IsServiceExcluded(serviceType) ? _provider.GetService(serviceType) : _scopedProvider
                .GetService(serviceType);
        }
    }
}
