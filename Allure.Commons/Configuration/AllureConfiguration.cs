using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Allure.Commons.Configuration
{
    public class AllureConfiguration
    {
        public string Directory { get; } = "allure-results";
        public HashSet<string> Links { get; } = new HashSet<string>();

        private AllureConfiguration() { }

        [JsonConstructor]
        protected AllureConfiguration(string directory, HashSet<string> links)
        {
            Directory = directory ?? Directory;
            Links = links ?? Links;
        }

        public static AllureConfiguration ReadFromJson(string json)
        {
            var config = new AllureConfiguration();

            var jo = JObject.Parse(json);
            var allureSection = jo["allure"];
            if (allureSection != null)
                config = allureSection?.ToObject<AllureConfiguration>();

            return config;
        }
    }
}
