using Allure.Commons;
using System.IO;
using Xunit;
namespace Allure.Commons.Tests.Configuration
{
    public class ConfigurationTests
    {
        static object lockobj = new object();

        [Theory(DisplayName = "Should initialize using configuration defaults")]
        [InlineData(null)]
        [InlineData(@"{}")]
        [InlineData(@"{""allure"":{""logging"": ""false""}}")]

        public void Defaults(string config)
        {
            RestoreState(config);

            var allureCycle = AllureLifecycle.CreateInstance();
            Assert.IsType<AllureLifecycle>(allureCycle);


        }

        [Theory(DisplayName = "Should instantiate Lifecycle with/without logging")]
        [InlineData(@"{""allure"":{""logging"": ""true""}}")]
        public void LoggerInitializingTest(string config)
        {
            RestoreState(config);
            var allureCycle = AllureLifecycle.CreateInstance();
            Assert.IsNotType<AllureLifecycle>(allureCycle);

        }

        [Fact(DisplayName = "Should access Configuration")]
        public void ShouldAccessConfigProperties()
        {
            var config = @"{""allure"":{""customKey"": ""customValue""}}";
            RestoreState(config);
            var cycle = AllureLifecycle.CreateInstance();
            Assert.Equal("customValue", cycle.Configuration["allure:customKey"]);

        }

        private static void RestoreState(string config)
        {
            lock (lockobj)
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
}
