using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Allure.Net.Commons;
using Allure.XUnit;
using Allure.Xunit.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Allure.Xunit
{
    public static class AllureXunitHelper
    {
        static AllureXunitHelper()
        {
            const string allureConfigEnvVariable = "ALLURE_CONFIG";
            const string allureConfigName = "allureConfig.json";

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(allureConfigEnvVariable)))
            {
                return;
            }

            var allureConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, allureConfigName);

            Environment.SetEnvironmentVariable(allureConfigEnvVariable, allureConfigPath);
        }

        public static void StartTestContainer(ITestCaseStarting testCaseStarting)
        {
            if (testCaseStarting.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            StartTestContainer(testCaseStarting.TestClass, testResults);
        }

        public static void StartTestCase(ITestCaseMessage testCaseMessage)
        {
            if (testCaseMessage.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            var testCase = testCaseMessage.TestCase;
            var uuid = NewUuid(testCase.DisplayName);
            testResults.TestResult = new()
            {
                uuid = uuid,
                name = BuildName(testCase),
                historyId = testCase.DisplayName,
                fullName = BuildFullName(testCase),
                labels = new()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.TestClass(testCase.TestMethod.TestClass.Class.Name),
                    Label.TestMethod(testCase.DisplayName),
                    Label.Package(testCase.TestMethod.TestClass.Class.Name)
                },
                parameters = testCase.TestMethod.Method.GetParameters()
                    .Zip(testCase.TestMethodArguments ?? Array.Empty<object>(), (param, value) => new Parameter
                    {
                        name = param.Name,
                        value = value?.ToString() ?? "null"
                    })
                    .ToList()
            };
            UpdateTestDataFromAttributes(testResults.TestResult, testCase);
            AllureLifecycle.Instance.StartTestCase(testResults.TestResultContainer.uuid, testResults.TestResult);
        }

        public static void MarkTestCaseAsFailedOrBroken(ITestFailed testFailed)
        {
            if (testFailed.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            var statusDetails = testResults.TestResult.statusDetails ??= new();
            statusDetails.trace = string.Join('\n', testFailed.StackTraces);
            statusDetails.message = string.Join('\n', testFailed.Messages);

            if (testFailed.ExceptionTypes.Any(exceptionType => !exceptionType.StartsWith("Xunit.Sdk.")))
            {
                testResults.TestResult.status = Status.broken;
                return;
            }
            testResults.TestResult.status = Status.failed;
        }

        public static void MarkTestCaseAsPassed(ITestPassed testPassed)
        {
            if (testPassed.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            var statusDetails = testResults.TestResult.statusDetails ??= new();
            statusDetails.message = testPassed.Output;
            testResults.TestResult.status = Status.passed;
        }

        public static void MarkTestCaseAsSkipped(ITestCaseMessage testCaseMessage)
        {
            if (testCaseMessage.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            var statusDetails = testResults.TestResult.statusDetails ??= new();
            statusDetails.message = testCaseMessage.TestCase.SkipReason;
            testResults.TestResult.status = Status.skipped;
        }

        public static void FinishTestCase(ITestCaseMessage testCaseMessage)
        {
            if (testCaseMessage.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            AllureLifecycle.Instance.StopTestCase(testResults.TestResult.uuid);
            AllureLifecycle.Instance.WriteTestCase(testResults.TestResult.uuid);
            AllureLifecycle.Instance.StopTestContainer(testResults.TestResultContainer.uuid);
            AllureLifecycle.Instance.WriteTestContainer(testResults.TestResultContainer.uuid);
        }

        private static void StartTestContainer(ITestClass testClass, ITestResultAccessor testResult)
        {
            var uuid = NewUuid(testClass.Class.Name);
            testResult.TestResultContainer = new()
            {
                uuid = uuid,
                name = testClass.Class.Name
            };
            AllureLifecycle.Instance.StartTestContainer(testResult.TestResultContainer);
        }

        private static string NewUuid(string name)
        {
            var uuid = string.Concat(Guid.NewGuid().ToString(), "-", name);
            return uuid;
        }

        internal static void Log(string message)
        {
            AllureMessageBus.TestOutputHelper.Value.WriteLine("╬════════════════════════");
            AllureMessageBus.TestOutputHelper.Value.WriteLine($"║ {message}");
            AllureMessageBus.TestOutputHelper.Value.WriteLine("╬═══════════════");
        }

        private static void AddDistinct(this List<Label> labels, Label labelToInsert, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelToInsert.name);
            }
            
            labels.Add(labelToInsert);
        }

        private static void AddDistinct(this List<Label> labels, string labelName, string[] values, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelName);
            }

            foreach (var value in values)
            {
                labels.Add(new Label {name = labelName, value = value});
            }
        }

        private static void UpdateTestDataFromAttributes(TestResult testResult, ITestCase testCase)
        {
            var classAttributes = GetCustomAttributesRecursive(testCase.TestMethod.TestClass.Class, typeof(IAllureInfo));
            var methodAttributes = testCase.TestMethod.Method.GetCustomAttributes(typeof(IAllureInfo));

            foreach (var attribute in classAttributes.Concat(methodAttributes))
            {
                switch (((IReflectionAttributeInfo) attribute).Attribute)
                {
                    case AllureFeatureAttribute featureAttribute:
                        testResult.labels.AddDistinct("feature", featureAttribute.Features, featureAttribute.Overwrite);
                        break;

                    case AllureLinkAttribute linkAttribute:
                        testResult.links.Add(linkAttribute.Link);
                        break;

                    case AllureIssueAttribute issueAttribute:
                        testResult.links.Add(issueAttribute.IssueLink);
                        break;

                    case AllureOwnerAttribute ownerAttribute:
                        testResult.labels.AddDistinct(Label.Owner(ownerAttribute.Owner), ownerAttribute.Overwrite);
                        break;

                    case AllureSuiteAttribute suiteAttribute:
                        testResult.labels.AddDistinct(Label.Suite(suiteAttribute.Suite), suiteAttribute.Overwrite);
                        break;

                    case AllureSubSuiteAttribute subSuiteAttribute:
                        testResult.labels.AddDistinct(Label.SubSuite(subSuiteAttribute.SubSuite), subSuiteAttribute.Overwrite);
                        break;

                    case AllureEpicAttribute epicAttribute:
                        testResult.labels.AddDistinct(Label.Epic(epicAttribute.Epic), epicAttribute.Overwrite);
                        break;

                    case AllureTagAttribute tagAttribute:
                        testResult.labels.AddDistinct("tag", tagAttribute.Tags, tagAttribute.Overwrite);
                        break;

                    case AllureSeverityAttribute severityAttribute:
                        testResult.labels.AddDistinct(Label.Severity(severityAttribute.Severity), true);
                        break;

                    case AllureParentSuiteAttribute parentSuiteAttribute:
                        testResult.labels.AddDistinct(Label.ParentSuite(parentSuiteAttribute.ParentSuite), parentSuiteAttribute.Overwrite);
                        break;

                    case AllureStoryAttribute storyAttribute:
                        testResult.labels.AddDistinct("story", storyAttribute.Stories, storyAttribute.Overwrite);
                        break;

                    case AllureDescriptionAttribute descriptionAttribute:
                        testResult.description = descriptionAttribute.Description;
                        break;
                    
                    case AllureIdAttribute allureIdAttribute:
                        var allureIdLabel = new Label {name = "ALLURE_ID", value = allureIdAttribute.AllureId};
                        testResult.labels.AddDistinct(allureIdLabel, false);
                        break;

                    case AllureLabelAttribute labelAttribute:
                        var label = new Label()
                        {
                            name = labelAttribute.Label,
                            value = labelAttribute.Value
                        };
                        testResult.labels.AddDistinct(label, labelAttribute.Overwrite);
                        break;
                }
            }
        }

        private static string BuildName(ITestCase testCase)
        {
            var factAttribute = testCase.TestMethod.Method.GetCustomAttributes(typeof(FactAttribute)).SingleOrDefault();
            if (factAttribute is null)
            {
                return BuildFullName(testCase);
            }

            var displayName = factAttribute.GetNamedArgument<string>("DisplayName");
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return BuildFullName(testCase);
            }
            
            return displayName;
        }
        
        private static string BuildFullName(ITestCase testCase)
        {
            var parameters = testCase.TestMethod.Method
                .GetParameters()
                .Select(parameter =>
                    $"{parameter.ParameterType.ToRuntimeType().GetFullFormattedTypeName()} {parameter.Name}")
                .ToArray();
            var parametersSegment = parameters.Any()
                ? $"({string.Join(", ", parameters)})"
                : string.Empty;

            return $"{testCase.TestMethod.TestClass.Class.Name}.{testCase.TestMethod.Method.Name}{parametersSegment}";
        }

        private static IEnumerable<IAttributeInfo> GetCustomAttributesRecursive(ITypeInfo typeInfo, Type attributeType)
        {
            foreach (var type in GetInheritanceTree(typeInfo).Reverse())
            {
                foreach (var attribute in type.GetCustomAttributes(attributeType.AssemblyQualifiedName))
                    yield return attribute;
            }

            static IEnumerable<ITypeInfo> GetInheritanceTree(ITypeInfo typeInfo)
            {
                var currentType = typeInfo;
                while (currentType.ToRuntimeType() != typeof(object))
                {
                    yield return currentType;
                    currentType = currentType.BaseType;
                }
            }
        }
    }
}