using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Centaurea.Multitenancy;
using System;

namespace Centaurea.Multitenancy.Test
{
    public class AddTenantRelatedService
    {
        interface IFake { };
        class Fake : IFake { };

        class FakeChild : Fake { };

        class TenantFake : IFake { };
        private IServiceCollection _services;

        private TenantId testTenant = new TenantId("test");

        public AddTenantRelatedService()
        {
            _services = new ServiceCollection();
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
        public void ResolveTenantedScopedService()
        {
            _services.AddScoped<IFake, Fake>();
            _services.AddScopedForTenant<IFake, TenantFake>(testTenant);

            IServiceProvider serviceProvider = _services.BuildMultitenantServiceProvider();

            var scope = serviceProvider.CreateScope();
//            scope.
//
////            _services.

        }

//        private void CreateTenantConfiguration()
//
//        private void RegisterContextAccessor(IServiceCollection services)
//        {
//            var httpContextMock
//        }

        
    }
}
