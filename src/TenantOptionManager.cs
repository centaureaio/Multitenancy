using Microsoft.Extensions.Options;

namespace Centaurea.Multitenancy
{
    public class TenantOptionManager<TOptions>: OptionsManager<TOptions> where TOptions: class, new()
    {
        public TenantOptionManager(IOptionsFactory<TOptions> factory) : base(factory)
        {
        }

        public override TOptions Get(string name)
        {
            return base.Get(name);
        }
    }
}