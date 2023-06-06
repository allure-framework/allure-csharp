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
        Dictionary<ITest, TestResultContainer> testContainers = new();
        Dictionary<ITest, TestResult> testResults = new();
        Dictionary<ITest, object[]> testArguments = new();

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
            testArguments.Add(test, arguments);
        }

        void OnTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            var message = args.Message;
            var test = message.Test;
            Steps.CurrentTest = test;
            if (message.TestMethod.Method.IsStatic)
            {
                this.StartStaticAllureTestCase(test);
            }
            else
            {
                this.StartAllureContainer(test, message.TestClass.Class.Name);
            }
        }

        void OnTestClassConstructionFinished(
            MessageHandlerArgs<ITestClassConstructionFinished> args
        )
        {
            var message = args.Message;
            var test = message.Test;
            if (!testResults.ContainsKey(test)
                && testContainers.TryGetValue(test, out var container))
            {
                this.StartAllureTestCase(test, container);
            }
        }

        void OnTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            var message = args.Message;
            var test = message.Test;
            if (testResults.TryGetValue(test, out var testResult))
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
            if (testResults.TryGetValue(test, out var testResult))
            {
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = message.Output;
                testResult.status = Status.passed;
            }
        }

        void OnTestFinished(MessageHandlerArgs<ITestFinished> args)
        {
            var test = args.Message.Test;
            if (testResults.TryGetValue(test, out var testResult))
            {
                this.AddAllureParameters(testResult, test);
                AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
                testResults.Remove(test);
            }

            if (testContainers.TryGetValue(test, out var container))
            {
                AllureLifecycle.Instance.StopTestContainer(container.uuid);
                AllureLifecycle.Instance.WriteTestContainer(container.uuid);
                testContainers.Remove(test);
            }

            if (testArguments.ContainsKey(test))
            {
                testArguments.Remove(test);
            }
        }

        void OnTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            var testCase = args.Message.TestCase;
            if (testCase.SkipReason != null)
            {
                var testResult = this.CreateTestResult(testCase, testCase.DisplayName);
                var statusDetails = testResult.statusDetails ??= new();
                statusDetails.message = testCase.SkipReason;
                testResult.status = Status.skipped;
                AllureLifecycle.Instance.StartTestCase(testResult);
                AllureLifecycle.Instance.StopTestCase(testResult.uuid);
                AllureLifecycle.Instance.WriteTestCase(testResult.uuid);
            }
        }

        TestResultContainer StartAllureContainer(ITest test, string className)
        {
            var container = new TestResultContainer
            {
                uuid = NewUuid(className),
                name = className
            };
            this.testContainers.Add(test, container);
            AllureLifecycle.Instance.StartTestContainer(container);
            return container;
        }

        void StartAllureTestCase(
            ITest test,
            TestResultContainer container
        ) => AllureLifecycle.Instance.StartTestCase(
            container.uuid,
            this.AddNewTestResult(test)
        );

        void StartStaticAllureTestCase(ITest test) =>
            AllureLifecycle.Instance.StartTestCase(
                this.AddNewTestResult(test)
            );

        TestResult AddNewTestResult(ITest test)
        {
            var result = this.CreateTestResult(test.TestCase, test.DisplayName);
            testResults.Add(test, result);
            return result;
        }

        TestResult CreateTestResult(ITestCase testCase, string displayName)
        {
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
                    Label.TestClass(testCase.TestMethod.TestClass.Class.Name),
                    Label.TestMethod(displayName),
                    Label.Package(testCase.TestMethod.TestClass.Class.Name)
                }
            };
            UpdateTestDataFromAttributes(testResult, testCase);
            return testResult;
        }

        void AddAllureParameters(TestResult testResult, ITest test)
        {
            var testCase = test.TestCase;
            var parameters = testCase.TestMethod.Method.GetParameters();
            var arguments = ResolveTestArguments(test);
            testResult.parameters = parameters.Zip(
                arguments,
                (param, value) => new Parameter
                {
                    name = param.Name,
                    value = value?.ToString() ?? "null"
                }
            ).ToList();
        }

        object[] ResolveTestArguments(ITest test)
        {
            object[] arguments = null;
            if (!testArguments.TryGetValue(test, out arguments))
            {
                arguments = test.TestCase.TestMethodArguments;
            }
            return arguments ?? Array.Empty<object>();
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

        internal TestResultContainer GetTestResultContainer(ITest test)
        {
            return this.testContainers[test];
        }

        internal TestResult GetTestResult(ITest test)
        {
            return this.testResults[test];
        }
    }
}