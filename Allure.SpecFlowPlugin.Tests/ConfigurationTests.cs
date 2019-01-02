using NUnit.Framework;
using System.IO;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [TestCase(@"allureConfig.json")]
        [TestCase(@"allureConfigStepArguments.json")]
        [TestCase(@"allureConfigWithInvalidRegex.json")]
        public void ShouldNotHaveNullParents(string json)
        {
            var config = PluginHelper.GetConfiguration(File.ReadAllText(json));
            Assert.Multiple(() =>
            {
                Assert.That(config.grouping.behaviors, Is.Not.Null);
                Assert.That(config.grouping.packages, Is.Not.Null);
                Assert.That(config.grouping.suites, Is.Not.Null);
                Assert.That(config.labels, Is.Not.Null);
                Assert.That(config.links, Is.Not.Null);
                Assert.That(config.stepArguments, Is.Not.Null);
            });
        }
    }
}
