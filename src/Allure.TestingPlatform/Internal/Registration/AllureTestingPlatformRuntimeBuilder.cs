using System;
using System.Collections.Immutable;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeBuilder(
    AllureTestingPlatformRegistrationInput input,
    IServiceProvider serviceProvider,
    AllureTestingPlatformRuntimeProvider runtimeProvider
) :
    IAllureTestingPlatformRuntimeOwner
{
    IAllureTestingPlatformRuntimeProvider IAllureTestingPlatformRuntimeOwner.RuntimeProvider => runtimeProvider;

    public AllureTestingPlatformRuntime Configure()
    {
        _ = runtimeProvider.Value switch
        {
            SuppressedAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot configure Allure.TestingPlatform: "
                        + "Allure.TestingPlatform is suppressed via the CLI"
                ),

            { State: not AllureTestingPlatformRuntimeState.NotInitialized } =>
                throw new InvalidOperationException(
                    "Cannot configure Allure.TestingPlatform: "
                        + "Allure.TestingPlatform is already configured"
                ),

            var value => value,
        };

        var cliOptions = serviceProvider.GetCommandLineOptions();
        var allureToggle = TestingPlatformFunctions.GetAllureToggleValue(cliOptions);
        if (allureToggle == false)
        {
            var suppressedState = new SuppressedAllureTestingPlatformRuntime(input.Mode);
            runtimeProvider.Value = suppressedState;
            return suppressedState;
        }

        var configuration = input.ConfigurationFactory(serviceProvider);
        var logger = input.LoggerFactory(serviceProvider, configuration);

        ConfiguredAllureTestingPlatformRuntime configuredState =
            allureToggle is null && !input.IsSdkEnabled(serviceProvider, configuration)
                ? new DisabledAllureTestingPlatformRuntime(input.Mode, logger, configuration)
                : new EnabledAllureTestingPlatformRuntime(input.Mode, logger, configuration);

        runtimeProvider.Value = configuredState;
        input.OnConfigured?.Invoke(configuredState);

        return configuredState;
    }

    public AllureTestingPlatformRuntime Build()
    {
        var configuredState = runtimeProvider.Value switch
        {
            EnabledAllureTestingPlatformRuntime catp => catp,

            SuppressedAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot create Allure.TestingPlatform runtime: "
                        + "Allure.TestingPlatform is suppressed via the CLI"
                ),

            DisabledAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot create Allure.TestingPlatform runtime: "
                        + "Allure.TestingPlatform is disabled"
                ),

            ReadyAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot create Allure.TestingPlatform runtime: "
                        + "the runtime was already created"
                ),

            _ => throw new InvalidOperationException(
                "Cannot create Allure.TestingPlatform runtime: "
                    + "Allure.TestingPlatform must be configured first"
            ),
        };

        var configuration = configuredState.Configuration;
        var logger = configuredState.Logger;

        var writer = input.WriterFactory(serviceProvider, configuration);
        var typeFormatters = input.TypeFormattersFactory(serviceProvider, configuration)
            .ToImmutableDictionary();

        var readyState = new ReadyAllureTestingPlatformRuntime(
            Mode: input.Mode,
            Logger: logger,
            Configuration: configuration,
            CorrelationStrategy: input.CorrelationStrategyFactory(serviceProvider, configuration),
            Writer: writer,
            TypeFormatters: typeFormatters,
            Lifecycle: input.LifecycleFactory(
                serviceProvider,
                new(
                    Config: configuration,
                    Writer: writer,
                    TypeFormatters: typeFormatters
                )
            )
        );
        runtimeProvider.Value = readyState;
        input.OnReady?.Invoke(readyState);
        return readyState;
    }
}
