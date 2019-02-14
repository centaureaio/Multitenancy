using Microsoft.AspNetCore.Builder;

namespace Centaurea.Multitenancy
{
    public static class MultitenantAppExtension
    {
        public static void UseMultitenancy(this IApplicationBuilder app)
        {
            app.UseMiddleware<TenantDetectorMiddleware>(app.ApplicationServices.GetService(typeof(ITenantConfiguration)));
        }
    }
}