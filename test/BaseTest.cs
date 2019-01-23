using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Centaurea.Multitenancy.Test
{
    public class BaseTest
    {
        protected Mock<HttpContext> GetHttpContextMock(string host, Dictionary<object, object> requestData)
        {
            Mock<HttpContext> ctxMock = new Mock<HttpContext>();
            ctxMock.Setup(httpContext => httpContext.Request).Returns((HttpRequest) GetMockedRequest(host).Object);
            ctxMock.Setup(ctx => ctx.Items).Returns(requestData);
            return ctxMock;
        }

        private Mock<HttpRequest> GetMockedRequest(string host = "www.site.com")
        {
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Host).Returns(new HostString(host));
            return requestMock;
        }

        protected Mock<IApplicationBuilder> InitAppMockWithMiddleware(Dictionary<string, string> middlewareConfig)
        {
            Mock<IApplicationBuilder> appMock = new Mock<IApplicationBuilder>();
            MultitenantMappingConfiguration config = MultitenantMappingConfiguration.FromDictionary(middlewareConfig);
            appMock.Object.UseMultitenancy(config);
            appMock.Setup(app => app.Build())
                .Returns(ctx => new TenantDetectorMiddleware(null, config).InvokeAsync(ctx));
            return appMock;
        }

        protected async Task<TenantDetectorMiddleware> EmulateRequestExecution(Mock<IHttpContextAccessor> accessor, string host, string tenantId, string tenantRegexp)
        {
            Mock<HttpContext> ctx = GetHttpContextMock(host, new Dictionary<object, object>());
            accessor.Setup(acc => acc.HttpContext).Returns(ctx.Object);

            TenantDetectorMiddleware detector = new TenantDetectorMiddleware(null,
                MultitenantMappingConfiguration.FromDictionary(new Dictionary<string, string> {{tenantId, tenantRegexp}}));
            await detector.InvokeAsync(ctx.Object);
            return detector;
        }
    }
}