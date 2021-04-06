using NUnit.Framework;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [TestCase(@"{""allure"":{""logging"": ""false""}}")]
        [TestCase(@"{""allure"":{""directory"": ""allure-results""}}")]
        public void ShouldConfigureDefaultValues(string json)
        {
            var allureLifecycle = new AllureLifecycle(JObject.Parse(json));
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<AllureLifecycle>(allureLifecycle);
                Assert.IsNotNull(allureLifecycle.JsonConfiguration);
                Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, AllureConstants.DEFAULT_RESULTS_FOLDER),
                    allureLifecycle.ResultsDirectory);
                Assert.IsNotNull(allureLifecycle.AllureConfiguration.Links);
            });
        }

        [Test, Description("Should set results directory from config")]
        public void ShouldConfigureResultsDirectoryFromJson()
        {
            var json = @"{""allure"":{""directory"": ""test""}}";
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "test"),
                new AllureLifecycle(JObject.Parse(json)).ResultsDirectory);
        }

        [TestCase(@"{""allure"":{""links"":[ ""http://test//{}"" ] }}", ExpectedResult = 1)]
        [TestCase(@"{""allure"":{""links"":[ ""http://test//{}"", ""http://test//{}"" ] }}", ExpectedResult = 1)]
        [TestCase(@"{""allure"":{""links"":[ ""http://test//{tms}"", ""http://test//{issue}"" ] }}",
            ExpectedResult = 2)]
        public int ShouldConfigureIssueLinkPatternFromJson(string json)
        {
            return new AllureLifecycle(JObject.Parse(json)).AllureConfiguration.Links.Count;
        }

        [Test]
        public void ShouldConfigureTitle()
        {
            var json = @"{""allure"":{""title"": ""hello Allure""}}";
            Assert.AreEqual("hello Allure", new AllureLifecycle(JObject.Parse(json)).AllureConfiguration.Title);
        }
    }
}