using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime;

public interface IAllureTestingPlatformRuntimeHandle<out TConfiguration, out TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference { get; }

    IReadOnlyLateBoundReference<TRuntime> RuntimeReference { get; }

    bool CanPublish { get; }

    Task PublishAsync(IDataProducer dataProducer, IData data);
}
