using Microsoft.AspNetCore.Builder;

namespace Centaurea.Multitenancy
{
    public static class MultitenantAppExtension
    {
        public static void UseMultitenancy(this IApplicationBuilder app, MultitenantMappingConfiguration config)
        {
            app.UseMiddleware<TenantDetectorMiddleware>(config);
        }
    }
}