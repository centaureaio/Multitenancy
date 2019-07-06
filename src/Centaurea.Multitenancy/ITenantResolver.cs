using Centaurea.Multitenancy.Annotation;

namespace Centaurea.Multitenancy
{
    public interface ITenantResolver
    {
        TenantId Current { get; }
    }
}
