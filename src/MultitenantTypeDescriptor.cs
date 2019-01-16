using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Centaurea.Multitenancy
{
    internal static class MultitenantTypeDescriptor
    {
        static readonly ConcurrentDictionary<KeyValuePair<Type, TenantId>, Type> _typeCache =
            new ConcurrentDictionary<KeyValuePair<Type, TenantId>, Type>();

        static readonly AssemblyBuilder _dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(Guid.NewGuid().ToString()),
            AssemblyBuilderAccess.Run);

        static readonly ModuleBuilder _dynamicModule = _dynamicAssembly.DefineDynamicModule(Guid.NewGuid().ToString());

        public static Type AdoptToTenant(Type typeToAdopt, TenantId tenantId)
        {
            return Equals(tenantId, new TenantId(TenantId.DEFAULT))
                ? typeToAdopt
                : _typeCache.GetOrAdd(new KeyValuePair<Type, TenantId>(typeToAdopt, tenantId), pair => _dynamicModule
                    .DefineType($"{Guid.NewGuid()}|{pair.Value.ToString()}|{pair.Key.AssemblyQualifiedName}")
                    .CreateTypeInfo().AsType());
        }
    }
}