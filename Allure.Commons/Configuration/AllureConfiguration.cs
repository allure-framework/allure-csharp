using System.Collections.Generic;

namespace Allure.Commons.Configuration
{
    public class AllureConfiguration
    {
        public string Directory { get; set; } = "allure-results";
        public HashSet<string> Links { get; set; } = new HashSet<string>();
    }
}
