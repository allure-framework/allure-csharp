using Allure.Commons;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Allure.Configuration.Tests
{
    public class ConfigurationTests
    {
        [Theory(DisplayName = "Should initialize using configuration defaults")]
        [InlineData(null)]
        [InlineData(@"{}")]

        public void Defaults(string config)
        {
            RestoreState(config);

            var allureCycle = new AllureLifecycle();

            Assert.Equal(Path.Combine(Environment.CurrentDirectory, AllureConstants.DEFAULT_RESULTS_FOLDER), allureCycle.Output);

        }

        private static void RestoreState(string config)
        {
            if (Directory.Exists(AllureConstants.DEFAULT_RESULTS_FOLDER))
                Directory.Delete(AllureConstants.DEFAULT_RESULTS_FOLDER, true);

            if (config != null)
                File.WriteAllText(AllureConstants.CONFIG_FILENAME, config);
            else
                if (File.Exists(AllureConstants.CONFIG_FILENAME))
                File.Delete(AllureConstants.CONFIG_FILENAME);
        }

        [Theory(DisplayName = "Should cleanup temp results folder")]
        [InlineData(@"{""allure"":{""directory"": ""c:\\windows\\allure-results"", ""cleanup"":  true}}")]
        public void ShouldCleanupTempResultsFolder(string config)
        {
            RestoreState(config);

            var expectedResultsFolder = Path.Combine(Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);
            File.WriteAllText(Path.Combine(expectedResultsFolder, Guid.NewGuid().ToString()), "");

            var allureCycle = new AllureLifecycle();

            Assert.Empty(Directory.GetFiles(expectedResultsFolder));
        }

        [Theory(DisplayName = "Should cleanup existing results folder")]
        [InlineData(@"{""allure"":{""cleanup"": true}}")]

        public void ShouldCleanupExistingResultsFolder(string config)
        {
            RestoreState(config);

            var expectedResultsFolder = AllureConstants.DEFAULT_RESULTS_FOLDER;
            Directory.CreateDirectory(expectedResultsFolder);
            File.WriteAllText(Path.Combine(expectedResultsFolder, Guid.NewGuid().ToString()), "");

            var allureCycle = new AllureLifecycle();

            Assert.Empty(Directory.GetFiles(expectedResultsFolder));
        }

        [Fact(DisplayName = "Should access Configuration")]
        public void ShouldAccessConfigProperties()
        {
            var config = @"{""allure"":{""customKey"": ""customValue""}}";
            RestoreState(config);
            var cycle = new AllureLifecycle();
            Assert.Equal("customValue", cycle.Configuration["allure:customKey"]);

        }
    }
}
