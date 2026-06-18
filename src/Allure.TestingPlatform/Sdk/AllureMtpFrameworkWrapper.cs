using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Sdk;

public class AllureMtpFrameworkWrapper(
    ITestFramework underlyingFramework
) : ITestFramework
{
    public string Uid { get; } =
        "c980534e-8bdc-46a9-9fad-c23f3f0f3822";

    public string Version { get; } =
        ExtensionFunctions.GetCurrentPackageVersion();

    public string DisplayName { get; } =
        $"Allure wrapper for {underlyingFramework.DisplayName}";

    public string Description { get; } =
        "A test framework that establishes the session context for the Allure MTP message "
            + "API and delegates to the actual test framework.";

    public async Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context) =>
        await underlyingFramework.CreateTestSessionAsync(context);

    public async Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context) =>
        await underlyingFramework.CloseTestSessionAsync(context);

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        SessionUid.Current = context.Request.Session.SessionUid;
        await underlyingFramework.ExecuteRequestAsync(context);
    }

    public async Task<bool> IsEnabledAsync() =>
        await underlyingFramework.IsEnabledAsync();
}