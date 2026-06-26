using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntimeConfigurator : IAllureRegistrationContext
{
    IAllureRuntimeBuilder Configure();
}
