using System;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons.TestPlan;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Xunit.Functions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Xunit;
using Xunit.Runner.Common;
using Xunit.Sdk;
using IMtpMessageBus = Microsoft.Testing.Platform.Messages.IMessageBus;

namespace Allure.Xunit.Internal;

sealed class AllureMessageHandler(
    IRunnerLogger logger,
    IMtpMessageBus mtpMessageBus
) :
    DefaultRunnerReporterMessageHandler(logger),
    IRunnerReporterMessageHandler,
    IDataProducer
{
    public Type[] DataTypesProduced => [
        typeof(AllureScopeStartMessage),
        typeof(AllureScopeStopMessage),
        typeof(AllureTestUpdateMessage),
    ];

    public string Uid { get; } = "30b8fdc2-68a8-49be-b0f8-b6bed05d07bd";

    public string DisplayName { get; } =
        "Allure.Xunit.v3 runner message handler";

    public string Description { get; } =
        "A message handler that translates xUnit runner reporter messages for Allure.";

    public string Version { get; } = TestingPlatformFunctions.GetPackageVersion(typeof(AllureMessageHandler));

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    protected override void HandleTestCaseStarting(MessageHandlerArgs<ITestCaseStarting> args)
    {
        base.HandleTestCaseStarting(args);

        if (XunitMessageMapping.TryConvertToAllureScopeStartMessage(args.Message, out var allureScopeStart))
        {
            this.PublishSync(allureScopeStart);
        }
    }

    internal void HandleBeforeTest(MethodInfo testMethod, ITest test, object?[]? arguments)
    {
        this.ApplyRuntimeGuard(testMethod, test);

        if (XunitMessageMapping.TryConvertToTestUpdateWithMethod(
            testMethod,
            test,
            arguments,
            out var allureTestUpdate
        ))
        {
            this.PublishSync(allureTestUpdate);
        }
    }

    protected override void HandleTestFailed(MessageHandlerArgs<ITestFailed> args)
    {
        base.HandleTestFailed(args);

        if (XunitMessageMapping.TryConvertToTestUpdateWithFailedStatus(
            args.Message,
            this.MetadataCache,
            out var allureTestUpdate
        ))
        {
            this.PublishSync(allureTestUpdate);
        }
    }

    protected override void HandleTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
    {
        base.HandleTestCaseFinished(args);

        if (XunitMessageMapping.TryConvertToAllureScopeStopMessage(
            args.Message,
            this.MetadataCache,
            out var allureScopeStop
        ))
        {
            this.PublishSync(allureScopeStop);
        }
    }

    void ApplyRuntimeGuard(MethodInfo testMethod, ITest test)
    {
        var isSelected = TestPlanFunctions.IsSelected(testMethod);
        if (isSelected)
        {
            return;
        }

        if (XunitMessageMapping.TryConvertToCancellation(test, out var cancellation))
        {
            this.PublishSync(cancellation);
        }

        Assert.Skip(AllureTestPlan.SkipReason);
    }

    void PublishSync(AllureCorrelatedMessage message) =>
        mtpMessageBus
            .PublishAsync(this, message)
            .SpinWait();
}
