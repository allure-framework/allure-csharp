using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace Allure.XUnit
{
    internal class AllureXunitConfiguration : AllureConfiguration
    {
        public string XunitRunnerReporter { get; set; } = "auto";

        [JsonConstructor]
        protected AllureXunitConfiguration(
            string title,
            string directory,
            HashSet<string> links
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
            ?? throw new FileNotFoundException(
                "allureConfig.json not found"
            );
    }
}
