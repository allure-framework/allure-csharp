using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Configuration
{
    public class AllureConfiguration
    {
        public AllureConfiguration()
        {
        }

        [JsonConstructor]
        public AllureConfiguration(string title, string directory, HashSet<string> links)
        {
            Title = title ?? Title;
            Directory = Path.GetFullPath(directory ?? Directory);
            Links = links ?? Links;
        }

        public string Title { get; init; }
        public string Directory { get; init; } = Path.GetFullPath(AllureConstants.DEFAULT_RESULTS_FOLDER);
        public HashSet<string> Links { get; init; } = [];
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