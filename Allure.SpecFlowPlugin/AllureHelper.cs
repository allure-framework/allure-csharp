using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Allure.SpecFlowPlugin
{
    static class AllureHelper
    {
        static ScenarioInfo emptyScenarioInfo = new ScenarioInfo(string.Empty);

        static FeatureInfo emptyFeatureInfo = new FeatureInfo(
            CultureInfo.CurrentCulture, string.Empty, string.Empty);

        static PluginConfiguration config = new PluginConfiguration(AllureLifecycle.Instance.Configuration);

        internal static string GetFeatureContainerId(FeatureInfo featureInfo)
        {
            var id = (featureInfo != null)
                ? featureInfo.GetHashCode().ToString()
                : emptyFeatureInfo.GetHashCode().ToString();

            return id;
        }

        internal static string NewId() => Guid.NewGuid().ToString("N");

        internal static FixtureResult GetFixtureResult(HookBinding hook) => new FixtureResult
        {
            name = $"{hook.Method.Name} [{hook.HookOrder}]"
        };

        internal static TestResult StartTestCase(string containerId, FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            var featureInfo = featureContext?.FeatureInfo ?? emptyFeatureInfo;
            var scenarioInfo = scenarioContext?.ScenarioInfo ?? emptyScenarioInfo;
            var tags = GetTags(featureInfo, scenarioInfo);
            var testResult = new TestResult
            {
                uuid = NewId(),
                historyId = scenarioInfo.Title,
                name = scenarioInfo.Title,
                fullName = scenarioInfo.Title,
                labels = new List<Label>
                    {
                        Label.Thread(),
                        Label.Host(),
                        Label.Feature(featureInfo.Title)
                    }
                    .Union(tags.Item1).ToList(),
                links = tags.Item2
            };

            AllureLifecycle.Instance.StartTestCase(containerId, testResult);
            scenarioContext?.Set(testResult);
            featureContext?.Get<HashSet<TestResult>>().Add(testResult);

            return testResult;
        }

        internal static TestResult GetCurrentTestCase(ScenarioContext context)
        {
            context.TryGetValue(out TestResult testresult);
            return testresult;
        }

        internal static TestResultContainer StartTestContainer(FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            var containerId = GetFeatureContainerId(featureContext?.FeatureInfo);

            var scenarioContainer = new TestResultContainer
            {
                uuid = NewId()
            };
            AllureLifecycle.Instance.StartTestContainer(containerId, scenarioContainer);
            scenarioContext?.Set(scenarioContainer);
            featureContext?.Get<HashSet<TestResultContainer>>().Add(scenarioContainer);

            return scenarioContainer;
        }

        internal static TestResultContainer GetCurrentTestConainer(ScenarioContext context)
        {
            context.TryGetValue(out TestResultContainer testresultContainer);
            return testresultContainer;
        }

        private static Tuple<List<Label>, List<Link>> GetTags(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            var result = Tuple.Create(new List<Label>(), new List<Link>());

            var tags = scenarioInfo.Tags
                .Union(featureInfo.Tags)
                .Distinct(StringComparer.CurrentCultureIgnoreCase);

            foreach (var tag in tags)
            {
                var tagValue = tag;
                // issue
                if (TryUpdateValueByMatch(config.IssueRegex, ref tagValue))
                {
                    result.Item2.Add(Link.Issue(tagValue)); continue;
                }
                // tms
                if (TryUpdateValueByMatch(config.TmsRegex, ref tagValue))
                {
                    result.Item2.Add(Link.Tms(tagValue)); continue;
                }
                // parent suite
                if (TryUpdateValueByMatch(config.ParentSuiteRegex, ref tagValue))
                {
                    result.Item1.Add(Label.ParentSuite(tagValue)); continue;
                }
                // suite
                if (TryUpdateValueByMatch(config.SuiteRegex, ref tagValue))
                {
                    result.Item1.Add(Label.Suite(tagValue)); continue;
                }
                // sub suite
                if (TryUpdateValueByMatch(config.SubSuiteRegex, ref tagValue))
                {
                    result.Item1.Add(Label.SubSuite(tagValue)); continue;
                }
                // epic
                if (TryUpdateValueByMatch(config.EpicRegex, ref tagValue))
                {
                    result.Item1.Add(Label.Epic(tagValue)); continue;
                }
                // story
                if (TryUpdateValueByMatch(config.StoryRegex, ref tagValue))
                {
                    result.Item1.Add(Label.Story(tagValue)); continue;
                }
                // package
                if (TryUpdateValueByMatch(config.PackageRegex, ref tagValue))
                {
                    result.Item1.Add(Label.Package(tagValue)); continue;
                }
                // test class
                if (TryUpdateValueByMatch(config.TestClassRegex, ref tagValue))
                {
                    result.Item1.Add(Label.TestClass(tagValue)); continue;
                }
                // test method
                if (TryUpdateValueByMatch(config.TestMethodRegex, ref tagValue))
                {
                    result.Item1.Add(Label.TestMethod(tagValue)); continue;
                }
                // owner
                if (TryUpdateValueByMatch(config.OwnerRegex, ref tagValue))
                {
                    result.Item1.Add(Label.Owner(tagValue)); continue;
                }
                // severity
                if (TryUpdateValueByMatch(config.SeverityRegex, ref tagValue) && Enum.TryParse(tagValue, out SeverityLevel level))
                {
                    result.Item1.Add(Label.Severity(level)); continue;
                }
                // tag
                result.Item1.Add(Label.Tag(tagValue));
            }
            return result;
        }

        private static bool TryUpdateValueByMatch(Regex regex, ref string value)
        {
            if (regex == null)
                return false;

            if (regex.IsMatch(value))
            {
                var groups = regex.Match(value).Groups;
                if (groups?.Count == 1)
                    value = groups[0].Value;
                else
                    value = groups[1].Value;

                return true;
            }
            else
                return false;
        }
    }
}