using System;
using Microsoft.Extensions.DependencyInjection;

namespace Centaurea.Multitenancy
{
    internal class TenantedServiceDescriptor : ServiceDescriptor
    {
        public TenantedServiceDescriptor(TenantId tenant, Type serviceType, Type implementationType,
            ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        {
            Tenant = tenant;
        }

        public TenantedServiceDescriptor(TenantId tenant, Type serviceType, object instance) : base(serviceType,
            instance)
        {

            Tenant = tenant;
        }

        public TenantedServiceDescriptor(TenantId tenant, Type serviceType, Func<IServiceProvider, object> factory,
            ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
            Tenant = tenant;
        }

        internal TenantId Tenant { get; set; }
    }
}