using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

using Allure.Net.Commons;
using Allure.Net.Commons.Steps;
using Allure.XUnit.Attributes;
using Allure.Xunit.Attributes;
using Allure.Xunit;

namespace Allure.XUnit
{
    public class AllureMessageSink : TestMessageSink
    {
        IRunnerLogger logger;
        Dictionary<ITest, AllureXunitTestResultAccessor> allureTestData = new();

        public AllureMessageSink(IRunnerLogger logger)
        {
            this.logger = logger;

            this.Runner.TestAssemblyExecutionStartingEvent +=
                this.OnTestAssemblyExecutionStarting;

            this.Execution.TestStartingEvent += this.OnTestStarting;
            this.Execution.TestClassConstructionFinishedEvent +=
                this.OnTestClassConstructionFinished;
            this.Execution.TestFailedEvent += this.OnTestFailed;
            this.Execution.TestPassedEvent += this.OnTestPassed;
            this.Execution.TestFinishedEvent += this.OnTestFinished;
            this.Execution.TestCaseFinishedEvent+= this.OnTestCaseFinished;
        }

        void OnTestAssemblyExecutionStarting(
            MessageHandlerArgs<ITestAssemblyExecutionStarting> args
        )
        {
            args.Message.ExecutionOptions.SetSynchronousMessageReporting(true);
        }

        internal void OnTestArgumentsCreated(ITest test, object[] arguments)
        {
            var accessor = this.GetOrCreateAllureResultAccessor(test);
            accessor.Arguments = arguments;
        }

        AllureXunitTestResultAccessor GetOrCreateAllureResultAccessor(ITest test)
        {
            if (!this.allureTestData.TryGetValue(test, out var accessor))
            {
                accessor = new AllureXunitTestResultAccessor();
                this.allureTestData[test] = accessor;
            }
            return accessor;
        }

        void OnTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            var message = args.Message;
            var test = message.Test;
            var accessor = this.GetOrCreateAllureResultAccessor(test);

            CoreStepsHelper.TestResultAccessor = accessor;

            if (message.TestMethod.Method.IsStatic)
            {
                accessor.TestResult = this.StartStaticAllureTestCase(test);
            }
            else
            {
                accessor.TestResultContainer = this.StartNewAllureContainer(
                    test,
                    message.TestClass.Class.Name
                );
            }
        }

        void OnTestClassConstructionFinished(
            MessageHandlerArgs<ITestClassConstructionFinished> args
        )
        {
            var test = args.Message.Test;
            var accessor = this.allureTestData[test];
            var container = accessor.TestResultContainer;
            if (accessor.TestResult is null && container is not null)
            {
                accessor.TestResult = this.StartAllureTestCase(test, container);
            }
        }

        void OnTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            var message = args.Message;
            var test = message.Test;
            var testResult = this.allureTestData[test].TestResult;

            if (testResult is not null)
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.trace = string.Join('\n', message.StackTraces);
                statusDetails.message = string.Join('\n', message.Messages);

                testResult.status = message.ExceptionTypes.Any(
                    exceptionType => !exceptionType.StartsWith("Xunit.Sdk.")
                ) ? Status.broken : Status.failed;
            }
        }

        void OnTestPassed(MessageHandlerArgs<ITestPassed> args)
        {
            var message = args.Message;
            var test = message.Test;
            var testResult = this.allureTestData[test].TestResult;

            if (testResult is not null)
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = message.Output;
                testResult.status = Status.passed;
            }
        }

        void OnTestFinished(MessageHandlerArgs<ITestFinished> args)
        {
            var test = args.Message.Test;
            var accessor = this.allureTestData[test];
            this.allureTestData.Remove(test);
            var testResult = accessor.TestResult;
            if (testResult is not null)
            {
                this.AddAllureParameters(testResult, test, accessor.Arguments);
                AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                AllureLifecycle.Instance.WriteTestCase(testResult.uuid);

                var container = accessor.TestResultContainer;
                if (container is not null)
                {
                    AllureLifecycle.Instance.StopTestContainer(container.uuid);
                    AllureLifecycle.Instance.WriteTestContainer(container.uuid);
                }
            }


        }

        void OnTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            var testCase = args.Message.TestCase;
            if (testCase.SkipReason != null)
            {
                var testResult = this.CreateTestResultByTestCase(testCase);
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = testCase.SkipReason;
                testResult.status = Status.skipped;
                AllureLifecycle.Instance.StartTestCase(testResult);
                AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
            }
        }

        TestResultContainer StartNewAllureContainer(
            ITest test,
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

        TestResult StartAllureTestCase(
            ITest test,
            TestResultContainer container
        )
        {
            var testResult = this.CreateTestResultByTest(test);
            AllureLifecycle.Instance.StartTestCase(container.uuid, testResult);
            return testResult;
        }

        TestResult StartStaticAllureTestCase(ITest test)
        {
            var testResult = this.CreateTestResultByTest(test);
            AllureLifecycle.Instance.StartTestCase(testResult);
            return testResult;
        }

        TestResult CreateTestResultByTest(ITest test) =>
            this.CreateTestResult(test.TestCase, test.DisplayName);

        TestResult CreateTestResultByTestCase(ITestCase testCase) =>
            this.CreateTestResult(testCase, testCase.DisplayName);

        TestResult CreateTestResult(ITestCase testCase, string displayName)
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

        void AddAllureParameters(
            TestResult testResult,
            ITest test,
            object[] arguments
        )
        {
            var testCase = test.TestCase;
            var parameters = testCase.TestMethod.Method.GetParameters();
            arguments ??= testCase.TestMethodArguments ?? Array.Empty<object>();

            if (parameters.Any() && !arguments.Any())
            {
                this.LogUnreportedTheoryArgs(test.DisplayName);
            }
            else
            {
                testResult.parameters = parameters.Zip(
                    arguments,
                    (param, value) => new Parameter
                    {
                        name = param.Name,
                        value = value?.ToString() ?? "null"
                    }
                ).ToList();
            }
        }

        void LogUnreportedTheoryArgs(string testName)
        {
            var message = $"Unable to attach arguments of {testName} to " +
                "allure report";
#if !DEBUG
            message += ". You may try to compile the project in debug mode " +
                "as a workaround";
#endif
            this.logger.LogWarning(message);
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
                switch (((IReflectionAttributeInfo) attribute).Attribute)
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
                        testResult.labels.AddDistinct(label, labelAttribute.Overwrite);
                        break;
                }
            }
        }
    }
}