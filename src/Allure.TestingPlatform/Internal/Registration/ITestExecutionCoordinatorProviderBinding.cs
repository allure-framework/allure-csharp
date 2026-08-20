namespace Allure.TestingPlatform.Internal.Registration;

interface ITestExecutionCoordinatorProviderBinding<TConfiguration>
{
    void BindTestExecutionCoordinatorProvider(
        TestExecutionCoordinatorProvider<TConfiguration> provider
    );
}
