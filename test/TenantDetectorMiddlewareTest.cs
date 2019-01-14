using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Moq;
using Xunit;

namespace Centaurea.Multitenancy.Test
{
    public class TenantDetectorMiddlewareTest
    {
        [Fact]
        public void TestMiddlewareActivationExtension()
        {
            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(app => app.UseMiddleware(It.IsAny<Type>(), It.IsAny<MultitenantMappingConfiguration>()))
                .Verifiable();

            appMock.Object.UseMultitenancy(MultitenantMappingConfiguration.FromDictionary(new Dictionary<string, string> { { "Test", "testhost.com" } }));
            appMock.Verify(app =>
                app.UseMiddleware(It.IsAny<Type>(), It.IsAny<MultitenantMappingConfiguration>()), Times.Once());
        }
    }
}
