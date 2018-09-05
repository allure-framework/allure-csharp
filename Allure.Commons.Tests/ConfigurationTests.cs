using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private string configFile;

        [TestCase(@"{""allure"":{""logging"": ""false""}}")]
        [TestCase(@"{""allure"":{""directory"": ""allure-results""}}")]
        public void Defaults(string json)
        {
            configFile = WriteConfig(json);
            var allureCycle = new AllureLifecycle(configFile);
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<AllureLifecycle>(allureCycle);
                Assert.IsNotNull(allureCycle.JsonConfiguration);
                Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, AllureConstants.DEFAULT_RESULTS_FOLDER),
                    allureCycle.ResultsDirectory);
                Assert.IsNotNull(allureCycle.AllureConfiguration.Links);
            });
        }

        [Test]
        public void ShouldThrowExceptionIfConfigurationNotFound()
        {
            Assert.Throws<FileNotFoundException>(() => new AllureLifecycle(Path.GetRandomFileName()));
        }
        [Test, Description("Should set results directory from config")]
        public void ShouldConfigureResultsDirectoryFromJson()
        {
            var json = @"{""allure"":{""directory"": ""test""}}";
            configFile = WriteConfig(json);
            var allureCycle = new AllureLifecycle(configFile);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "test"),
                allureCycle.ResultsDirectory);
        }

        [TestCase(@"{""allure"":{""links"":[ ""http://test//{}"" ] }}", ExpectedResult = 1)]
        [TestCase(@"{""allure"":{""links"":[ ""http://test//{}"", ""http://test//{}"" ] }}", ExpectedResult = 1)]
        [TestCase(@"{""allure"":{""links"":[ ""http://test//{tms}"", ""http://test//{issue}"" ] }}", ExpectedResult = 2)]
        public int ShouldConfigureIssueLinkPatternFromJson(string json)
        {
            configFile = WriteConfig(json);
            return new AllureLifecycle(configFile).AllureConfiguration.Links.Count;
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(configFile);
        }

        private string WriteConfig(string json)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, json);
            return path;
        }


    }
}
