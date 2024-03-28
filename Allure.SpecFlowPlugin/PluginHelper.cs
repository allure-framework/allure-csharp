﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Allure.SpecFlowPlugin
{
    public static class PluginHelper
    {
        public static string IGNORE_EXCEPTION = "IgnoreException";
        const string LABELS_AND_LINKS_CACHE_KEY = "LABELS_AND_LINKS";

        internal static PluginConfiguration PluginConfiguration =
            GetConfiguration(AllureLifecycle.Instance.JsonConfiguration);

        public static PluginConfiguration GetConfiguration(
            string allureConfiguration
        )
        {
            var config = new PluginConfiguration();
            var configJson = JObject.Parse(allureConfiguration);
            var specflowSection = configJson["specflow"];
            if (specflowSection != null)
            {
                config = specflowSection.ToObject<PluginConfiguration>()
                    ?? throw new NullReferenceException();
            }

            return config;
        }

        internal static string GetFeatureContainerId(
            FeatureInfo featureInfo
        ) => featureInfo.GetHashCode().ToString();

        internal static string NewId() => Guid.NewGuid().ToString("N");

        internal static FixtureResult GetFixtureResult(HookBinding hook) =>
            new()
            {
                name = $"{hook.Method.Name} [{hook.HookOrder}]"
            };

        internal static void StartTestContainer() =>
            AllureLifecycle.Instance.StartTestContainer(new()
            {
                uuid = AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds
                    ? NewId()
                    : IdFunctions.CreateUUID()
            });

        internal static void StartTestCase(
            ITestRunnerManager testRunnerManager,
            FeatureInfo featureInfo,
            ScenarioContext scenarioContext
        ) =>
            StartTestCase(
                testRunnerManager,
                featureInfo,
                scenarioContext.ScenarioInfo,
                GetOrParseLabelsAndLinks(featureInfo, scenarioContext)
            );

        internal static void StartTestCase(
            ITestRunnerManager testRunnerManager,
            FeatureInfo featureInfo,
            ScenarioInfo scenarioInfo
        ) =>
            StartTestCase(
                testRunnerManager,
                featureInfo,
                scenarioInfo,
                ParseLabelsAndLinks(featureInfo, scenarioInfo)
            );

        internal static void StartTestCase(
            ITestRunnerManager testRunnerManager,
            FeatureInfo featureInfo,
            ScenarioInfo scenarioInfo,
            (List<Label>, List<Link>) allureMetadata
        )
        {
            var (labels, links) = allureMetadata;
            var (parameters, parametersHash) = GetParameters(scenarioInfo);
            var title = scenarioInfo.Title;
            var testResult = new TestResult
            {
                name = title,
                description = scenarioInfo.Description,
                labels = new List<Label>
                {
                    Label.Thread(),
                    string.IsNullOrWhiteSpace(
                        AllureLifecycle.Instance.AllureConfiguration.Title
                    ) ? Label.Host() : Label.Host(
                        AllureLifecycle.Instance.AllureConfiguration.Title
                    ),
                    Label.Language(),
                    Label.Framework("SpecFlow"),
                    Label.Feature(featureInfo.Title)
                }
                    .Union(labels).ToList(),
                links = links,
                parameters = parameters
            };
            SetTestResultIdentifiers(
                testRunnerManager,
                featureInfo,
                testResult,
                title,
                parametersHash
            );

            AllureLifecycle.Instance.StartTestCase(testResult);
        }

        internal static (List<Label> labels, List<Link> links) GetOrParseLabelsAndLinks(
            FeatureInfo featureInfo,
            ScenarioContext scenarioContext
        )
        {
            var cacheHit = scenarioContext.TryGetValue(
                LABELS_AND_LINKS_CACHE_KEY,
                out (List<Label>, List<Link>) items
            );
            if (!cacheHit)
            {
                items = ParseLabelsAndLinks(
                    featureInfo,
                    scenarioContext.ScenarioInfo
                );
                scenarioContext.Set(items, LABELS_AND_LINKS_CACHE_KEY);
            }
            return items;
        }

        internal static StatusDetails GetStatusDetails(Exception ex) =>
            new()
            {
                message = GetFullExceptionMessage(ex),
                trace = ex.ToString()
            };

        internal static T CaptureAllureContext<T>(
            SpecFlowContext specFlowContext,
            Func<T> fn
        )
        {
            T result = fn();
            specFlowContext.Set(AllureLifecycle.Instance.Context);
            return result;
        }

        internal static void UseCapturedAllureContext(
            SpecFlowContext specFlowContext,
            Action fn
        ) => AllureLifecycle.Instance.RunInContext(
            specFlowContext.Get<AllureContext>(),
            fn
        );

        internal static T UseCapturedAllureContext<T>(
            SpecFlowContext specFlowContext,
            Func<T> fn
        )
        {
            T? result = default;
            UseCapturedAllureContext(
                specFlowContext,
                () => { result = fn(); }
            );
            return result!;
        }

        internal static Action<ExecutableItem> WrapStatusUpdate(
            Status status,
            StatusDetails? statusDetails = null
        ) => item =>
        {
            item.status = status;
            item.statusDetails = statusDetails;
        };

        internal static Action<ExecutableItem> WrapStatusOverwrite(
            Status status,
            StatusDetails? statusDetails,
            params Status[] statusesToOverwrite
        )
        {
            var updateStatus = WrapStatusUpdate(status, statusDetails);
            return item =>
            {
                if (statusesToOverwrite.Contains(item.status))
                {
                    updateStatus(item);
                }
            };
        }

        internal static Action<ExecutableItem> WrapStatusInit(
            Status status,
            StatusDetails statusDetails
        ) =>
            WrapStatusOverwrite(status, statusDetails, Status.none);

        internal static Action<ExecutableItem> WrapStatusInit(
            Status status
        ) =>
            WrapStatusOverwrite(status, null, Status.none);

        internal static Action<ExecutableItem> WrapStatusInit(
            Status status,
            Exception error
        )
        {
            var updateStatus = WrapStatusUpdate(
                status,
                PluginHelper.GetStatusDetails(error)
            );
            return item =>
            {
                if (item.status is Status.none)
                {
                    updateStatus(item);
                }
            };
        }

        internal static bool IsIgnoreException(Exception e) =>
            e?.GetType().Name.Contains(IGNORE_EXCEPTION) is true;

        internal static Status ResolveErrorStatus(Exception error) =>
            ModelFunctions.ResolveErrorStatus(
                AllureLifecycle.Instance.AllureConfiguration.FailExceptions,
                error
            );

        internal static Action<TestResult> TestStatusResolver(
            ScenarioContext scenarioContext
        ) =>
            test => (test.status, test.statusDetails) = (
                ResolveTestCaseStatus(scenarioContext, test.status),
                ResolveTestCaseDetails(
                    scenarioContext,
                    test.status,
                    test.statusDetails
                )
            );

        static void SetTestResultIdentifiers(
            ITestRunnerManager testRunnerManager,
            FeatureInfo featureInfo,
            TestResult testResult,
            string scenarioTitle,
            string parametersHash
        )
        {
            if (AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds)
            {
                SetLegacyTestResultIdentifiers(
                    testResult,
                    scenarioTitle,
                    parametersHash
                );
                return;
            }

            testResult.uuid = IdFunctions.CreateUUID();
            testResult.fullName = CreateFullName(
                testRunnerManager,
                featureInfo,
                scenarioTitle
            );
        }

        static void SetLegacyTestResultIdentifiers(
            TestResult testResult,
            string scenarioTitle,
            string parametersHash
        )
        {
            testResult.uuid = NewId();
            testResult.historyId = scenarioTitle + parametersHash;
            testResult.fullName = scenarioTitle;
        }

        static string CreateFullName(
            ITestRunnerManager testRunnerManager,
            FeatureInfo featureInfo,
            string scenarioTitle
        ) =>
            string.Join(
                "/",
                new[]
                {
                    testRunnerManager.TestAssembly.GetName().Name,
                    featureInfo.FolderPath,
                    EscapeFullNamePart(featureInfo.Title),
                    EscapeFullNamePart(scenarioTitle)
                }
            );

        static string EscapeFullNamePart(string part) =>
            part.Replace("/", "\\/");

        static Status ResolveTestCaseStatus(
            ScenarioContext scenarioContext,
            Status status
        ) =>
            status switch
            {
                Status.none =>
                    scenarioContext.ScenarioExecutionStatus switch
                    {
                        ScenarioExecutionStatus.OK => Status.passed,
                        ScenarioExecutionStatus.Skipped => Status.skipped,
                        ScenarioExecutionStatus.TestError
                            when IsIgnoreException(
                                scenarioContext.TestError
                            ) => Status.skipped,
                        ScenarioExecutionStatus.TestError =>
                            ResolveErrorStatus(
                                scenarioContext.TestError
                            ),
                        _ => Status.broken
                    },
                _ => status
            };

        static StatusDetails ResolveTestCaseDetails(
            ScenarioContext scenarioContext,
            Status status,
            StatusDetails statusDetails
        ) =>
            status switch
            {
                Status.none
                    when scenarioContext.ScenarioExecutionStatus
                        is ScenarioExecutionStatus.TestError
                    => GetStatusDetails(scenarioContext.TestError),
                _ => statusDetails
            };

        static string GetFullExceptionMessage(Exception ex) =>
            ex.Message + (
                !string.IsNullOrWhiteSpace(ex.InnerException?.Message)
                    ? $" -> {GetFullExceptionMessage(ex.InnerException!)}"
                    : string.Empty
            );

        static (List<Label> labels, List<Link> links) ParseLabelsAndLinks(
            FeatureInfo featureInfo,
            ScenarioInfo scenarioInfo
        )
        {
            var labels = new List<Label>();
            var links = new List<Link>();

            var tags = scenarioInfo.Tags
                .Union(featureInfo.Tags)
                .Distinct(StringComparer.CurrentCultureIgnoreCase);

            foreach (var tag in tags)
            {
                var tagValue = tag;
                // link
                if (TryUpdateValueByMatch(PluginConfiguration.links.link, ref tagValue))
                {
                    links.Add(new()
                    {
                        name = tagValue,
                        url = tagValue
                    });
                    continue;
                }

                // issue
                if (TryUpdateValueByMatch(PluginConfiguration.links.issue, ref tagValue))
                {
                    links.Add(
                        Link.Issue(tagValue, tagValue)
                    );
                    continue;
                }

                // tms
                if (TryUpdateValueByMatch(PluginConfiguration.links.tms, ref tagValue))
                {
                    links.Add(
                        Link.Tms(tagValue, tagValue)
                    );
                    continue;
                }

                // parent suite
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.suites.parentSuite, ref tagValue))
                {
                    labels.Add(
                        Label.ParentSuite(tagValue)
                    );
                    continue;
                }

                // suite
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.suites.suite, ref tagValue))
                {
                    labels.Add(
                        Label.Suite(tagValue)
                    );
                    continue;
                }

                // sub suite
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.suites.subSuite, ref tagValue))
                {
                    labels.Add(
                        Label.SubSuite(tagValue)
                    );
                    continue;
                }

                // epic
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.behaviors.epic, ref tagValue))
                {
                    labels.Add(
                        Label.Epic(tagValue)
                    );
                    continue;
                }

                // story
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.behaviors.story, ref tagValue))
                {
                    labels.Add(
                        Label.Story(tagValue)
                    );
                    continue;
                }

                // package
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.packages.package, ref tagValue))
                {
                    labels.Add(
                        Label.Package(tagValue)
                    );
                    continue;
                }

                // test class
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.packages.testClass, ref tagValue))
                {
                    labels.Add(
                        Label.TestClass(tagValue)
                    );
                    continue;
                }

                // test method
                if (TryUpdateValueByMatch(PluginConfiguration.grouping.packages.testMethod, ref tagValue))
                {
                    labels.Add(
                        Label.TestMethod(tagValue)
                    );
                    continue;
                }

                // owner
                if (TryUpdateValueByMatch(PluginConfiguration.labels.owner, ref tagValue))
                {
                    labels.Add(
                        Label.Owner(tagValue)
                    );
                    continue;
                }

                // severity
                if (TryUpdateValueByMatch(PluginConfiguration.labels.severity, ref tagValue) &&
                    Enum.TryParse(tagValue, out SeverityLevel level))
                {
                    labels.Add(
                        Label.Severity(level)
                    );
                    continue;
                }

                // label
                if (GetLabelProps(PluginConfiguration.labels.label, tagValue, out var props))
                {
                    labels.Add(new()
                    {
                        name = props.Key,
                        value = props.Value
                    });
                    continue;
                }

                // tag
                labels.Add(
                    Label.Tag(tagValue)
                );
            }

            return (labels, links);
        }

        static (List<Parameter> parameters, string hash) GetParameters(
            ScenarioInfo scenarioInfo
        )
        {
            var sb = new StringBuilder();
            var parameters = new List<Parameter>();
            var argumentsEnumerator = scenarioInfo.Arguments.GetEnumerator();
            while (argumentsEnumerator.MoveNext())
            {
                var key = argumentsEnumerator.Key.ToString();
                var value = FormatFunctions.Format(
                    argumentsEnumerator.Value,
                    AllureLifecycle.Instance.TypeFormatters
                );
                sb.Append(key);
                sb.Append(value);

                parameters.Add(new()
                {
                    name = key.ToString(),
                    value = value.ToString()
                });
            }
            var hash = (parameters.Count > 0)
                ? sb.ToString().GetDeterministicHashCode().ToString()
                : string.Empty;
            return (parameters, hash);
        }

        static bool TryUpdateValueByMatch(
            string? expression,
            ref string? value
        )
        {
            var matchedGroups = GetMatchedGroups(expression, value);

            if (!matchedGroups.Any())
            {
                return false;
            }

            value = matchedGroups.Count == 1
                ? matchedGroups[0]
                : matchedGroups[1];

            return true;
        }

        static bool GetLabelProps(
            string? expression,
            string? value,
            out KeyValuePair<string, string> props
        )
        {
            props = default;

            var matchedGroups = GetMatchedGroups(expression, value);

            if (matchedGroups.Count != 3)
            {
                return false;
            }

            props = new(matchedGroups[1], matchedGroups[2]);
            return true;

        }

        static List<string> GetMatchedGroups(
            string? expression,
            string? value
        )
        {
            var matchedGroups = new List<string>();
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(expression))
            {
                return matchedGroups;
            }

            Regex regex;
            try
            {
                regex = new Regex(
                    expression,
                    RegexOptions.Compiled
                        | RegexOptions.Singleline
                        | RegexOptions.IgnoreCase
                );
            }
            catch (Exception)
            {
                return matchedGroups;
            }

            if (regex == null)
            {
                return matchedGroups;
            }

            if (regex.IsMatch(value))
            {
                var groups = regex.Match(value).Groups;

                for (var i = 0; i < groups.Count; i++)
                {
                    matchedGroups.Add(groups[i].Value);
                }

                return matchedGroups;
            }

            return matchedGroups;
        }

        static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
