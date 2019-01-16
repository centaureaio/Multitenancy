using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Centaurea.Multitenancy.Test
{
    public class TenantOptionManagerTest : BaseTest
    {
        private readonly IServiceCollection _services;

        public TenantOptionManagerTest()
        {
            _services = new ServiceCollection();
            _services.ActivateMultitenancy();
        }

        class Settings
        {
            public string Name { get; set; }
        }

        [Fact]
        public void CorrectlyResolvingTenantDependentSetting()
        {
            Assert.False(true);
        }
    }
    

}