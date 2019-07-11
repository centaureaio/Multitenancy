using System;
using Centaurea.Multitenancy.Annotation;
using Microsoft.AspNetCore.Http;

namespace Centaurea.Multitenancy
{
//    public class CachedTenantResolver : ITenantResolver
//    {
//        private readonly IHttpContextAccessor _ctx;
//        private TenantId _tenantCache = TenantId.UNKNOWN;
//        private bool _isAlreadyActivated;
//
//        public CachedTenantResolver(IHttpContextAccessor context)
//        {
//            _ctx = context;
//        }
//
//        public void ForceTenantReset(TenantId tenant)
//        {
//            if (_isAlreadyActivated)
//            {
//                throw new InvalidOperationException("TenantResolver was already used inside this scope and changing current tenant could cause unpredicted behaviour.");
//            }
//            __set(tenant);
//        }
//
//        private void __set(TenantId tenant)
//        {
//            _tenantCache = tenant;
//            _isAlreadyActivated = true;
//        }
//
//        public TenantId Current
//        {
//            get
//            {
//                if (!_isAlreadyActivated)
//                {
//                    __set((TenantId)(_ctx.HttpContext?.Items[Constants.TENANT_CONTEXT_KEY] ?? TenantId.DEFAULT_ID));
//                }
//                return _tenantCache;
//            }
//        }
//    }
}