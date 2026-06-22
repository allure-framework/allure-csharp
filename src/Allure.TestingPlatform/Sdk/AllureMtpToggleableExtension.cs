using System.Threading.Tasks;

namespace Allure.TestingPlatform.Sdk;

public abstract class AllureMtpToggleableExtension(
    string uid,
    string displayName,
    string description,
    IAllureRuntime allure
) : AllureMtpExtensionBase(uid, displayName, description)
{
    public IAllureRuntime Allure => allure;

    public override Task<bool> IsEnabledAsync() => Task.FromResult(allure.IsEnabled);
}