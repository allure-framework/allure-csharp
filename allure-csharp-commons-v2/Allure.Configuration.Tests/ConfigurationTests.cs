using Allure.Commons;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

        }

        [Fact(DisplayName = "Should access Configuration")]
        public void ShouldAccessConfigProperties()
        {
            var config = @"{""allure"":{""customKey"": ""customValue""}}";
            RestoreState(config);
            var cycle = new AllureLifecycle();
            Assert.Equal("customValue", cycle.Configuration["allure:customKey"]);

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


    }
}
