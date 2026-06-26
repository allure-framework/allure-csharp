using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Implementation;

public class AllureRuntimeBuildResult(
    AllureConfiguration configuration,
    ILogger logger,
    IAllureExtensionSettings settings,
    Func<AllureConfiguration, ICorrelationService> correlationServiceFactory,
    Func<AllureConfiguration, IAllureResultsWriter> writerFactory,
    Func<AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory,
    Func<AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
) :
    IAllureRuntimeBuildResult
{
    public AllureConfiguration Configuration => configuration;

    public ILogger Logger => logger;

    public IAllureExtensionSettings ExtensionSettings => settings;

    public IAllureRuntime CreateRuntime()
    {
        var writer = writerFactory(this.Configuration);
        var typeFormatters = typeFormattersFactory(this.Configuration)
            .ToImmutableDictionary();

        return new AllureRuntime(
            configuration: this.Configuration,
            logger: this.Logger,
            correlationService: correlationServiceFactory(this.Configuration),
            writer: writer,
            typeFormatters: typeFormatters,
            lifecycle: lifecycleFactory(
                new(
                    Config: this.Configuration,
                    Writer: writer,
                    TypeFormatters: typeFormatters
                )
            )
        );
    }
}
