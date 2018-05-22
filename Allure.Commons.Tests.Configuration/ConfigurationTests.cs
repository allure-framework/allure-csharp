using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        static object lockobj = new object();

        [TestCase(null)]
        [TestCase(@"{}")]
        [TestCase(@"{""allure"":{""logging"": ""false""}}")]
        public void Defaults(string config)
        {
            RestoreState(config);

            var allureCycle = AllureLifecycle.CreateInstance();
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<AllureLifecycle>(allureCycle);
                Assert.IsNotNull(allureCycle.Configuration);
                Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, AllureConstants.DEFAULT_RESULTS_FOLDER),
                    allureCycle.ResultsDirectory);
            });
        }

        [Test, Description("Should set results directory from config")]
        public void ShouldSetResultsDirectoryFromConfig()
        {
            var config = @"{""allure"":{""directory"": ""test""}}";
            RestoreState(config);
            var allureCycle = AllureLifecycle.CreateInstance();
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "test"),
                allureCycle.ResultsDirectory);

        }

        //[Theory(DisplayName = "Should instantiate Lifecycle with/without logging")]
        //[InlineData(@"{""allure"":{""logging"": ""true""}}")]
        //public void LoggerInitializingTest(string config)
        //{
        //    RestoreState(config);
        //    var allureCycle = AllureLifecycle.CreateInstance();
        //    Assert.IsNotType<AllureLifecycle>(allureCycle);

        //}

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
