using NUnit.Framework;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Tests
{
    [TestFixture]
    public class InstantiationTests
    {
        [SetUp]
        [TearDown]
        public void CleanConfig()
        {
            var defaultConfig = Path.Combine(
                Path.GetDirectoryName(typeof(InstantiationTests).Assembly.Location),
                AllureConstants.CONFIG_FILENAME);
            if (File.Exists(defaultConfig))
                File.Delete(defaultConfig);

            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, null);
        }

        [Test]
        public void ShouldStartWithEmptyConfiguration()
        {
            Assert.IsInstanceOf<AllureLifecycle>(new AllureLifecycle());
            Assert.AreEqual(new JObject().ToString(), new AllureLifecycle().JsonConfiguration);
        }

        [Test]
        public void ShouldThrowIfEnvVariableConfigNotFound()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE,
                Path.Combine(tempDirectory, AllureConstants.CONFIG_FILENAME));

            Assert.Throws<FileNotFoundException>(() => { new AllureLifecycle(); });
        }

        [Test]
        public void ShouldReadConfigFromEnvironmentVariable()
        {
            var configuration = @"{""allure"":{""directory"": ""env""}}";

            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            var configFile = Path.Combine(tempDirectory, AllureConstants.CONFIG_FILENAME);
            File.WriteAllText(configFile, configuration);
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, configFile);

            Assert.AreEqual("env", new AllureLifecycle().AllureConfiguration.Directory);
        }

        [Test]
        public void ShouldReadConfigFromAppDomainDirectoryIfEnvVariableNotSpecified()
        {
            var configContent = @"{""allure"":{""directory"": ""bin""}}";
            Assert.IsNull(Environment.GetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE));
            File.WriteAllText(AllureConstants.CONFIG_FILENAME, configContent);

            Assert.AreEqual("bin", new AllureLifecycle().AllureConfiguration.Directory);
        }
    }
}