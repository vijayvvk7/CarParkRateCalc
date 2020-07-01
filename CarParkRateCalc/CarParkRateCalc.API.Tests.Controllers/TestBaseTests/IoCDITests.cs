using System.Threading.Tasks;

using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CarParkRateCalc.API.Common.Settings;

namespace CarParkRateCalc.API.Tests.Controllers.TestBaseTests
{
    [TestClass]
    public class IoCDITests : TestBase
    {
        [TestMethod]
        public async Task IoC_DI_ServiceProvider_OK()
        {
            Assert.IsNotNull(_serviceProvider);
        }


        [TestMethod]
        public async Task IoC_DI_Mapper_OK()
        {
            Assert.IsNotNull(_serviceProvider);

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            Assert.IsNotNull(mapper);
        }

        [TestMethod]
        public async Task IoC_DI_LoggerFactory_OK()
        {
            var serviceProvider = _services.BuildServiceProvider();
            Assert.IsNotNull(serviceProvider);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            Assert.IsNotNull(loggerFactory);
        }

        [TestMethod]
        public async Task IoC_DI_IOptions_AppSettings_OK()
        {
            var serviceProvider = _services.BuildServiceProvider();
            Assert.IsNotNull(serviceProvider);

            var ioptions = serviceProvider.GetService<IOptions<AppSettings>>();
            Assert.IsNotNull(ioptions);
        }
    }
}
