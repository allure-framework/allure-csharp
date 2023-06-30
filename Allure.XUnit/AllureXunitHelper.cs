using Allure.Net.Commons;
using Allure.Net.Commons.Storage;
using Allure.Xunit.Attributes;
using Allure.XUnit;
using Allure.XUnit.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.Xunit
{
    public static class AllureXunitHelper
    {
        internal static List<Type> ExceptionTypes = new()
        {
            typeof(XunitException)
        };

        static AllureXunitHelper()
        {
            const string allureConfigEnvVariable = "ALLURE_CONFIG";
            const string allureConfigName = "allureConfig.json";

            var allureConfigPath = Environment.GetEnvironmentVariable(
                allureConfigEnvVariable
            );
            if (!string.IsNullOrEmpty(allureConfigPath))
            {
                return;
            }

            allureConfigPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                allureConfigName
            );

            Environment.SetEnvironmentVariable(
                allureConfigEnvVariable,
                allureConfigPath
            );
        }

        internal static TestResult StartStaticAllureTestCase(ITest test)
        {
            var testResult = CreateTestResultByTest(test);
            AllureLifecycle.Instance.StartTestCase(testResult);
            return testResult;
        }

        internal static TestResultContainer StartNewAllureContainer(
            string className
        )
        {
            var container = new TestResultContainer
            {
                uuid = NewUuid(className),
                name = className
            };
            AllureLifecycle.Instance.StartTestContainer(container);
            return container;
        }

        internal static TestResult StartAllureTestCase(
            ITest test,
            TestResultContainer container
        )
        {
            var testResult = CreateTestResultByTest(test);
            AllureLifecycle.Instance.StartTestCase(container.uuid, testResult);
            return testResult;
        }

        internal static void ApplyTestFailure(
            TestResult testResult,
            IFailureInformation failure
        )
        {
            var statusDetails = testResult.statusDetails ??= new();
            statusDetails.trace = string.Join('\n', failure.StackTraces);
            statusDetails.message = string.Join('\n', failure.Messages);

            testResult.status = failure.ExceptionTypes.Any(
                exceptionType => !exceptionType.StartsWith("Xunit.Sdk.")
            ) ? Status.broken : Status.failed;
        }

        internal static void ApplyTestSuccess(
            TestResult testResult,
            ITestResultMessage message
        )
        {
            var statusDetails = testResult.statusDetails ??= new();
            statusDetails.message = message.Output;
            testResult.status = Status.passed;
        }

        internal static void ApplyTestParameters(
            TestResult testResult,
            IEnumerable<IParameterInfo> parameters,
            object[] arguments
        ) => testResult.parameters = parameters.Zip(
            arguments,
            (param, value) => new Parameter
            {
                name = param.Name,
                value = value?.ToString() ?? "null"
            }
        ).ToList();

        internal static void ReportTestCase(TestResult testResult)
        {
            AllureLifecycle.Instance.StopTestCase(testResult.uuid);
            AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
        }

        internal static void ReportTestContainer(TestResultContainer container)
        {
            AllureLifecycle.Instance.StopTestContainer(container.uuid);
            AllureLifecycle.Instance.WriteTestContainer(container.uuid);
        }

        internal static void ReportSkippedTestCase(ITestCase testCase)
        {
            var testResult = CreateTestResultByTestCase(testCase);
            ApplyTestSkip(testResult, testCase.SkipReason);
            AllureLifecycle.Instance.StartTestCase(testResult);
            ReportTestCase(testResult);
        }

        static TestResult CreateTestResultByTest(ITest test) =>
            CreateTestResult(test.TestCase, test.DisplayName);

        static TestResult CreateTestResultByTestCase(ITestCase testCase) =>
            CreateTestResult(testCase, testCase.DisplayName);

        static TestResult CreateTestResult(
            ITestCase testCase,
            string displayName
        )
        {
            var testMethod = testCase.TestMethod;
            var testResult = new TestResult
            {
                uuid = NewUuid(displayName),
                name = BuildName(testCase),
                historyId = displayName,
                fullName = BuildFullName(testCase),
                labels = new()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.TestClass(testMethod.TestClass.Class.Name),
                    Label.TestMethod(displayName),
                    Label.Package(testMethod.TestClass.Class.Name)
                }
            };
            UpdateTestDataFromAttributes(testResult, testMethod);
            return testResult;
        }

        static void ApplyTestSkip(
            TestResult testResult,
            string reason
        )
        {
            var statusDetails = testResult.statusDetails ??= new();
            statusDetails.message = reason;
            testResult.status = Status.skipped;
        }

        static void AddDistinct(
            this List<Label> labels,
            Label labelToInsert,
            bool overwrite
        )
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelToInsert.name);
            }
            
            labels.Add(labelToInsert);
        }

        static void AddDistinct(
            this List<Label> labels,
            string labelName,
            string[] values, bool overwrite
        )
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

        static void UpdateTestDataFromAttributes(
            TestResult testResult,
            ITestMethod method
        )
        {
            var classAttributes = method.TestClass.Class.GetCustomAttributes(
                typeof(IAllureInfo)
            );
            var methodAttributes = method.Method.GetCustomAttributes(
                typeof(IAllureInfo)
            );

            foreach (var attribute in classAttributes.Concat(methodAttributes))
            {
                switch (((IReflectionAttributeInfo)attribute).Attribute)
                {
                    case AllureFeatureAttribute featureAttribute:
                        testResult.labels.AddDistinct(
                            "feature",
                            featureAttribute.Features,
                            featureAttribute.Overwrite
                        );
                        break;

                    case AllureLinkAttribute linkAttribute:
                        testResult.links.Add(linkAttribute.Link);
                        break;

                    case AllureIssueAttribute issueAttribute:
                        testResult.links.Add(issueAttribute.IssueLink);
                        break;

                    case AllureOwnerAttribute ownerAttribute:
                        testResult.labels.AddDistinct(
                            Label.Owner(ownerAttribute.Owner),
                            ownerAttribute.Overwrite
                        );
                        break;

                    case AllureSuiteAttribute suiteAttribute:
                        testResult.labels.AddDistinct(
                            Label.Suite(suiteAttribute.Suite),
                            suiteAttribute.Overwrite
                        );
                        break;

                    case AllureSubSuiteAttribute subSuiteAttribute:
                        testResult.labels.AddDistinct(
                            Label.SubSuite(subSuiteAttribute.SubSuite),
                            subSuiteAttribute.Overwrite
                        );
                        break;

                    case AllureEpicAttribute epicAttribute:
                        testResult.labels.AddDistinct(
                            Label.Epic(epicAttribute.Epic),
                            epicAttribute.Overwrite
                        );
                        break;

                    case AllureTagAttribute tagAttribute:
                        testResult.labels.AddDistinct(
                            "tag",
                            tagAttribute.Tags,
                            tagAttribute.Overwrite
                        );
                        break;

                    case AllureSeverityAttribute severityAttribute:
                        testResult.labels.AddDistinct(
                            Label.Severity(severityAttribute.Severity),
                            true
                        );
                        break;

                    case AllureParentSuiteAttribute parentSuiteAttribute:
                        testResult.labels.AddDistinct(
                            Label.ParentSuite(parentSuiteAttribute.ParentSuite),
                            parentSuiteAttribute.Overwrite
                        );
                        break;

                    case AllureStoryAttribute storyAttribute:
                        testResult.labels.AddDistinct(
                            "story",
                            storyAttribute.Stories,
                            storyAttribute.Overwrite
                        );
                        break;

                    case AllureDescriptionAttribute descriptionAttribute:
                        testResult.description = descriptionAttribute.Description;
                        break;

                    case AllureIdAttribute allureIdAttribute:
                        var allureIdLabel = new Label
                        {
                            name = "ALLURE_ID",
                            value = allureIdAttribute.AllureId
                        };
                        testResult.labels.AddDistinct(allureIdLabel, false);
                        break;

                    case AllureLabelAttribute labelAttribute:
                        var label = new Label()
                        {
                            name = labelAttribute.Label,
                            value = labelAttribute.Value
                        };
                        testResult.labels.AddDistinct(
                            label,
                            labelAttribute.Overwrite
                        );
                        break;
                }
            }
        }

        static string NewUuid(string name) =>
            string.Concat(Guid.NewGuid().ToString(), "-", name);

        static string BuildName(ITestCase testCase) =>
            testCase.TestMethod.Method.GetCustomAttributes(
                typeof(FactAttribute)
            ).SingleOrDefault()?.GetNamedArgument<string>(
                "DisplayName"
            ) ?? BuildFullName(testCase);

        static string BuildFullName(ITestCase testCase)
        {
            var parameters = testCase.TestMethod.Method
                .GetParameters()
                .Select(parameter => string.Format(
                    "{0} {1}",
                    parameter
                        .ParameterType
                        .ToRuntimeType()
                        .GetFullFormattedTypeName(),
                    parameter.Name
                )).ToArray();
            var parametersSegment = parameters.Any()
                ? $"({string.Join(", ", parameters)})"
                : string.Empty;

            return string.Format(
                "{0}.{1}{2}",
                testCase.TestMethod.TestClass.Class.Name,
                testCase.TestMethod.Method.Name,
                parametersSegment
            );
        }

        #region Obsolete public methods
        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void StartTestContainer(
            ITestCaseStarting testCaseStarting
        )
        {
            var testCase = testCaseStarting.TestCase;
            if (testCase is not ITestResultAccessor testResults)
            {
                return;
            }

            testResults.TestResultContainer = StartNewAllureContainer(
                testCaseStarting.TestClass.Class.Name
            );
        }

        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void StartTestCase(ITestCaseMessage testCaseMessage)
        {
            var testCase = testCaseMessage.TestCase;
            if (testCase is not ITestResultAccessor testResults)
            {
                return;
            }

            testResults.TestResult = CreateTestResultByTestCase(testCase);
            ApplyTestParameters(
                testResults.TestResult,
                testCase.TestMethod.Method.GetParameters(),
                testCase.TestMethodArguments
            );
            AllureLifecycle.Instance.StartTestCase(
                testResults.TestResultContainer.uuid,
                testResults.TestResult
            );
        }

        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MarkTestCaseAsFailedOrBroken(ITestFailed testFailed)
        {
            if (testFailed.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            ApplyTestFailure(testResults.TestResult, testFailed);
        }

        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MarkTestCaseAsPassed(ITestPassed testPassed)
        {
            if (testPassed.TestCase is not ITestResultAccessor testResults)
            {
                return;
            }

            ApplyTestSuccess(testResults.TestResult, testPassed);
        }

        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MarkTestCaseAsSkipped(
            ITestCaseMessage testCaseMessage
        )
        {
            var testCase = testCaseMessage.TestCase;
            if (testCase is not ITestResultAccessor testResults)
            {
                return;
            }

            ApplyTestSkip(testResults.TestResult, testCase.SkipReason);
        }

        [Obsolete(
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release. You " +
            "should create model classes by yourself and use " +
            "AllureLyfecycle.Instance methods instead"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FinishTestCase(ITestCaseMessage testCaseMessage)
        {
            var testCase = testCaseMessage.TestCase;
            if (testCase is not ITestResultAccessor testResults)
            {
                return;
            }

            ReportTestCase(testResults.TestResult);
            ReportTestContainer(testResults.TestResultContainer);
        }
        #endregion
    }
}