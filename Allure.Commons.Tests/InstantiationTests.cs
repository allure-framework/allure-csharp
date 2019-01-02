using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class InstantiationTests
    {
        [SetUp]
        [TearDown]
        public void CleanConfig()
        {
            var defaultConfig = Path.Combine(
                Path.GetDirectoryName(typeof(AllureLifecycle).Assembly.Location),
                AllureConstants.CONFIG_FILENAME);
            if (File.Exists(defaultConfig))
                File.Delete(defaultConfig);

            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, null);
        }

        [Test]
        public void ShouldThrowIfConfigNotFoundInBinaryAndNotSpecifiedInEnvVariable()
        {
            Assert.Throws<FileNotFoundException>(() => { new AllureLifecycle(); });
        }

        [Test]
        public void ShouldThrowIfEnvVariableConfigNotFound()
        {
            var tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempdir);
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE,
                Path.Combine(tempdir, AllureConstants.CONFIG_FILENAME));

            Assert.Throws<FileNotFoundException>(() => { new AllureLifecycle(); });
        }

        [Test]
        public void ShouldReadConfigFromEnvironmentVariable()
        {
            var configContent = @"{""allure"":{""directory"": ""env""}}";

            var tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempdir);
            var configFile = Path.Combine(tempdir, AllureConstants.CONFIG_FILENAME);
            File.WriteAllText(configFile, configContent);
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, configFile);

            Assert.AreEqual("env", new AllureLifecycle().AllureConfiguration.Directory);
        }

        [Test]
        public void ShouldReadConfigFromBinaryIfEnvVariableNotSpecified()
        {
            var configContent = @"{""allure"":{""directory"": ""bin""}}";
            File.WriteAllText(AllureConstants.CONFIG_FILENAME, configContent);
            Assert.AreEqual("bin", new AllureLifecycle().AllureConfiguration.Directory);
        }

    }
}
