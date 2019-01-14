using System;
using Microsoft.Extensions.DependencyInjection;

namespace Centaurea.Multitenancy
{
    public static class MultitenantServicesExtension
    {
        public static void AddMultitenantDi(this IServiceCollection services)
        {
            services.AddSingleton<MultitenantTypeRegister>();
        }

        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services)
        {
            return new MultitenantServiceProvider(services.BuildServiceProvider());
        }
        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services, bool validateScopes)
        {
            return new MultitenantServiceProvider(services.BuildServiceProvider(validateScopes));
        }
        public static IServiceProvider BuildMultitenantServiceProvider(this IServiceCollection services, ServiceProviderOptions opts)
        {
            return new MultitenantServiceProvider(services.BuildServiceProvider(opts));
        }

        public static void AddScopedForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddScoped(tenantAdoptedType, serviceType);
        }

        public static void AddScopedForTenant(this IServiceCollection services, Type serviceType, Type implementationType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddScoped(tenantAdoptedType, implementationType);
        }

        public static void AddScopedForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddScopedForTenant(typeof(TService), tenantId);
        }

        public static void AddScopedForTenant<TService, TImplementation>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddScopedForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }

        public static void AddTransientForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddTransient(tenantAdoptedType, serviceType);
        }

        public static void AddTransientForTenant(this IServiceCollection services, Type serviceType, Type implementationType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddTransient(tenantAdoptedType, implementationType);
        }

        public static void AddTransientForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddTransientForTenant(typeof(TService), tenantId);
        }

        public static void AddTransientForTenant<TService, TImplementation>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddTransientForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }

        public static void AddSingletonForTenant(this IServiceCollection services, Type serviceType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddSingleton(tenantAdoptedType, serviceType);
        }

        public static void AddSingletonForTenant(this IServiceCollection services, Type serviceType, Type implementationType, TenantId tenantId)
        {
            Type tenantAdoptedType = MultitenantTypeDescriptor.AdoptToTenant(serviceType, tenantId);
            services.AddSingleton(tenantAdoptedType, implementationType);
        }

        public static void AddSingletonForTenant<TService>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddSingletonForTenant(typeof(TService), tenantId);
        }

        public static void AddSingletonForTenant<TService, TImplementation>(this IServiceCollection services, TenantId tenantId)
        {
            services.AddSingletonForTenant(typeof(TService), typeof(TImplementation), tenantId);
        }
    }
}
