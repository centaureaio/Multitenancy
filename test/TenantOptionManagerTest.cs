using System;
using System.Collections.Generic;
using System.Linq;
using Centaurea.Multitenancy.Annotation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Centaurea.Multitenancy.Test
{
    public class TenantOptionManagerTest : BaseTest
    {
        private readonly IServiceCollection _services;

        public TenantOptionManagerTest()
        {
            _services = new ServiceCollection();
        }

        [Config("Settings")]
        class Settings : IMultitenantSetting
        {
            public int Number { get; set; }
            public string Str { get; set; }
        }

        class Dep
        {
            public IOptions<Settings> Settings { get; set; }
            public Dep(IOptions<Settings> settings)
            {
                Settings = settings;
            }
        }

        [Fact]
        public async void CorrectlyResolvingTenantDependentSetting()
        {
            string tenant = "tenant";
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(GetSettings(2, "abc"))
                .AddInMemoryCollection(GetSettings(3, null).Select(kv => new KeyValuePair<string, string>($"{Constants.TENANT_CONF_KEY}:{tenant}:{kv.Key}", kv.Value))).Build();

            _services.Configure<Settings>(config.GetSection(typeof(Settings).Name));
            Mock<IHttpContextAccessor> accessor = new Mock<IHttpContextAccessor>();
            _services.AddSingleton(accessor.Object);
            _services.AddScoped<Dep>();
            IServiceProvider sp = _services.BuildMultitenantServiceProvider(GetTenantConfiguration(config, ("yahoo", "yahoo"), (tenant, tenant)));

            await EmulateRequestExecution(accessor, "google.com", "yahoo", "yahoo");

            var dep = sp.GetService<Dep>();
            Assert.NotNull(dep.Settings);
            Assert.NotNull(dep.Settings.Value);
            Assert.Equal(2, dep.Settings.Value.Number);

            await EmulateRequestExecution(accessor, tenant, tenant, tenant);

            dep = sp.GetService<Dep>();
            Assert.NotNull(dep.Settings);
            Assert.NotNull(dep.Settings.Value);
            Assert.Equal(3, dep.Settings.Value.Number);
            Assert.Equal("abc", dep.Settings.Value.Str);
        }

        [Fact]
        public async void CorrectlyResolvingTenantDependentSettingInScope()
        {
            string tenant = "tenant";
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(GetSettings(2, "abc"))
                .AddInMemoryCollection(GetSettings(3, null).Select(kv => new KeyValuePair<string, string>($"{Constants.TENANT_CONF_KEY}:{tenant}:{kv.Key}", kv.Value))).Build();

            _services.Configure<Settings>(config.GetSection(typeof(Settings).Name));
            Mock<IHttpContextAccessor> accessor = new Mock<IHttpContextAccessor>();
            _services.AddSingleton(accessor.Object);
            _services.AddScoped<Dep>();
            IServiceProvider sp = _services.BuildMultitenantServiceProvider(GetTenantConfiguration(config, ("yahoo", "yahoo"), (tenant, tenant)));

            await EmulateRequestExecution(accessor, "google.com", "yahoo", "yahoo");

            using (var scope = sp.CreateScope())
            {
                var dep = scope.ServiceProvider.GetService<Dep>();
                Assert.NotNull(dep.Settings);
                Assert.NotNull(dep.Settings.Value);
                Assert.Equal(2, dep.Settings.Value.Number);

                await EmulateRequestExecution(accessor, tenant, tenant, tenant);

                dep = scope.ServiceProvider.GetService<Dep>();
                Assert.NotNull(dep.Settings);
                Assert.NotNull(dep.Settings.Value);
                Assert.Equal(3, dep.Settings.Value.Number);
                Assert.Equal("abc", dep.Settings.Value.Str);
            }

            await EmulateRequestExecution(accessor, tenant, tenant, tenant);
            using (var scope = sp.CreateScope())
            {
                var dep = scope.ServiceProvider.GetService<Dep>();
                Assert.NotNull(dep.Settings);
                Assert.NotNull(dep.Settings.Value);
                Assert.Equal(3, dep.Settings.Value.Number);
                Assert.Equal("abc", dep.Settings.Value.Str);
            }

        }

        private IEnumerable<KeyValuePair<string, string>> GetSettings(int n, string str) 
            => new Dictionary<string, string> {{"Settings:Number", n.ToString()}, {"Settings:Str", str}};
    }
}