using System.Threading;
using Centaurea.Multitenancy.Annotation;

namespace Centaurea.Multitenancy
{
    public class TenantAmbientContextAccessor : ITenantAmbientContext
    {
        private static readonly AsyncLocal<TenantHolder> _tenantIdCurrent = new AsyncLocal<TenantHolder>();
        public TenantId Current => _tenantIdCurrent.Value?.Tenant ?? TenantId.DEFAULT_ID;
        public void SetTenantValue(TenantId tenant)
        {
            TenantHolder holder = _tenantIdCurrent.Value;
            if (holder != null)
            {
                holder.Tenant = null;
            }

            if (tenant != null)
            {
                // Use an object indirection to hold the HttpContext in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _tenantIdCurrent.Value = new TenantHolder { Tenant = tenant };
            }
        }

        private class TenantHolder
        {
            public TenantId Tenant;
        }

    }
}