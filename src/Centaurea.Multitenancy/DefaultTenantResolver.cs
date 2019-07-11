using System;
using Centaurea.Multitenancy.Annotation;
using Microsoft.AspNetCore.Http;

namespace Centaurea.Multitenancy
{
    public class DefaultTenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _ctx;
        private readonly ITenantAmbientContext _tenantCtx;

        public DefaultTenantResolver(IHttpContextAccessor context, ITenantAmbientContext ambientTenantContext)
        {
            _ctx = context;
            _tenantCtx = ambientTenantContext;
        }

        public TenantId Current {
            get
            {
                TenantId result;

                if (_ctx.HttpContext != null)
                {
                    result = (TenantId)(_ctx.HttpContext.Items[Constants.TENANT_CONTEXT_KEY] ?? TenantId.DEFAULT_ID);
                }
                else
                {
                    result = _tenantCtx.Current;
                }

                return result;
            }
        } 
    }
}