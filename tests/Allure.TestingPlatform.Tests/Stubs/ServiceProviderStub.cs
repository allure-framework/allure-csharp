using Microsoft.Testing.Platform.CommandLine;

namespace Allure.TestingPlatform.Tests.Stubs;

public class ServiceProviderStub(
    CommandLineOptionsStub commandLineOptions
) : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        if (serviceType == typeof(ICommandLineOptions))
        {
            return commandLineOptions;
        }

        throw new NotImplementedException();
    }
}