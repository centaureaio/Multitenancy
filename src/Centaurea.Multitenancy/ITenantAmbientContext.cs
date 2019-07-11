using Centaurea.Multitenancy.Annotation;

namespace Centaurea.Multitenancy
{
    public interface ITenantAmbientContext : ITenantResolver
    {
        void SetTenantValue(TenantId tenant);
    }
}