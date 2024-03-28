using NUnit.Framework;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Tests
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
                Assert.That(allureLifecycle, Is.InstanceOf<AllureLifecycle>());
                Assert.That(allureLifecycle.JsonConfiguration, Is.Not.Null);
                Assert.That(allureLifecycle.ResultsDirectory, Is.EqualTo(
                    Path.Combine(Environment.CurrentDirectory, AllureConstants.DEFAULT_RESULTS_FOLDER)
                ));
                Assert.That(allureLifecycle.AllureConfiguration.Links, Is.Not.Null);
            });
        }

        [Test, Description("Should set results directory from config")]
        public void ShouldConfigureResultsDirectoryFromJson()
        {
            var json = @"{""allure"":{""directory"": ""test""}}";
            Assert.That(new AllureLifecycle(JObject.Parse(json)).ResultsDirectory, Is.EqualTo(
                Path.Combine(Environment.CurrentDirectory, "test")
            ));
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
            Assert.That(new AllureLifecycle(JObject.Parse(json)).AllureConfiguration.Title, Is.EqualTo("hello Allure"));
        }
    }
}