using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Centaurea.Multitenancy
{
    public class MultitenantMappingConfiguration
    {
        private readonly Dictionary<TenantId, Regex> _mapping;

        MultitenantMappingConfiguration(Dictionary<TenantId, Regex> cfg)
        {
            _mapping = cfg;
        }

        public TenantId GetMatchingOrDefault(string host)
        {
            TenantId result = _mapping.First().Key;
            foreach (KeyValuePair<TenantId, Regex> pair in _mapping.Skip(1))
            {
                if (pair.Value.IsMatch(host))
                {
                    result = pair.Key;
                }
            }

            return result;
        }

        public static MultitenantMappingConfiguration FromJson(string pathToJson)
        {
            if (!File.Exists(pathToJson))
            {
                throw new FileNotFoundException($"No file found at the address of {pathToJson}");
            }

            var data = JsonConvert.DeserializeAnonymousType(
                new JsonTextReader(new StreamReader(File.OpenRead(pathToJson))).ReadAsString(),
                new {Tenants = new List<KeyValuePair<string, string>>()});

            return BuildConfiguration(data.Tenants);
        }

        public static MultitenantMappingConfiguration FromDictionary(Dictionary<string, string> map)
        {
            return BuildConfiguration(map.ToList());
        }

        private static MultitenantMappingConfiguration BuildConfiguration(List<KeyValuePair<string, string>> items)
        {
            Dictionary<TenantId, Regex> mapping = new Dictionary<TenantId, Regex>() {{new TenantId(), new Regex(".")}};

            items?.ForEach(kv => mapping.Add(new TenantId(kv.Key), new Regex(kv.Value)));
            return new MultitenantMappingConfiguration(mapping);
        }
    }
}