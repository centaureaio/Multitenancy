using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Centaurea.Multitenancy
{
    public static class MultitenantServicesExtension
    {
        internal static void ActivateMultitenancy(this IServiceCollection services)
        {
            services.AddSingleton<ITenantResolver, DefaultTenantResolver>();
            services.Remove(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
            services.Remove(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>)));
            
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(TenantOptionsManager<>)));
            services.TryAdd(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(TenantOptionsManager<>)));
        }

        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services)
        {
            return new MultitenantServiceProvider(MultitenantServiceProvider.InitProviderCollections(services)
                .ToDictionary(kv => kv.Key, kv => (IServiceProvider) kv.Value.BuildServiceProvider()));
        }

        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services,
            bool validateScopes)
        {
            return new MultitenantServiceProvider(MultitenantServiceProvider.InitProviderCollections(services)
                .ToDictionary(kv => kv.Key, kv => (IServiceProvider) kv.Value.BuildServiceProvider(validateScopes)));
        }

        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services,
            ServiceProviderOptions opts)
        {
            return new MultitenantServiceProvider(MultitenantServiceProvider.InitProviderCollections(services)
                .ToDictionary(kv => kv.Key, kv => (IServiceProvider) kv.Value.BuildServiceProvider(opts)));
        }

        public static void AddScopedForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, serviceType, ServiceLifetime.Scoped));
        }

        public static void AddScopedForTenant(this IServiceCollection services, Type serviceType,
            Type implementationType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, implementationType,
                ServiceLifetime.Scoped));
        }

        public static void AddScopedForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddScopedForTenant(typeof(TService), tenantId);
        }

        public static void AddScopedForTenant<TService, TImplementation>(this IServiceCollection services,
            TenantId tenantId)
        {
            services.AddScopedForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }

        public static void AddTransientForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, serviceType, ServiceLifetime.Transient));
        }

        public static void AddTransientForTenant(this IServiceCollection services, Type serviceType,
            Type implementationType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, implementationType,
                ServiceLifetime.Transient));
        }

        public static void AddTransientForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddTransientForTenant(typeof(TService), tenantId);
        }

        public static void AddTransientForTenant<TService, TImplementation>(this IServiceCollection services,
            TenantId tenantId)
        {
            services.AddTransientForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }

        public static void AddSingletonForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, serviceType, ServiceLifetime.Singleton));
        }

        public static void AddSingletonForTenant(this IServiceCollection services, Type serviceType,
            Type implementationType, TenantId tenantId)
        {
            services.Add(new TenantedServiceDescriptor(tenantId, serviceType, implementationType,
                ServiceLifetime.Singleton));
        }

        public static void AddSingletonForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddSingletonForTenant(typeof(TService), tenantId);
        }

        public static void AddSingletonForTenant<TService, TImplementation>(this IServiceCollection services,
            TenantId tenantId)
        {
            services.AddSingletonForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }

        public static void ConfigureTenants(this IServiceCollection services, IConfiguration config, string key = Constants.TENANT_CONF_KEY)
        {
            services.AddSingleton(new TenantConfig{Config = config.GetSection(key)});
        }
    }
}