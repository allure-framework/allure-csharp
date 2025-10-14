using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable

namespace Allure.Xunit
{
    internal class AllureXunitConfiguration : AllureConfiguration
    {
        const string KNOWN_TOOL_REQNROLL = "Reqnroll";
        const string KNOWN_TOOL_SPECFLOW = "TechTalk.SpecFlow";

        public string XunitRunnerReporter { get; set; } = "auto";

        public List<string> FirstClassIntegrationTools { get; set; } = [
            KNOWN_TOOL_REQNROLL,
            KNOWN_TOOL_SPECFLOW,
        ];

        [JsonConstructor]
        protected AllureXunitConfiguration(
            string? title,
            string? directory,
            HashSet<string>? links
        ) : base(title, directory, links)
        {
        }

        public static AllureXunitConfiguration CurrentConfig
        {
            get => currentConfig.Value;
        }

        static readonly Lazy<AllureXunitConfiguration> currentConfig
            = new(ParseCurrentConfig);

        static AllureXunitConfiguration ParseCurrentConfig() => JObject.Parse(
            AllureLifecycle.Instance.JsonConfiguration
        )["allure"]?.ToObject<AllureXunitConfiguration>()
            ?? new AllureXunitConfiguration(null, null, null);
    }
}
