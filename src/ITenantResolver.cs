using Microsoft.AspNetCore.Http;

namespace Centaurea.Multitenancy
{
    public interface ITenantResolver
    {
        TenantId Current { get; }
    }

    public class DefaultTenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _ctx;

        public DefaultTenantResolver(IHttpContextAccessor context)
        {
            _ctx = context;
        }

        public TenantId Current => (TenantId)_ctx.HttpContext.Items[Constants.TENANT_CONTEXT_KEY];
    }
}
