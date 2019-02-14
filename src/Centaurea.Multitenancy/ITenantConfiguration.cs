using System.Collections.Generic;
using Centaurea.Multitenancy.Annotation;

namespace Centaurea.Multitenancy
{
    public interface ITenantConfiguration
    {
        TenantId GetMatchingOrDefault(string host);
        bool IsValidTenant(TenantId tenant);
        IReadOnlyCollection<TenantId> GetAll();
    }
}