namespace Allure.TestingPlatform.Internal.Registration;

interface IAllureTestingPlatformRequestCoordinator
{
    void ActivateRequest(TestingPlatformRequestBinding binding);

    public void ReleaseRequest(ITestingPlatformRequestBinding binding);

    public void DisposeRequestBinding(ITestingPlatformRequestBinding binding);
}
