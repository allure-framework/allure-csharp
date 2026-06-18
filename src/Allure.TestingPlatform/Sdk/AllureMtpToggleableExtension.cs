using System.Threading.Tasks;

namespace Allure.TestingPlatform.Sdk;

public abstract class AllureMtpToggleableExtension(
    string uid,
    string displayName,
    string description,
    IAllureInfrastructure allure
) : AllureMtpExtensionBase(uid, displayName, description)
{
    public IAllureInfrastructure Allure => allure;

    public override Task<bool> IsEnabledAsync() => Task.FromResult(allure.IsEnabled);
}