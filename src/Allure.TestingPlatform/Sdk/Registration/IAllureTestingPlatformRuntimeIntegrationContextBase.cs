using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeIntegrationContextBase<TConfiguration, out TRuntime> :
    IAllureRuntimeIntegrationContextBase<
        TConfiguration,
        TRuntime
    >,
    IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    void UseLogger(Func<TConfiguration, ILogger> loggerFactory);

    void UseCorrelationStrategy(Func<TConfiguration, ICorrelationStrategy> correlationStrategyFactory);

    void UseCorrelationContext(Func<TConfiguration, ICorrelationContext> correlationContextFactory);

    void UseExecutionStateContext(Func<TConfiguration, ExecutionStateContext> executionStateContextFactory);
}
