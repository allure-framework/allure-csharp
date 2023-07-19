using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;
using Allure.Net.Commons.Storage;
using Allure.Xunit;
using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.XUnit
{
    public class AllureMessageSink : TestMessageSink
    {
        readonly IRunnerLogger logger;
        readonly ConcurrentDictionary<ITest, AllureXunitTestData> allureTestData
            = new();

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
            this.Execution.TestSkippedEvent += this.OnTestSkipped;
            this.Execution.TestFinishedEvent += this.OnTestFinished;
        }

        public override bool OnMessageWithTypes(
            IMessageSinkMessage message,
            HashSet<string> messageTypes
        )
        {
            try
            {
                this.logger.LogMessage(message.GetType().Name);
                return base.OnMessageWithTypes(message, messageTypes);
            }
            catch (Exception e)
            {
                if (message is ITestCaseMessage testCaseMessage)
                {
                    this.logger.LogError(
                        "Error during execution of {0}: {1}",
                        testCaseMessage.TestCase.DisplayName,
                        e
                    );
                }
                else
                {
                    this.logger.LogError(e.ToString());
                }
                return false;
            }
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
                _ => AllureXunitHelper.ApplyTestFailure(args.Message)
            );

        void OnTestPassed(MessageHandlerArgs<ITestPassed> args) =>
            this.RunInTestContext(
                args.Message.Test,
                _ => AllureXunitHelper.ApplyTestSuccess(args.Message)
            );

        void OnTestSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            var message = args.Message;
            var test = message.Test;
            this.UpdateTestContext(test, ctx =>
            {
                if (!ctx.HasTest)
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

            this.RunInTestContext(test, _ =>
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

        void RunInTestContext(ITest test, Action<AllureContext> action) =>
            AllureLifecycle.Instance.RunInContext(
                this.GetOrCreateTestData(test).Context,
                action
            );

        void UpdateTestContext(ITest test, Action<AllureContext> action) =>
            this.RunInTestContext(
                test,
                ctx =>
                {
                    action(ctx);
                    this.CaptureTestContext(test);
                }
            );

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

        static bool IsStaticTestMethod(ITestMethodMessage message) =>
            message.TestMethod.Method.IsStatic;
    }
}