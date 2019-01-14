using System;
using Microsoft.Extensions.DependencyInjection;

namespace Centaurea.Multitenancy
{
    public class MultitenantServiceProvider : IServiceProvider
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly ServiceProvider _provider;
        private static readonly Type _resolverType = typeof(ITenantResolver);

        public MultitenantServiceProvider(ServiceProvider provider)
        {
            _tenantResolver = (ITenantResolver)provider.GetService(_resolverType);
            _provider = provider;
        }

        public object GetService(Type serviceType)
        {
            Type typeToResolve = MultitenantTypeDescriptor.AdoptToTenant(serviceType, _tenantResolver.Current);
            return _provider.GetService(typeToResolve ?? serviceType);
        }
        
    }
}
