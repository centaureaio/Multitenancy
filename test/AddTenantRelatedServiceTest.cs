using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Centaurea.Multitenancy.Test
{
    public class AddTenantRelatedServiceTest : BaseTest
    {
        interface IFake { };
        class Fake : IFake { };

        class FakeChild : Fake { };

        class TenantFake : IFake { };
        private IServiceCollection _services;

        private TenantId testTenant = new TenantId("test");

        public AddTenantRelatedServiceTest()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void TestMultitenancyDepsAdding()
        {
            _services.ActivateMultitenancy();
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
            _services.ActivateMultitenancy();
            _services.AddScoped<IFake, Fake>();
            _services.AddScopedForTenant<IFake, TenantFake>(new TenantId());
            _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            IServiceProvider provider = _services.BuildMultitenantServiceProvider();
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
            string ya = "yahoo";
            _services.ActivateMultitenancy();
            _services.AddScoped<IFake, Fake>();
            _services.AddScopedForTenant<IFake, TenantFake>(new TenantId(ya));
            Mock<IHttpContextAccessor> accessor = new Mock<IHttpContextAccessor>();
            _services.AddSingleton(accessor.Object);
            IServiceProvider serviceProvider = _services.BuildMultitenantServiceProvider();

            Mock<HttpContext> ctx = GetHttpContextMock("google.com", new Dictionary<object, object>());
            accessor.Setup(acc => acc.HttpContext).Returns(ctx.Object);

            TenantDetectorMiddleware detector = new TenantDetectorMiddleware(null,
                MultitenantMappingConfiguration.FromDictionary(new Dictionary<string, string> {{ya, "yahoo"}}));
            await detector.InvokeAsync(ctx.Object);

            IFake service = serviceProvider.GetService<IFake>();
            Assert.Equal(typeof(Fake),service.GetType());

            ctx = GetHttpContextMock("yahoo.com", new Dictionary<object, object>());
            accessor.Setup(acc => acc.HttpContext).Returns(ctx.Object);
            await detector.InvokeAsync(ctx.Object);
            service = serviceProvider.GetService<IFake>();
            Assert.Equal(typeof(TenantFake), service.GetType());
        }

        //TODO:Add unit tests for transient and singleton registered services
    }
}
