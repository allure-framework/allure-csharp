namespace Allure.SpecFlowPlugin
{
    public class PluginConfiguration
    {
        public Steparguments stepArguments { get; set; } = new();
        public Grouping grouping { get; set; } = new();
        public Labels labels { get; set; } = new();
        public Links links { get; set; } = new();
    }

    public class Steparguments
    {
        public bool convertToParameters { get; set; }
        public string? paramNameRegex { get; set; }
        public string? paramValueRegex { get; set; }
    }

    public class Grouping
    {
        public Suites suites { get; set; } = new();
        public Behaviors behaviors { get; set; } = new();
        public Packages packages { get; set; } = new();
    }

    public class Suites
    {
        public string? parentSuite { get; set; }
        public string? suite { get; set; }
        public string? subSuite { get; set; }
    }

    public class Behaviors
    {
        public string? epic { get; set; }
        public string? story { get; set; }
    }

    public class Packages
    {
        public string? package { get; set; }
        public string? testClass { get; set; }
        public string? testMethod { get; set; }
    }

    public class Labels
    {
        public string? owner { get; set; }
        public string? severity { get; set; }
        public string? label { get; set; }
    }

    public class Links
    {
        public string? link { get; set; }
        public string? issue { get; set; }
        public string? tms { get; set; }
    }
}