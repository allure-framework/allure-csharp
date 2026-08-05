using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal sealed record RuntimeServiceCreationContext<TConfiguration>(
    TConfiguration Configuration,
    IReadOnlyLateBoundReference<IAllureRuntime<TConfiguration>>
        RuntimeReference
)
    where TConfiguration : AllureConfiguration;
