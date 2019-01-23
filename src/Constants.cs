using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Centaurea.Multitenancy.Test")]
namespace Centaurea.Multitenancy
{
    internal class Constants
    {
       internal const string TENANT_CONF_KEY = "tenants";
       internal const string TENANT_CONTEXT_KEY = "tenant_name_for_context";
    }
}