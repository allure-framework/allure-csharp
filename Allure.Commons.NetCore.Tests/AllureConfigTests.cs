using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.NetCore.Tests
{
    [TestFixture]
    public class AllureConfigTests
    {
        [Test]
        public void ShouldReadAllureConfig()
        {
            Environment.SetEnvironmentVariable(
                AllureConstants.ALLURE_CONFIG_ENV_VARIABLE,
                Path.Combine(Environment.CurrentDirectory, AllureConstants.CONFIG_FILENAME));
            var config = AllureLifecycle.Instance.JsonConfiguration;
        }
    }
}
