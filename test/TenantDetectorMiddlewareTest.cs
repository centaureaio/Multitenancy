using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Centaurea.Multitenancy.Test
{
    public class TenantDetectorMiddlewareTest : BaseTest

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
        public async void TestMiddlewareDetectTenantBasedOnRegexp(string host, string tenant = null,
            string regex = null,
            string expectedT = null)
        {
            Mock<IApplicationBuilder> appMock =
                InitAppMockWithMiddleware(new Dictionary<string, string> {{tenant, regex}});
            await RunTenantDetectionCheck(host, tenant, appMock, expectedT);
        }

        private async Task RunTenantDetectionCheck(string host, string tenant, Mock<IApplicationBuilder> appMock,
            string expectedT = null)
        {
            Dictionary<object, object> requestData = new Dictionary<object, object>();
            Mock<HttpContext> ctxMock = GetHttpContextMock(host, requestData);

            RequestDelegate request = appMock.Object.Build();
            await request.Invoke(ctxMock.Object);

            Assert.Single(requestData);
            Assert.Contains(new TenantId(expectedT ?? tenant), requestData.Values);
        }
    }
}