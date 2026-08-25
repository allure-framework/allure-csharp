using Allure.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.Xunit.Configuration;
using Allure.Xunit.Runtime;

namespace Allure.Xunit.Internal.Registration;

using IAllureXunitRegistration =
    IAllureTestingPlatformRegistration<AllureXunitConfiguration, AllureXunitRuntime>;

static class AllureXunitRegistration
{
    static readonly LateBoundReference<IAllureXunitRegistration> registrationReference = new();

    public static bool IsRegistered => registrationReference.IsBound;

    public static bool IsEnabled =>
        IsRegistered
            && Current.ConfigurationReference is { IsBound: true } configurationReference
            && configurationReference.Value.IsEnabled;

    public static bool IsAvailable =>
        IsEnabled && AllureRunnerReporter.MessageHandlerReference.IsBound;

    public static IAllureXunitRegistration Current => registrationReference.Value;

    internal static void Bind(IAllureXunitRegistration registration)
    {
        registrationReference.Bind(registration);
    }
}
