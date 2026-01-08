using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Configuration
{
    public class AllureConfiguration
    {
        internal AllureConfiguration() : this(null, null, null)
        {
        }

        [JsonConstructor]
        protected AllureConfiguration(string title, string directory, HashSet<string> links)
        {
            this.Title = title;
            this.Directory =
                Environment.GetEnvironmentVariable(AllureConstants.ALLURE_RESULTSDIR_ENV_VARIABLE)
                    ?? directory
                    ?? AllureConstants.DEFAULT_RESULTS_FOLDER;
            this.Links = links ?? [];
        }

        public string Title { get; init; }
        public string Directory { get; init; }
        public HashSet<string> Links { get; }
        public List<string> FailExceptions { get; set; }
        public bool UseLegacyIds { get; set; } = false;
        public bool IndentOutput { get; set; } = false;
        public Dictionary<string, string> GlobalLabels { get; set; } = [];

        public static AllureConfiguration ReadFromJObject(JObject jObject)
        {
            var config = new AllureConfiguration();
            var allureSection = jObject["allure"];
            if (allureSection != null)
                config = allureSection?.ToObject<AllureConfiguration>();

            return config;
        }
    }
}