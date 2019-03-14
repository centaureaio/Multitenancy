using System.Collections.Concurrent;
using Centaurea.Multitenancy.Annotation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Centaurea.Multitenancy
{
    public class TenantOptionsManager<TOptions> : OptionsManager<TOptions>, ITenantOptions<TOptions> where TOptions : class, new()
    {
        private readonly ITenantResolver _resolver;
        private readonly TenantConfig _config;

        private readonly ConcurrentDictionary<TenantId, TOptions> _cache =
            new ConcurrentDictionary<TenantId, TOptions>();

        public TenantOptionsManager(IOptionsFactory<TOptions> factory, ITenantResolver resolver, TenantConfig config) : base(factory)
        {
            _resolver = resolver;
            _config = config;
        }

        public override TOptions Get(string name)
        {
            return _cache.GetOrAdd(_resolver.Current,
                tenant =>
                {
                    TOptions result = base.Get(name);

                    if (!tenant.Equals(TenantId.DEFAULT_ID) && (typeof(IMultitenantSetting).IsAssignableFrom(typeof(TOptions))))
                    {
                        string tenantPath = $"{tenant}:{ConfigAttribute.ReadSectionPath(typeof(TOptions))}";

                        TOptions tenantOpts = new TOptions();
                        _config.Config.GetSection(tenantPath).Bind(tenantOpts);

                        JObject initial = JObject.FromObject(result);
                        JObject tenanted = JObject.FromObject(tenantOpts, JsonSerializer.Create(new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }));

                        initial.Merge(tenanted, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Replace,
                            MergeNullValueHandling = MergeNullValueHandling.Ignore,
                            PropertyNameComparison = System.StringComparison.InvariantCultureIgnoreCase
                        });
                        result = initial.ToObject<TOptions>();
                    }

                    return result;
                });
        }
    }

    public interface ITenantOptions<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
    }

    public class TenantConfig
    {
        public IConfigurationSection Config { get; set; }
    }
}