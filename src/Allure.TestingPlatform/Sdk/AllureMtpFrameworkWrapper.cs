using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Sdk;

public class AllureMtpFrameworkWrapper(
    ITestFramework underlyingFramework
) :
    AllureMtpExtensionBase(
        "c980534e-8bdc-46a9-9fad-c23f3f0f3822",
        $"Allure wrapper for {underlyingFramework.DisplayName}",
        "A test framework that establishes the session context for Allure MTP message "
            + "producers and delegates to the actual test framework."
    ),
    ITestFramework
{
    public async Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context) =>
        await underlyingFramework.CreateTestSessionAsync(context);

    public async Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context) =>
        await underlyingFramework.CloseTestSessionAsync(context);

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        SessionUid.Current = context.Request.Session.SessionUid;
        await underlyingFramework.ExecuteRequestAsync(context);
    }

    public override async Task<bool> IsEnabledAsync() =>
        await underlyingFramework.IsEnabledAsync();
}