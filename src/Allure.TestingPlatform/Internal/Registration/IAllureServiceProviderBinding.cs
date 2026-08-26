namespace Allure.TestingPlatform.Internal.Registration;

interface IAllureServiceProviderBinding<TConfiguration>
{
    void BindServiceProvider(
        InternalServiceProvider<TConfiguration> provider
    );
}
