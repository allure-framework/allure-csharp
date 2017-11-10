using Allure.Commons;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class ConfigurationTests
    {
        [NonParallelizable]
        [TestCase(@"..\..\allureConfig.json", ExpectedResult = "^a.*")]
        [TestCase(@"..\..\allureConfigEmpty.json", ExpectedResult = ".*")]
        [TestCase(@"..\..\allureConfigWithInvalidRegex.json", ExpectedResult = ".*")]
        public string ReadPluginConfiguration(string config)
        {
            File.Copy(config, "allureConfig.json", true);
            var configuration = new PluginConfiguration(AllureLifecycle.CreateInstance().Configuration);
            Assert.Multiple(() =>
            {
                Assert.NotNull(configuration.ConvertToParameters);
                Assert.NotNull(configuration.ParamNameRegex);
                Assert.NotNull(configuration.ParamValueRegex);
            });
            return configuration.ParamNameRegex.ToString();
        }
    }
}
