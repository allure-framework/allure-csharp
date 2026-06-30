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

/// <summary>
/// Provides default factories used by Allure.TestingPlatform registration.
/// </summary>
public static class AllureRegistrationDefaults
{
    /// <summary>
    /// Creates the default Microsoft Testing Platform logger.
    /// </summary>
    public static ILogger GetTestingPlatformLogger(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        serviceProvider
            .GetLoggerFactory()
            .CreateLogger("Allure");

    /// <summary>
    /// Reads the default Allure configuration.
    /// </summary>
    public static AllureConfiguration ReadAllureConfiguration(IServiceProvider serviceProvider) =>
        ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(serviceProvider);

    /// <summary>
    /// Returns <see langword="true"/> for every registration.
    /// </summary>
    public static bool AlwaysEnabled(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        true;

    /// <summary>
    /// Uses Microsoft.Testing.Platform session UIDs for message correlation.
    /// </summary>
    public static ICorrelationStrategy CorrelateBySessionUidOnly(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        new TestingPlatformSessionUidCorrelationStrategy();

    /// <summary>
    /// Creates the default file system results writer that saves result files
    /// to <see cref="AllureConfiguration.Directory"/>.
    /// </summary>
    public static IAllureResultsWriter GetFileSystemResultsWriter(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) =>
        new FileSystemResultsWriter(
            outputDirectory: configuration.Directory,
            indentOutput: configuration.IndentOutput
        );

    /// <summary>
    /// Returns an empty type formatter map.
    /// </summary>
    public static IReadOnlyDictionary<Type, ITypeFormatter> NoTypeFormatters(
        IServiceProvider serviceProvider,
        AllureConfiguration configuration
    ) => ImmutableDictionary<Type, ITypeFormatter>.Empty;

    /// <summary>
    /// Creates the default Allure lifecycle.
    /// </summary>
    public static AllureLifecycle CreateLifecycle(
        IServiceProvider serviceProvider,
        AllureLifecycleFactoryContext lifecycleFactoryContext
    ) =>
        new(
            lifecycleFactoryContext.Config,
            lifecycleFactoryContext.Writer,
            new(lifecycleFactoryContext.TypeFormatters)
        );

    /// <summary>
    /// Creates a factory that returns the specified type formatters.
    /// Each formatter must implement <see cref="TypeFormatter{T}"/> and will be associated
    /// with type <c>T</c>.
    /// </summary>
    public static Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> ExplicitTypeFormatters(
        params IEnumerable<ITypeFormatter> formatters
    ) =>
        (_, _) => formatters
            .Select((formatter) =>
                formatter.GetType() is { IsGenericType: true } type
                    && type.GetGenericTypeDefinition() == typeof(TypeFormatter<>)
                        ? (type.GetGenericArguments()[0], formatter)
                        : ((Type TargetType, ITypeFormatter Formatter)?)null
            )
            .Where((p) => p.HasValue)
            .ToImmutableDictionary(
                (p) => p!.Value.TargetType,
                (p) => p!.Value.Formatter
            );
}
