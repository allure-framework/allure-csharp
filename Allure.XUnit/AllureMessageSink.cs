using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit;

namespace Allure.XUnit
{
    public class AllureMessageSink : TestMessageSink
    {
        IRunnerLogger logger;
        Dictionary<ITestCase, TestResultContainer> testContainers = new();
        Dictionary<ITestCase, TestResult> testResults = new();

        public AllureMessageSink(IRunnerLogger logger)
        {
            this.logger = logger;

            this.Runner.TestAssemblyExecutionStartingEvent +=
                this.OnTestAssemblyExecutionStarting;

            this.Execution.TestCaseStartingEvent += this.OnTestCaseStarting;
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
            AllureMessageSink.CurrentSink = this;
        }

        void OnTestCaseStarting(MessageHandlerArgs<ITestCaseStarting> args)
        {
            var testClass = args.Message.TestClass;
            var testCase = args.Message.TestCase;
            var container = new TestResultContainer
            {
                uuid = NewUuid(testClass.Class.Name),
                name = testClass.Class.Name
            };
            this.testContainers.Add(testCase, container);
            Steps.CurrentTestCase = testCase;
            AllureLifecycle.Instance.StartTestContainer(container);
        }

        void OnTestClassConstructionFinished(
            MessageHandlerArgs<ITestClassConstructionFinished> args
        ) => StartTestCase(args.Message.TestCase);

        void OnTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            var message = args.Message;
            var testCase = message.TestCase;
            if (testContainers.TryGetValue(testCase, out var container)
                && testResults.TryGetValue(testCase, out var testResult))
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
            var testCase = message.TestCase;
            if (testContainers.TryGetValue(testCase, out var container)
                && testResults.TryGetValue(testCase, out var testResult))
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = message.Output;
                testResult.status = Status.passed;
            }
        }

        void OnTestFinished(MessageHandlerArgs<ITestFinished> args)
        {
            var testCase = args.Message.TestCase;
            if (testContainers.TryGetValue(testCase, out var container)
                && testResults.TryGetValue(testCase, out var testResult))
            {
                AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
                testResults.Remove(testCase);
            }
        }

        void OnTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            TestResultContainer container = null;
            TestResult testResult = null;
            var testCase = args.Message.TestCase;
            if (testCase.SkipReason != null)
            {
                StartTestCase(testCase);
                if (testContainers.TryGetValue(testCase, out container)
                    && testResults.TryGetValue(testCase, out testResult))
                {
                    var statusDetails = testResult.statusDetails ??= new();
                    statusDetails.message = testCase.SkipReason;
                    testResult.status = Status.skipped;
                }
            }
            if (testContainers.TryGetValue(testCase, out container)
                && testResults.TryGetValue(testCase, out testResult))
            {
                //AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                //AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
                AllureLifecycle.Instance.StopTestContainer(container.uuid);
                AllureLifecycle.Instance.WriteTestContainer(container.uuid);
            }
        }

        void StartTestCase(ITestCase testCase)
        {
            if (testContainers.TryGetValue(testCase, out var container))
            {
                var testResult = new TestResult
                {
                    uuid = NewUuid(testCase.DisplayName),
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
                    parameters = testCase.TestMethod.Method.GetParameters().Zip(
                        testCase.TestMethodArguments ?? Array.Empty<object>(),
                        (param, value) => new Parameter
                        {
                            name = param.Name,
                            value = value?.ToString() ?? "null"
                        }
                    ).ToList()
                };
                testResults.Add(testCase, testResult);
                UpdateTestDataFromAttributes(testResult, testCase);
                AllureLifecycle.Instance.StartTestCase(container.uuid, testResult);
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

        static void UpdateTestDataFromAttributes(
            TestResult testResult,
            ITestCase testCase
        )
        {
            var method = testCase.TestMethod;
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

        internal TestResultContainer GetTestResultContainer(ITestCase testCase)
        {
            return this.testContainers[testCase];
        }

        internal TestResult GetTestResult(ITestCase testCase)
        {
            return this.testResults[testCase];
        }

        internal static AllureMessageSink CurrentSink { get; private set; }
    }
}