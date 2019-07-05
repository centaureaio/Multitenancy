using System;
using System.Collections.Concurrent;
using Centaurea.Multitenancy.Annotation;
using Microsoft.Extensions.Options;

namespace Centaurea.Multitenancy
{
    public class TenantOptionsMonitorCache<TOptions> : IOptionsMonitorCache<TOptions> where TOptions : class
    {
        private readonly ConcurrentDictionary<TenantId, Lazy<IOptionsMonitorCache<TOptions>>> _cache = new ConcurrentDictionary<TenantId, 
            Lazy<IOptionsMonitorCache<TOptions>>>();

        private readonly ITenantResolver _resolver;

        public TenantOptionsMonitorCache(ITenantResolver resolver)
        {
            _resolver = resolver;
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions) => GetCache.Value.GetOrAdd(name, createOptions);

        public bool TryAdd(string name, TOptions options) => GetCache.Value.TryAdd(name, options);

        public bool TryRemove(string name) => GetCache.Value.TryRemove(name);

        public void Clear() => GetCache.Value.Clear();

        private Lazy<IOptionsMonitorCache<TOptions>> GetCache =>
            _cache.GetOrAdd(_resolver.Current, (tenant) => new Lazy<IOptionsMonitorCache<TOptions>>(() => new OptionsCache<TOptions>()));
    }
}