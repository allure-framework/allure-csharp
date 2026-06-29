using System;
using System.Collections.Immutable;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformBuilder(
    AllureTestingPlatformRegistrationInput input,
    IServiceProvider serviceProvider,
    AllureTestingPlatformServiceProvider allureStateProvider
) :
    IAllureTestingPlatformBuilder
{
    IAllureTestingPlatformServiceProvider IAllureTestingPlatformBuilder.StateProvider => allureStateProvider;

    public AllureTestingPlatform Configure()
    {
        _ = allureStateProvider.Value switch
        {
            SuppressedAllureTestingPlatform =>
                throw new InvalidOperationException(
                    "Cannot configure Allure.TestingPlatform: "
                        + "Allure.TestingPlatform is suppressed via the CLI"
                ),

            { State: not AllureTestingPlatformState.NotInitialized } =>
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
            var suppressedState = new SuppressedAllureTestingPlatform(input.Mode);
            allureStateProvider.Value = suppressedState;
            return suppressedState;
        }

        var configuration = input.ConfigurationFactory(serviceProvider);
        var logger = input.LoggerFactory(serviceProvider, configuration);

        ConfiguredAllureTestingPlatform configuredState =
            allureToggle is null && !input.IsSdkEnabled(serviceProvider, configuration)
                ? new DisabledAllureTestingPlatform(input.Mode, logger, configuration)
                : new EnabledAllureTestingPlatform(input.Mode, logger, configuration);

        allureStateProvider.Value = configuredState;
        input.OnConfigured?.Invoke(configuredState);

        return configuredState;
    }

    public AllureTestingPlatform Build()
    {
        var configuredState = allureStateProvider.Value switch
        {
            EnabledAllureTestingPlatform catp => catp,

            SuppressedAllureTestingPlatform =>
                throw new InvalidOperationException(
                    "Cannot create Allure.TestingPlatform runtime: "
                        + "Allure.TestingPlatform is suppressed via the CLI"
                ),

            DisabledAllureTestingPlatform =>
                throw new InvalidOperationException(
                    "Cannot create Allure.TestingPlatform runtime: "
                        + "Allure.TestingPlatform is disabled"
                ),

            ReadyAllureTestingPlatform =>
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

        var readyState = new ReadyAllureTestingPlatform(
            Mode: input.Mode,
            Logger: logger,
            Configuration: configuration,
            CorrelationSource: input.CorrelationServiceFactory(serviceProvider, configuration),
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
        allureStateProvider.Value = readyState;
        input.OnReady?.Invoke(readyState);
        return readyState;
    }
}
