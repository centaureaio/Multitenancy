using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System;
using Centaurea.Multitenancy.Annotation;
using Microsoft.AspNetCore.Http;

namespace Centaurea.Multitenancy.Test
{
    public class TenantedServicesdAddOrGetTest : BaseTest
    {
        interface IFake
        {
        };

        class Fake : IFake
        {
        };

        class TenantFake : IFake
        {
        };

        class Dep
        {
            public ITenantResolver res;
            public IServiceProvider prov;

            public Dep(IFake fake, ITenantResolver resolver, IServiceProvider provider)
            {
                Faked = fake;
                res = resolver;
                prov = provider;
            }

            public IFake Faked { get; set; }
        }

        private IServiceCollection _services;

        public TenantedServicesdAddOrGetTest()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void TestMultitenancyDepsAdding()
        {
            _services.ActivateMultitenancy(DefaultConfig, (p) => null);
            Assert.NotEmpty(_services);
        }

        [Fact]
        public void AddTenantScopedDep()
        {
            _services.AddScoped<Fake>();
            Assert.Equal(1, _services.Count);

            _services.AddScopedForTenant<Fake>(new TenantId("tenant1"));
            Assert.Equal(2, _services.Count);
        }

        [Fact]
        public void AddingDepToDefaultTenantRewriteRegular()
        {
            _services.AddScoped<IFake, Fake>();
            _services.AddScopedForTenant<IFake, TenantFake>(new TenantId());
            _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            IServiceProvider provider = _services.BuildMultitenantServiceProvider(DefaultConfig);
            IFake result = provider.GetRequiredService<IFake>();
            Assert.Equal(typeof(TenantFake), result.GetType());
        }

        [Fact]
        public void AddTenantDependentAndIndependentDepsWorksFine()
        {
            _services.AddScoped<IFake, Fake>();
            _services.AddScopedForTenant<IFake, TenantFake>(new TenantId("test"));

            Assert.Equal(2, _services.Count);
        }

        [Fact]
        public async void ResolveTenantScopedService()
        {
            IServiceProvider serviceProvider = null;
            string ya = "yahoo";
            _services.AddScoped<IFake, Fake>();
            _services.AddScoped<Fake>();
            _services.AddScoped<Dep>();
            _services.AddScopedForTenant<IFake, TenantFake>(new TenantId(ya));
            Mock<IHttpContextAccessor> accessor = new Mock<IHttpContextAccessor>();
            _services.AddSingleton(accessor.Object);
//            _services.AddSingleton<IServiceProvider>((p1) => serviceProvider);
            serviceProvider = _services.BuildMultitenantServiceProvider(GetTenantConfiguration(mapps: (ya, ya)));

            await EmulateRequestExecution(accessor, "google.com", ya, ya);
            IFake service = serviceProvider.GetService<IFake>();
            Dep dep = serviceProvider.GetService<Dep>();
            Assert.Equal(typeof(DefaultTenantResolver), dep.res.GetType());

            var fact = serviceProvider.GetService<IServiceScopeFactory>();
      
            using(var scope = serviceProvider.CreateScope())
            {
                Assert.Equal(typeof(MultitenantServiceScopeProxy), scope.GetType());
                service = scope.ServiceProvider.GetService<IFake>();
                Assert.Equal(typeof(Fake), service.GetType());
                dep = scope.ServiceProvider.GetService<Dep>();
                Assert.Equal(typeof(Fake), dep.Faked.GetType());
                //Assert.Equal(typeof(MultitenantServiceProvider), dep.prov.GetType());
            }

            await EmulateRequestExecution(accessor, "yahoo.com", ya, "yahoo");
            using(var scope = serviceProvider.CreateScope()){
                service = scope.ServiceProvider.GetService<IFake>();
                Fake notOverridenByScopeService = serviceProvider.GetService<Fake>();
                Assert.Equal(typeof(TenantFake), service.GetType());
                Assert.NotNull(notOverridenByScopeService);
                dep =  scope.ServiceProvider.GetService<Dep>();
                Assert.Equal(typeof(TenantFake), dep.Faked.GetType());

                using (var nestedScope = scope.ServiceProvider.CreateScope())
                {
                    Assert.Equal(typeof(MultitenantServiceScopeProxy), nestedScope.GetType());
                    service = nestedScope.ServiceProvider.GetService<IFake>();
                    Assert.Equal(typeof(TenantFake), service.GetType());
                }
            }
        }
         

        //TODO:Add unit tests for transient and singleton registered services
    }
}