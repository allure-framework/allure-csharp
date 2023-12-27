using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
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

#nullable enable

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

        internal static TestResultContainer StartNewAllureContainer(
            string className
        )
        {
            var container = CreateContainer(className);
            AllureLifecycle.Instance.StartTestContainer(container);
            return container;
        }

        internal static void StartAllureTestCase(
            ITest test,
            TestResult? testResult
        ) =>
            AllureLifecycle.Instance.StartTestCase(
                testResult ?? CreateTestResultByTest(test)
            );

        internal static void ApplyTestFailure(IFailureInformation failure)
        {
            var trace = string.Join('\n', failure.StackTraces);
            var message = string.Join('\n', failure.Messages);
            var status = failure.ExceptionTypes.Any(
                exceptionType => !exceptionType.StartsWith("Xunit.Sdk.")
            ) ? Status.broken : Status.failed;

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.trace = trace;
                statusDetails.message = message;
                testResult.status = status;
            });
        }

        internal static void ApplyTestSuccess(ITestResultMessage success)
        {
            var message = success.Output;
            var status = Status.passed;

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = message;
                testResult.status = status;
            });
        }

        internal static void ApplyTestSkip(ITestSkipped skip)
        {
            var message = skip.Reason;
            var status = Status.skipped;

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = message;
                testResult.status = status;
            });
        }

        internal static void ApplyTestParameters(
            IEnumerable<IParameterInfo> parameters,
            object[] arguments
        )
        {
            var parametersList = parameters.Zip(
                arguments,
                (param, value) => new Parameter
                {
                    name = param.Name,
                    value = FormatFunctions.Format(
                        value,
                        AllureLifecycle.Instance.TypeFormatters
                    )
                }
            ).ToList();

            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                testResult.parameters = parametersList;
            });
        }

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

        internal static TestResult CreateTestResultByTest(ITest test) =>
            CreateTestResult(test.TestCase, test.DisplayName);

        static TestResultContainer CreateContainer(string className) => new()
        {
            uuid = AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds
                ? NewUuid(className)
                : IdFunctions.CreateUUID(),
            name = className
        };

        static TestResult CreateTestResult(
            ITestCase testCase,
            string displayName
        )
        {
            var testMethod = testCase.TestMethod;
            var testResult = new TestResult
            {
                name = BuildName(testCase),
                labels = new()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Language(),
                    Label.Framework("xUnit.net"),
                    Label.TestClass(testMethod.TestClass.Class.Name),
                    Label.TestMethod(displayName),
                    Label.Package(testMethod.TestClass.Class.Name),
                }
            };
            SetTestResultIdentifiers(testCase, displayName, testResult);
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

        static void SetTestResultIdentifiers(
            ITestCase testCase,
            string displayName,
            TestResult testResult
        )
        {
            if (AllureLifecycle.Instance.AllureConfiguration.UseLegacyIds)
            {
                SetLegacyTestResultIdentifiers(testCase, displayName, testResult);
                return;
            }

            testResult.uuid = IdFunctions.CreateUUID();
            testResult.fullName = IdFunctions.CreateFullName(
                testCase.TestMethod.Method.ToRuntimeMethod()
            );
            testResult.testCaseId = IdFunctions.CreateTestCaseId(
                testResult.fullName
            );
        }

        static void SetLegacyTestResultIdentifiers(
            ITestCase testCase,
            string displayName,
            TestResult testResult
        )
        {
            testResult.uuid = NewUuid(displayName);
            testResult.fullName = BuildFullName(testCase);
            testResult.historyId = displayName;
        }

        static void UpdateTestDataFromAttributes(
            TestResult testResult,
            ITestMethod method
        )
        {
            var classAttributes = GetCustomAttributesRecursive(method.TestClass.Class, typeof(IAllureInfo));
            var methodAttributes = method.Method.GetCustomAttributes(typeof(IAllureInfo));

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

        #region Obsolete public methods
        const string OBS_MSG_UNINTENDED_PUBLIC =
            "This method wasn't supposed to be in the public API. It's not " +
            "relevant anymore and will be removed in a future release";

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
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

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void StartTestCase(ITestCaseMessage testCaseMessage)
        {
            var testCase = testCaseMessage.TestCase;
            if (testCase is not ITestResultAccessor testResults)
            {
                return;
            }

            testResults.TestResult = CreateTestResultByTestCase(testCase);
            AllureLifecycle.Instance.StartTestCase(testResults.TestResult);
            ApplyTestParameters(
                testCase.TestMethod.Method.GetParameters(),
                testCase.TestMethodArguments
            );
        }

        static TestResult CreateTestResultByTestCase(ITestCase testCase) =>
            CreateTestResult(testCase, testCase.DisplayName);

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MarkTestCaseAsFailedOrBroken(ITestFailed testFailed)
        {
            if (testFailed.TestCase is not ITestResultAccessor)
            {
                return;
            }

            ApplyTestFailure(testFailed);
        }

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MarkTestCaseAsPassed(ITestPassed testPassed)
        {
            if (testPassed.TestCase is not ITestResultAccessor)
            {
                return;
            }

            ApplyTestSuccess(testPassed);
        }

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
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

        [Obsolete(OBS_MSG_UNINTENDED_PUBLIC)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FinishTestCase(ITestCaseMessage testCaseMessage)
        {
            var testCase = testCaseMessage.TestCase;
            if (testCase is not ITestResultAccessor)
            {
                return;
            }

            ReportCurrentTestCase();
            ReportCurrentTestContainer();
        }
        #endregion
    }
}