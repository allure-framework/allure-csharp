using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            var configContent = @"{""allure"":{}}";

            var tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempdir);
            var configFile = Path.Combine(tempdir, AllureConstants.CONFIG_FILENAME);
            File.WriteAllText(configFile, configContent);
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, configFile);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE, null);
        }
    }
}
