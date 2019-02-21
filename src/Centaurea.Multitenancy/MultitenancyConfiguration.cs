using Microsoft.Extensions.Configuration;

namespace Centaurea.Multitenancy
{
    public class MultitenancyConfiguration
    {
        public IConfiguration Config { get; set; }
        public string ConfigKey { get; set; } = Constants.TENANT_CONF_KEY;
        public ITenantConfiguration TenantConfiguration { get; set; }
    }
}