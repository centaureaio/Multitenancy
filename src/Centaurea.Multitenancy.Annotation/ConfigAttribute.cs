using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Centaurea.Multitenancy.Annotation
{
    public class ConfigAttribute : Attribute
    {
        public ConfigAttribute(string section)
        {
            Section = section;
        }

        public string Section { get; set; }

        public static string ReadSectionPath(Type type)
        {
            IEnumerable<ConfigAttribute> attrs = type.GetCustomAttributes<ConfigAttribute>();
            if (attrs is null || !attrs.Any())
            {
                throw new ArgumentException($"Configuration POCO object should be annotated with {typeof(ConfigAttribute).Name} attribute");
            }

            return attrs.FirstOrDefault().Section;
        }

        public static string ReadSectionPath<T>()
        {
            return ReadSectionPath(typeof(T));
        }
    }
}