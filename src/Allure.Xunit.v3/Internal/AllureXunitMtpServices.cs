using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Services;

namespace Allure.Xunit.Internal;

class AllureXunitMtpServices(
    IServiceProvider serviceProvider,
    IAllureTestingPlatformRuntimeReference runtimeReference
) :
    AllureTestingPlatformExtension(
        "725e7e6e-74de-4e77-b0bf-eaebfe19914e",
        "MTP services exposed to Allure.Xunit.v3",
        "The extension represents a static access point for "
            + "Allure.TestingPlatform and Microsoft Testing Platform "
            + "services required by Allure.Xunit.v3",
            runtimeReference
    ),
    ITestHostApplicationLifetime
{
    record class ExposedAllureMtpServices(
        LiveAllureTestingPlatformRuntime AllureRuntime,
        Lazy<IMessageBus> MessageBus
    );

    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        allureTestingPlatform = new(
            this.LiveRuntime,
            new(serviceProvider.GetMessageBus)
        );
        return Task.CompletedTask;
    }

    public Task AfterRunAsync(int exitCode, CancellationToken cancellationToken)
    {
        allureTestingPlatform = null;
        return Task.CompletedTask;
    }

    static ExposedAllureMtpServices? allureTestingPlatform;

    static ExposedAllureMtpServices Services => allureTestingPlatform
        ?? throw new InvalidOperationException(
            "Internal error: Allure.Xunit.v3 is not initialized."
        );

    public static bool IsAllureAlive => allureTestingPlatform is not null;

    public static IMessageBus MessageBus => Services.MessageBus.Value;

    public static LiveAllureTestingPlatformRuntime AllureRuntime => Services.AllureRuntime;

}
