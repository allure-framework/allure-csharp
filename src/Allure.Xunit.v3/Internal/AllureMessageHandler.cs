using System;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Xunit.Runner.Common;
using Xunit.Sdk;
using System.Reflection;
using Allure.Xunit.Functions;

using IMtpMessageBus = Microsoft.Testing.Platform.Messages.IMessageBus;
using Allure.TestingPlatform.Functions;

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
        "A message handler that translates Xunit runner reporter messages for Allure.";

    public string Version { get; } = TestingPlatformFunctions.GetPackageVersion(typeof(AllureMessageHandler));

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    protected override void HandleTestCaseStarting(MessageHandlerArgs<ITestCaseStarting> args)
    {
        base.HandleTestCaseStarting(args);

        if (CommunicationFunctions.TryConvertToAllureScopeStartMessage(args.Message, out var allureScopeStart))
        {
            this.PublishSync(allureScopeStart);
        }
    }

    protected override void HandleTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
    {
        base.HandleTestCaseFinished(args);

        if (CommunicationFunctions.TryConvertToAllureScopeStopMessage(
            args.Message,
            this.MetadataCache,
            out var allureScopeStop))
        {
            this.PublishSync(allureScopeStop);
        }
    }

    internal void HandleBeforeTest(MethodInfo testMethod, string testCaseUniqueID, object?[]? arguments)
    {
        if (CommunicationFunctions.TryConvertToTestUpdateWithMethod(
            testMethod,
            testCaseUniqueID,
            arguments,
            this.MetadataCache,
            out var allureTestUpdate))
        {
            this.PublishSync(allureTestUpdate);
        }

        TestPlanFunctions.ApplyRuntimeGuard(testMethod);
    }

    void PublishSync(AllureCorrelatedMessage message) =>
        mtpMessageBus
            .PublishAsync(this, message)
            .SpinWait();
}
