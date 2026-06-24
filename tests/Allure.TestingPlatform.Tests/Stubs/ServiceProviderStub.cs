using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.CommandLine;

namespace Allure.TestingPlatform.Tests.Stubs;

public class ServiceProviderStub(
    CommandLineOptionsStub commandLineOptions,
    AllureRuntimeProviderStub allureRuntimeProviderStub
) : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        if (serviceType == typeof(ICommandLineOptions))
        {
            return commandLineOptions;
        }

        if (serviceType == typeof(IAllureRuntimeProvider))
        {
            return allureRuntimeProviderStub;
        }

        throw new NotImplementedException();
    }
}