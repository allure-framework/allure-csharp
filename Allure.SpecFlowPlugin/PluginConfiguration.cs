namespace Allure.SpecFlowPlugin
{

    public class PluginConfiguration
    {
        public Steparguments stepArguments { get; set; }
        public Grouping grouping { get; set; }
        public Labels labels { get; set; }
        public Links links { get; set; }
    }

    public class Steparguments
    {
        public bool convertToParameters { get; set; }
        public string paramNameRegex { get; set; }
        public string paramValueRegex { get; set; }
    }

    public class Grouping
    {
        public Suites suites { get; set; }
        public Behaviors behaviors { get; set; }
        public Packages packages { get; set; }
    }

    public class Suites
    {
        public string parentSuite { get; set; }
        public string suite { get; set; }
        public string subSuite { get; set; }
    }

    public class Behaviors
    {
        public string epic { get; set; }
        public string story { get; set; }
    }

    public class Packages
    {
        public string package { get; set; }
        public string testClass { get; set; }
        public string testMethod { get; set; }
    }

    public class Labels
    {
        public string owner { get; set; }
        public string severity { get; set; }
    }

    public class Links
    {
        public string issue { get; set; }
        public string tms { get; set; }
    }



    //public class PluginConfiguration
    //{
    //    public bool ConvertToParameters { get; }
    //    public Regex ParamNameRegex { get; }
    //    public Regex ParamValueRegex { get; }

    //    public Regex ParentSuiteRegex { get; }
    //    public Regex SuiteRegex { get; }
    //    public Regex SubSuiteRegex { get; }

    //    public Regex EpicRegex { get; }
    //    public Regex StoryRegex { get; }

    //    public Regex PackageRegex { get; }
    //    public Regex TestClassRegex { get; }
    //    public Regex TestMethodRegex { get; }

    //    public Regex OwnerRegex { get; }
    //    public Regex SeverityRegex { get; }

    //    public Regex IssueRegex { get; }
    //    public Regex TmsRegex { get; }


    //    public PluginConfiguration(string allureConfiguration)
    //    {
    //        var sf = JObject.Parse(allureConfiguration)["specflow"];

    //        var stepArguments = sf["stepArguments"];
    //        ConvertToParameters = stepArguments["convertToParameters"].Value<bool>();
    //        ParamNameRegex = ParseRegex((string)stepArguments["paramNameRegex"]);
    //        ParamValueRegex = ParseRegex((string)stepArguments["paramValueRegex"]);

    //        var grouping = sf["grouping"];
    //        ParentSuiteRegex = ParseRegex((string)grouping["suites"]["parentSuite"]);
    //        SuiteRegex = ParseRegex((string)grouping["suites"]["suite"]);
    //        SubSuiteRegex = ParseRegex((string)grouping["suites"]["subSuite"]);

    //        EpicRegex = ParseRegex((string)grouping["behaviors"]["epic"]);
    //        StoryRegex = ParseRegex((string)grouping["behaviors"]["story"]);

    //        PackageRegex = ParseRegex((string)grouping["packages"]["package"]);
    //        TestClassRegex = ParseRegex((string)grouping["packages"]["testClass"]);
    //        TestMethodRegex = ParseRegex((string)grouping["packages"]["testMethod"]);

    //        var labels = sf["labels"];
    //        OwnerRegex = ParseRegex((string)labels["owner"]);
    //        SeverityRegex = ParseRegex((string)labels["severity"]);

    //        var links = sf["links"];
    //        IssueRegex = ParseRegex((string)links["issue"]);
    //        TmsRegex = ParseRegex((string)links["tms"]);


    //    }

    //    private Regex ParseRegex(string value)
    //    {
    //        if (string.IsNullOrWhiteSpace(value))
    //            return null;
    //        try
    //        {
    //            return new Regex(value,
    //                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

    //        }
    //        catch (Exception)
    //        {
    //            return null;
    //        }
    //    }
    //}
}
