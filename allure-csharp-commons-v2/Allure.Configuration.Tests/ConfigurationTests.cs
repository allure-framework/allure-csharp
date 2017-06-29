using Allure.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Allure.Configuration.Tests
{
    public class ConfigurationTests : IDisposable
    {
        public ConfigurationTests()
        {
            RestoreConfig();
        }
        public void Dispose()
        {
            RestoreConfig();
            GC.SuppressFinalize(this);
        }

        [Fact(DisplayName = "Should create 'Allure-results' folder without config file")]
        public void ShouldCreateResultsFoldernWithoutConfigFile()
        {
            if (File.Exists(AllureConstants.CONFIG_FILENAME))
                File.Delete(AllureConstants.CONFIG_FILENAME);

            if (Directory.Exists(AllureConstants.DEFAULT_RESULTS_FOLDER))
                Directory.Delete(AllureConstants.DEFAULT_RESULTS_FOLDER, true);

            var allureCycle = new AllureLifecycle();

            Assert.True(Directory.Exists(AllureConstants.DEFAULT_RESULTS_FOLDER));
        }

        [Fact(DisplayName = "Shouldn't cleanup 'Allure-results' by default")]
        public void ShouldntCleanupResultsFolder()
        {
            if (File.Exists(AllureConstants.CONFIG_FILENAME))
                File.Delete(AllureConstants.CONFIG_FILENAME);

            if (!Directory.Exists(AllureConstants.DEFAULT_RESULTS_FOLDER))
                Directory.CreateDirectory(AllureConstants.DEFAULT_RESULTS_FOLDER);

            File.WriteAllText(
                Path.Combine(AllureConstants.DEFAULT_RESULTS_FOLDER,
                Guid.NewGuid().ToString()),
                "");

            var allureCycle = new AllureLifecycle();
            Assert.NotEmpty(
                Directory.GetFiles(AllureConstants.DEFAULT_RESULTS_FOLDER));
        }

        [Fact(DisplayName = "Should access Configuration")]
        public void ShouldAccessConfigProperties()
        {
            var cycle = new AllureLifecycle();
            Assert.Equal("customValue", cycle.Configuration["allure:customKey"]);

        }

        private static void RestoreConfig()
        {
            var config = File.ReadAllText("testConfig.json");
            if (!File.Exists(AllureConstants.CONFIG_FILENAME))
                File.WriteAllText(AllureConstants.CONFIG_FILENAME, config);
        }
    }
}
