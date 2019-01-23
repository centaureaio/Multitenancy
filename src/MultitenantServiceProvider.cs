using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Centaurea.Multitenancy
{
    public class MultitenantServiceProvider : IServiceProvider
    {
        private readonly ITenantResolver _tenantResolver;

        private readonly Dictionary<TenantId, IServiceProvider> _providers;

        internal MultitenantServiceProvider(Dictionary<TenantId, IServiceProvider> providers)
        {
            if (providers is null || !providers.ContainsKey(TenantId.DEFAULT_ID) || providers[TenantId.DEFAULT_ID] is null)
            {
                throw new ArgumentException("Incorrect configuration for provider. Default provider should be configured.");
            }

            _providers = providers;
            _tenantResolver = (ITenantResolver)_providers[TenantId.DEFAULT_ID].GetService(typeof(ITenantResolver));
        }

        public object GetService(Type serviceType) => (_providers.ContainsKey(_tenantResolver.Current) ?
            _providers[_tenantResolver.Current] : _providers[TenantId.DEFAULT_ID]).GetService(serviceType);

        internal static Dictionary<TenantId, IServiceCollection> InitProviderCollections(IServiceCollection services)
        {
            Dictionary<TenantId, IServiceCollection> result = new Dictionary<TenantId, IServiceCollection>();
            IEnumerable<TenantedServiceDescriptor> tenantedServices = services.Where(d => d is TenantedServiceDescriptor td && !td.Tenant.Equals(TenantId.DEFAULT_ID))
                .Cast<TenantedServiceDescriptor>().ToArray();

            foreach (TenantedServiceDescriptor descriptor in tenantedServices)
            {
                services.Remove(descriptor);
            }

            result.Add(TenantId.DEFAULT_ID, services);

            foreach (IGrouping<TenantId, TenantedServiceDescriptor> grouping in tenantedServices.ToLookup(d => d.Tenant))
            {
                IServiceCollection collection = (IServiceCollection) Activator.CreateInstance(services.GetType());

                foreach (ServiceDescriptor serviceDescriptor in services)
                {
                    collection.Add(serviceDescriptor);
                }

                foreach (TenantedServiceDescriptor serviceDescriptor in grouping)
                {
                    collection.Replace(serviceDescriptor);
                }

                result.Add(grouping.Key, collection);
            }
            
            return result;
        }
    }
}
