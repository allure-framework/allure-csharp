using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Allure.Commons.Configuration
{
    public class AllureConfiguration
    {
        private AllureConfiguration()
        {
        }

        [JsonConstructor]
        protected AllureConfiguration(string title, string directory, HashSet<string> links)
        {
            Title = title ?? Title;
            Directory = directory ?? Directory;
            Links = links ?? Links;
        }

        public string Title { get; }
        public string Directory { get; } = "allure-results";
        public HashSet<string> Links { get; } = new HashSet<string>();

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