using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Configuration
{
    public class AllureConfiguration
    {
        internal AllureConfiguration()
        {
        }

        [JsonConstructor]
        protected AllureConfiguration(string title, string directory, HashSet<string> links)
        {
            Title = title ?? Title;
            Directory = directory ?? Directory;
            Links = links ?? Links;
        }

        public string Title { get; init; }
        public string Directory { get; init; } = AllureConstants.DEFAULT_RESULTS_FOLDER;
        public HashSet<string> Links { get; } = new HashSet<string>();
        public List<string> FailExceptions { get; set; }
        public bool UseLegacyIds { get; set; } = false;

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