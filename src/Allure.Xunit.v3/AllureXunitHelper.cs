using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using LegacyAttributes = Allure.Xunit.Attributes;

#nullable enable

namespace Allure.Xunit
{
    internal static class AllureXunitHelper
    {
        internal const string NS_OBSOLETE_MSG =
            "The Allure.XUnit namespace is deprecated. Please, use Allure.Xunit instead";

        internal static TestResultContainer StartNewAllureContainer(string className)
        {
            var container = CreateContainer(className);
            AllureLifecycle.Instance.StartTestContainer(container);
            return container;
        }

        internal static void StartAllureTestCase(TestResult? testResult) =>
            AllureLifecycle.Instance.StartTestCase(testResult ?? throw new ArgumentNullException(nameof(testResult)));

        internal static void ApplyTestFailure(object failureMessage)
        {
            var trace = GetStringList(failureMessage, "StackTraces");
            var message = GetStringList(failureMessage, "Messages");
            var exceptionTypes = GetStringList(failureMessage, "ExceptionTypes");

            var status = exceptionTypes.Any(exceptionType => !exceptionType.StartsWith("Xunit.Sdk.", StringComparison.Ordinal))
                ? Status.broken
                : Status.failed;

            var cause = failureMessage.GetType().GetProperty("Cause")?.GetValue(failureMessage)?.ToString();
            if (string.Equals(cause, "Assertion", StringComparison.Ordinal))
            {
                status = Status.failed;
            }

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.trace = string.Join("\n", trace);
                statusDetails.message = string.Join("\n", message);
                testResult.status = status;
            });
        }

        internal static void ApplyTestSuccess(string? output)
        {
            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = output;
                testResult.status = Status.passed;
            });
        }

        internal static void ApplyTestSkip(string? reason)
        {
            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = reason;
                testResult.status = Status.skipped;
            });
        }

        internal static void ApplyTestParameters(MethodInfo methodInfo, object[] arguments)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length == 0)
            {
                return;
            }

            Parameter[] parametersFromMethod =
            [
                ..ModelFunctions.CreateParameters(
                    parameters.Select(static p => p.Name ?? string.Empty),
                    parameters.Select(static p => p.GetCustomAttribute<AllureParameterAttribute>()),
                    arguments,
                    AllureLifecycle.Instance.TypeFormatters
                )
            ];

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var dynamicParameters = testResult.parameters;
                testResult.parameters = [..parametersFromMethod, ..dynamicParameters];
            });
        }

        internal static void ApplyDefaultSuites(MethodInfo method)
        {
            var testClass = method.DeclaringType;
            var assemblyName = testClass?.Assembly?.GetName().Name;
            var @namespace = testClass?.Namespace;
            var subSuite = ResolveSubSuite(testClass, @namespace);

            AllureLifecycle.Instance.UpdateTestCase(
                testResult => ModelFunctions.EnsureSuites(
                    testResult,
                    assemblyName,
                    @namespace,
                    subSuite
                )
            );
        }

        static string? ResolveSubSuite(Type? testClass, string? @namespace) =>
            (testClass is null ? null : AllureApiAttribute
                .GetTypeAttributes(testClass)
                .OfType<AllureNameAttribute>()
                .LastOrDefault()
                ?.Name)
                ?? (testClass?.FullName is null
                    ? testClass?.Name
                    : string.IsNullOrEmpty(@namespace)
                        ? testClass.Name
                        : testClass.FullName.Substring(@namespace.Length + 1));

        internal static void ReportCurrentTestCase()
        {
            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();
        }

        internal static void ReportCurrentTestContainer()
        {
            AllureLifecycle.Instance.StopTestContainer();
            AllureLifecycle.Instance.WriteTestContainer();
        }

        internal static TestResult CreateTestResult(MethodInfo method, string displayName)
        {
            var testClass = method.DeclaringType;
            var testResult = new TestResult
            {
                name = BuildName(method),
                titlePath = IdFunctions.CreateTitlePath(testClass),
                labels =
                [
                    Label.Thread(),
                    Label.Host(),
                    Label.Language(),
                    Label.Framework("xUnit.net v3"),
                    Label.TestClass(testClass?.FullName ?? testClass?.Name ?? ""),
                    Label.TestMethod(method.Name),
                    Label.Package(testClass?.FullName ?? testClass?.Name ?? ""),
                    ..ModelFunctions.EnumerateEnvironmentLabels(),
                    ..ModelFunctions.EnumerateGlobalLabels(),
                ]
            };

            SetTestResultIdentifiers(method, displayName, testResult);
            ApplyLegacyAllureAttributes(testResult, method);
            ApplyAllureAttributes(testResult, method);
            return testResult;
        }

        static void AddDistinct(this List<Label> labels, Label labelToInsert, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelToInsert.name);
            }

            labels.Add(labelToInsert);
        }

        static void AddDistinct(this List<Label> labels, string labelName, string[] values, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelName);
            }

            foreach (var value in values)
            {
                labels.Add(new Label { name = labelName, value = value });
            }
        }

        static void SetTestResultIdentifiers(MethodInfo method, string displayName, TestResult testResult)
        {
            if (AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds)
            {
                testResult.uuid = NewUuid(displayName);
                testResult.fullName = BuildFullName(method);
                testResult.historyId = displayName;
                return;
            }

            testResult.uuid = IdFunctions.CreateUUID();
            testResult.fullName = IdFunctions.CreateFullName(method);
        }

        static void ApplyAllureAttributes(TestResult testResult, MethodInfo method)
        {
            var testClass = method.DeclaringType;
            if (testClass is not null)
            {
                AllureApiAttribute.ApplyTypeAttributes(testClass, testResult);
            }
            AllureApiAttribute.ApplyMethodAttributes(method, testResult);
        }

        static void ApplyLegacyAllureAttributes(TestResult testResult, MethodInfo method)
        {
            var classAttributes = EnumerateLegacyClassAttributes(method.DeclaringType);
            var methodAttributes = method
                .GetCustomAttributes(inherit: true)
                .OfType<LegacyAttributes.IAllureInfo>();

            foreach (var attribute in classAttributes.Concat(methodAttributes))
            {
                switch (attribute)
                {
                    case LegacyAttributes.AllureFeatureAttribute featureAttribute:
                        testResult.labels.AddDistinct("feature", featureAttribute.Features, featureAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureLinkAttribute linkAttribute:
                        testResult.links.Add(linkAttribute.Link);
                        break;

                    case LegacyAttributes.AllureIssueAttribute issueAttribute:
                        testResult.links.Add(issueAttribute.IssueLink);
                        break;

                    case LegacyAttributes.AllureOwnerAttribute ownerAttribute:
                        testResult.labels.AddDistinct(Label.Owner(ownerAttribute.Owner), ownerAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureSuiteAttribute suiteAttribute:
                        testResult.labels.AddDistinct(Label.Suite(suiteAttribute.Suite), suiteAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureSubSuiteAttribute subSuiteAttribute:
                        testResult.labels.AddDistinct(Label.SubSuite(subSuiteAttribute.SubSuite), subSuiteAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureEpicAttribute epicAttribute:
                        testResult.labels.AddDistinct(Label.Epic(epicAttribute.Epic), epicAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureTagAttribute tagAttribute:
                        testResult.labels.AddDistinct("tag", tagAttribute.Tags, tagAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureSeverityAttribute severityAttribute:
                        testResult.labels.AddDistinct(Label.Severity(severityAttribute.Severity), true);
                        break;

                    case LegacyAttributes.AllureParentSuiteAttribute parentSuiteAttribute:
                        testResult.labels.AddDistinct(Label.ParentSuite(parentSuiteAttribute.ParentSuite), parentSuiteAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureStoryAttribute storyAttribute:
                        testResult.labels.AddDistinct("story", storyAttribute.Stories, storyAttribute.Overwrite);
                        break;

                    case LegacyAttributes.AllureDescriptionAttribute descriptionAttribute:
                        testResult.description = descriptionAttribute.Description;
                        break;

                    case LegacyAttributes.AllureIdAttribute allureIdAttribute:
                        testResult.labels.AddDistinct(new Label { name = "ALLURE_ID", value = allureIdAttribute.AllureId }, false);
                        break;

                    case LegacyAttributes.AllureLabelAttribute labelAttribute:
                        testResult.labels.AddDistinct(
                            new Label { name = labelAttribute.Label, value = labelAttribute.Value },
                            labelAttribute.Overwrite
                        );
                        break;
                }
            }
        }

        static IEnumerable<LegacyAttributes.IAllureInfo> EnumerateLegacyClassAttributes(Type? type)
        {
            if (type is null)
            {
                return [];
            }

            IEnumerable<Type> TraverseBaseTypes(Type t)
            {
                var chain = new Stack<Type>();
                var current = t;
                while (current is not null)
                {
                    chain.Push(current);
                    current = current.BaseType;
                }
                return chain;
            }

            var interfaces = type.GetInterfaces();
            var baseChain = TraverseBaseTypes(type);

            return interfaces
                .Concat(baseChain)
                .Distinct()
                .SelectMany(static t => t.GetCustomAttributes(inherit: false).OfType<LegacyAttributes.IAllureInfo>());
        }

        static string NewUuid(string name) =>
            string.Concat(Guid.NewGuid().ToString(), "-", name);

        static string BuildName(MethodInfo method) =>
            MaybeGetExplicitDisplayName(method)
                ?? method.Name;

        static string? MaybeGetExplicitDisplayName(MethodInfo method)
        {
            foreach (var attr in method.GetCustomAttributes(inherit: true))
            {
                var attrType = attr.GetType();
                var typeName = attrType.Name;
                if (!typeName.EndsWith("FactAttribute", StringComparison.Ordinal)
                    && !typeName.EndsWith("TheoryAttribute", StringComparison.Ordinal))
                {
                    continue;
                }

                var displayName = attrType.GetProperty("DisplayName")?.GetValue(attr) as string;
                if (!string.IsNullOrEmpty(displayName))
                {
                    return displayName;
                }
            }

            return null;
        }

        static string BuildFullName(MethodInfo method)
        {
            var parameters = method
                .GetParameters()
                .Select(static parameter => string.Format(
                    "{0} {1}",
                    parameter.ParameterType.FullName ?? parameter.ParameterType.Name,
                    parameter.Name
                )).ToArray();

            var parametersSegment = parameters.Any()
                ? $"({string.Join(", ", parameters)})"
                : string.Empty;

            return string.Format(
                "{0}.{1}{2}",
                method.DeclaringType?.FullName ?? method.DeclaringType?.Name,
                method.Name,
                parametersSegment
            );
        }

        static List<string> GetStringList(object source, string propertyName)
        {
            var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
            return value switch
            {
                IEnumerable<string> strings => [..strings],
                _ => []
            };
        }

        static TestResultContainer CreateContainer(string className) => new()
        {
            uuid = AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds
                ? NewUuid(className)
                : IdFunctions.CreateUUID(),
            name = className
        };
    }
}