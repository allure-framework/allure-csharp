using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.XUnit
{
    public class AllureMessageSink :
        DefaultRunnerReporterWithTypesMessageHandler
    {
        readonly ConcurrentDictionary<ITest, AllureXunitTestData>
            allureTestData = new();

        public AllureMessageSink(IRunnerLogger logger) : base(logger)
        {
            this.Runner.TestAssemblyExecutionStartingEvent +=
                this.OnTestAssemblyExecutionStarting;

            this.Execution.TestStartingEvent += this.OnTestStarting;
            this.Execution.TestClassConstructionFinishedEvent +=
                this.OnTestClassConstructionFinished;
            this.Execution.TestFailedEvent += this.OnTestFailed;
            this.Execution.TestPassedEvent += this.OnTestPassed;
            this.Execution.TestSkippedEvent += this.OnTestSkipped;
            this.Execution.TestFinishedEvent += this.OnTestFinished;

            CurrentSink ??= this;
        }

        void OnTestAssemblyExecutionStarting(
            MessageHandlerArgs<ITestAssemblyExecutionStarting> args
        )
        {
            args.Message.ExecutionOptions.SetSynchronousMessageReporting(true);
        }

        internal void OnTestArgumentsCreated(ITest test, object[] arguments) =>
            this.GetOrCreateTestData(test).Arguments = arguments;

        void OnTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            var message = args.Message;
            var test = message.Test;

            if (IsStaticTestMethod(message))
            {
                AllureXunitHelper.StartStaticAllureTestCase(test);
                this.CaptureTestContext(test);
            }
            else
            {
                AllureXunitHelper.StartNewAllureContainer(
                    message.TestClass.Class.Name
                );
            }
        }

        void OnTestClassConstructionFinished(
            MessageHandlerArgs<ITestClassConstructionFinished> args
        )
        {
            var message = args.Message;
            var test = message.Test;
            if (!IsStaticTestMethod(message))
            {
                AllureXunitHelper.StartAllureTestCase(test);
                this.CaptureTestContext(test);
            }
        }

        void OnTestFailed(MessageHandlerArgs<ITestFailed> args) =>
            this.RunInTestContext(
                args.Message.Test,
                () => AllureXunitHelper.ApplyTestFailure(args.Message)
            );

        void OnTestPassed(MessageHandlerArgs<ITestPassed> args) =>
            this.RunInTestContext(
                args.Message.Test,
                () => AllureXunitHelper.ApplyTestSuccess(args.Message)
            );

        void OnTestSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            var message = args.Message;
            var test = message.Test;
            this.UpdateTestContext(test, () =>
            {
                if (!AllureLifecycle.Instance.Context.HasTest)
                {
                    AllureXunitHelper.StartAllureTestCase(test);
                }
                AllureXunitHelper.ApplyTestSkip(message);
            });
        }

        void OnTestFinished(MessageHandlerArgs<ITestFinished> args)
        {
            var message = args.Message;
            var test = args.Message.Test;
            var arguments = this.allureTestData[test].Arguments;

            this.RunInTestContext(test, () =>
            {
                this.AddAllureParameters(test, arguments);
                AllureXunitHelper.ReportCurrentTestCase();
                if (!IsStaticTestMethod(message))
                {
                    AllureXunitHelper.ReportCurrentTestContainer();
                }
            });

            this.allureTestData.Remove(test, out _);
        }

        AllureXunitTestData GetOrCreateTestData(ITest test)
        {
            if (!this.allureTestData.TryGetValue(test, out var data))
            {
                data = new AllureXunitTestData();
                this.allureTestData[test] = data;
            }
            return data;
        }

        void AddAllureParameters(ITest test, object[] arguments)
        {
            var testCase = test.TestCase;
            var parameters = testCase.TestMethod.Method.GetParameters();
            arguments ??= testCase.TestMethodArguments
                ?? Array.Empty<object>();

            if (parameters.Any() && !arguments.Any())
            {
                this.LogUnreportedTheoryArgs(test.DisplayName);
            }
            else
            {
                AllureXunitHelper.ApplyTestParameters(parameters, arguments);
            }
        }

        void CaptureTestContext(ITest test) =>
            this.GetOrCreateTestData(test).Context =
                AllureLifecycle.Instance.Context;

        AllureContext RunInTestContext(ITest test, Action action) =>
            AllureLifecycle.Instance.RunInContext(
                this.GetOrCreateTestData(test).Context,
                action
            );

        void UpdateTestContext(ITest test, Action action) =>
            this.GetOrCreateTestData(test).Context = this.RunInTestContext(
                test,
                action
            );

        void LogUnreportedTheoryArgs(string testName)
        {
            var message = $"Unable to attach arguments of {testName} to " +
                "allure report";
#if !DEBUG
            message += ". You may try to compile the project in debug mode " +
                "as a workaround";
#endif
            this.Logger.LogWarning(message);
        }

        static bool IsStaticTestMethod(ITestMethodMessage message) =>
            message.TestMethod.Method.IsStatic;

        public static AllureMessageSink? CurrentSink { get; private set; }
    }
}