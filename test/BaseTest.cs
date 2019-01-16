using System.Collections.Generic;
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
    }
}