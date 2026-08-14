using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Runtime;

public interface IAllureTestingPlatformRuntimeHandle<out TConfiguration, out TRuntime> :
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference { get; }

    IReadOnlyLateBoundReference<TRuntime> RuntimeReference { get; }

    Task PublishAsync(IDataProducer dataProducer, IData data);
}
