using Allure.Commons;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Allure.SpecFlowPlugin
{
    public class PluginConfiguration
    {
        public bool ConvertToParameters { get; }
        public Regex ParamNameRegex { get; }
        public Regex ParamValueRegex { get; }

        public Regex ParentSuiteRegex { get; }
        public Regex SuiteRegex { get; }
        public Regex SubSuiteRegex { get; }

        public Regex EpicRegex { get; }
        public Regex StoryRegex { get; }

        public Regex PackageRegex { get; }
        public Regex TestClassRegex { get; }
        public Regex TestMethodRegex { get; }

        public Regex OwnerRegex { get; }
        public Regex SeverityRegex { get; }

        public Regex IssueRegex { get; }
        public Regex TmsRegex { get; }


        public PluginConfiguration(IConfiguration configuration)
        {

            bool.TryParse(configuration["specflow:stepArguments:convertToParameters"], out bool convertToParameters);
            ConvertToParameters = convertToParameters;

            ParamNameRegex = ParseRegex(configuration["specflow:stepArguments:paramNameRegex"]);
            ParamValueRegex = ParseRegex(configuration["specflow:stepArguments:paramValueRegex"]);

            ParentSuiteRegex = ParseRegex(configuration["specflow:grouping:suites:parentSuite"]);
            SuiteRegex = ParseRegex(configuration["specflow:grouping:suites:suite"]);
            SubSuiteRegex = ParseRegex(configuration["specflow:grouping:suites:subSuite"]);

            EpicRegex = ParseRegex(configuration["specflow:grouping:behaviors:epic"]);
            StoryRegex = ParseRegex(configuration["specflow:grouping:behaviors:story"]);

            PackageRegex = ParseRegex(configuration["specflow:grouping:packages:package"]);
            TestClassRegex = ParseRegex(configuration["specflow:grouping:packages:testClass"]);
            TestMethodRegex = ParseRegex(configuration["specflow:grouping:packages:testMethod"]);

            OwnerRegex = ParseRegex(configuration["specflow:labels:owner"]);
            SeverityRegex = ParseRegex(configuration["specflow:labels:severity"]);

            IssueRegex = ParseRegex(configuration["specflow:links:issue"]);
            TmsRegex = ParseRegex(configuration["specflow:links:tms"]);


        }

        private Regex ParseRegex(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                return new Regex(value,
                    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
