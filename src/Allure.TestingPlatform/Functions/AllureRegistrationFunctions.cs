using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Internal.Registration;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Functions;

public static class AllureRegistrationFunctions
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

    public static bool DoNotDisable(
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

    internal static IAllureTestingPlatformRuntimeRegistry RegisterAllureTestingPlatform(
        ITestApplicationBuilder builder,
        Action<AllureTestingPlatformRegistration> configureAllure,
        AllureTestingPlatformRegistrationMode registrationMode
    )
    {
        builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

        var allureRegistration = new AllureTestingPlatformRegistration(registrationMode);
        configureAllure(allureRegistration);
        var frozenRegistration = allureRegistration.Prepare();
        var registrationResults = frozenRegistration.RegistrationResults;

        var factory =
            new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                new AllureDataConsumer(
                    registrationResults.GetRuntimeProvider(serviceProvider)
                )
            );

        if (frozenRegistration.HostProcessWatchdogEnabled)
        {
            builder.TestHostControllers.AddProcessLifetimeHandler((serviceProvider) =>
                new AllureTestingPlatformHostProcessWatchdog(
                    frozenRegistration.CreateBuilder(serviceProvider)
                )
            );
        }

        builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
            new AllureTestingPlatformInProcessOwner(
                frozenRegistration.CreateBuilder(serviceProvider)
            )
        );
        builder.TestHost.AddDataConsumer(factory);
        builder.TestHost.AddTestSessionLifetimeHandler(factory);

        return frozenRegistration.RegistrationResults;
    }
}
