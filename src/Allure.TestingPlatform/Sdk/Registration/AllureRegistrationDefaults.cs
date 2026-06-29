using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Sdk.Registration;

public static class AllureRegistrationDefaults
{
    public static ILogger GetTestingPlatformLogger(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        serviceProvider
            .GetLoggerFactory()
            .CreateLogger("Allure.TestingPlatform");

    public static AllureConfiguration ReadAllureConfiguration(IServiceProvider serviceProvider) =>
        ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(serviceProvider);

    public static bool AlwaysEnabled(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        true;

    public static ICorrelationStrategy CorrelateBySessionUidOnly(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        new TestingPlatformSessionUidCorrelationStrategy();

    public static IAllureResultsWriter GetFileSystemResultsWriter(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        new FileSystemResultsWriter(
            outputDirectory: configuration.Directory,
            indentOutput: configuration.IndentOutput
        );

    public static IReadOnlyDictionary<Type, ITypeFormatter> NoTypeFormatters(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) => ImmutableDictionary<Type, ITypeFormatter>.Empty;

    public static AllureLifecycle CreateLifecycle(
        IServiceProvider serviceProvider,
        AllureLifecycleFactoryContext lifecycleFactoryContext
    ) =>
        new(
            lifecycleFactoryContext.Config,
            lifecycleFactoryContext.Writer,
            new(lifecycleFactoryContext.TypeFormatters)
        );

    public static Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> ExplicitTypeFormatters(
        params IEnumerable<ITypeFormatter> formatters
    ) =>
        (_, _) => formatters
            .Select((formatter) =>
                formatter.GetType() is { IsGenericType: true } type && type.GetGenericTypeDefinition() == typeof(TypeFormatter<>)
                    ? (type.GetGenericArguments()[0], formatter)
                    : ((Type TargetType, ITypeFormatter Formatter)?)null
            )
            .Where((p) => p.HasValue)
            .ToImmutableDictionary(
                (p) => p!.Value.TargetType,
                (p) => p!.Value.Formatter
            );
}
