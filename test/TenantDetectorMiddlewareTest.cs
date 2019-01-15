using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Centaurea.Multitenancy.Test
{
    public class TenantDetectorMiddlewareTest
    {
        private static readonly Dictionary<string, string> DEFAULT_CFG = new Dictionary<string, string>
            {{"Test", "testhost.com"}};

        [Fact]
        public void TestMiddlewareActivationExtension()
        {
            Mock<IApplicationBuilder> appMock = InitAppMockWithMiddleware(DEFAULT_CFG);

            appMock.Verify(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once());
        }

        [Theory]
        [InlineData("google.com", TenantId.DEFAULT)]
        [InlineData("", TenantId.DEFAULT)]
        [InlineData(null, TenantId.DEFAULT)]
        [InlineData("testhost.com", "Test")]
        public async void TestMiddlewareCorrectlyDetectSimpleHostMatched(string host, string tenant)
        {
            Mock<IApplicationBuilder> appMock = InitAppMockWithMiddleware(DEFAULT_CFG);
            await RunTenantDetectionCheck(host, tenant, appMock);
        }
        
        [Theory]
        [InlineData("www.google.com", "google", "^www")]
        [InlineData("qwwwgoogle.com", "google", "^www", TenantId.DEFAULT)]
        [InlineData("ww1.google.com", "google", "^www", TenantId.DEFAULT)]
        [InlineData("googlewwww.com", "google", "^www", TenantId.DEFAULT)]
        public async void TestMiddlewareDetectTennatBasedOnRegexp(string host, string tenant = null, string regex = null,
            string expectedT = null)
        {
            Mock<IApplicationBuilder> appMock = InitAppMockWithMiddleware(new Dictionary<string, string>{{tenant, regex}});
            await RunTenantDetectionCheck(host, tenant, appMock, expectedT);
        }

        private async Task RunTenantDetectionCheck(string host, string tenant, Mock<IApplicationBuilder> appMock, string expectedT = null)
        {
            Dictionary<object, object> requestData = new Dictionary<object, object>();

            Mock<HttpContext> ctxMock = new Mock<HttpContext>();
            ctxMock.Setup(httpContext => httpContext.Request).Returns(GetMockedRequest(host).Object);
            ctxMock.Setup(ctx => ctx.Items).Returns(requestData);

            RequestDelegate request = appMock.Object.Build();
            await request.Invoke(ctxMock.Object);

            Assert.Single(requestData);
            Assert.Contains(new TenantId(expectedT ?? tenant), requestData.Values);
        }

        private Mock<HttpRequest> GetMockedRequest(string host = "www.site.com")
        {
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Host).Returns(new HostString(host));
            return requestMock;
        }

        private Mock<IApplicationBuilder> InitAppMockWithMiddleware(Dictionary<string, string> middlewareConfig)
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