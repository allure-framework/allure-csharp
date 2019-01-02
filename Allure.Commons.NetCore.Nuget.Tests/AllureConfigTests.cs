using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.NetCore.Nuget.Tests
{
    [TestFixture]
    public class AllureConfigTests
    {
        [TearDown]
        public void RemoveEnvVariable()
        {
            Environment.SetEnvironmentVariable(
                AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, null);
        }

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
