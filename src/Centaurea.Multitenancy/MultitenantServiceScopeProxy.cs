using System;
using Microsoft.Extensions.DependencyInjection;

namespace Centaurea.Multitenancy
{
    internal class MultitenantServiceScopeProxy : IServiceScope
    {
        private IServiceScope _scope;

        public MultitenantServiceScopeProxy(IServiceScope scope, IServiceProvider provider)
        {
            _scope = scope;
            ServiceProvider = new ScopedServiceProviderProxy(provider, scope.ServiceProvider);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
