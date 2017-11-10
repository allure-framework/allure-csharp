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
        [TestCase(@"..\..\allureConfig.json", ExpectedResult = "^a.*")]
        [TestCase(@"..\..\allureConfigEmpty.json", ExpectedResult = null)]
        [TestCase(@"..\..\allureConfigWithInvalidRegex.json", ExpectedResult = null)]
        public string ParamNameRegex(string config)
        {
            File.Copy(config, "allureConfig.json", true);
            var configuration = new PluginConfiguration(AllureLifecycle.CreateInstance().Configuration);
            return configuration.ParamNameRegex?.ToString();
        }
    }
}
