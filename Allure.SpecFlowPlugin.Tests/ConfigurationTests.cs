using NUnit.Framework;
using System.IO;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [TestCase(@"allureConfigStepArguments.json")]
        [TestCase(@"allureConfigEmpty.json")]
        [TestCase(@"allureConfigWithInvalidRegex.json")]
        public void ParamNameRegex(string json)
        {
            var config = PluginHelper.GetConfiguration(File.ReadAllText(json));
            Assert.IsNotNull(config);
        }
    }
}
