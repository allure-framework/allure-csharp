using System;
using System.Collections.Generic;
using System.IO;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Services;
using Newtonsoft.Json.Linq;

namespace Allure.TestingPlatform.Implementation;

public class AllureRuntimeBuilder : IAllureRuntimeBuilder
{
    Func<IServiceProvider, AllureConfiguration> configurationFactory = static (serviceProvider) =>
    {
        var configEnvVarName = AllureConstants.ALLURE_CONFIG_ENV_VARIABLE;
        var jsonConfigPath = Environment.GetEnvironmentVariable(
            configEnvVarName
        );

        if (jsonConfigPath != null && !File.Exists(jsonConfigPath))
        {
            throw new FileNotFoundException(
                $"Couldn't find '{jsonConfigPath}' specified " +
                    $"by the '{configEnvVarName}' environment variable"
            );
        }

        if (File.Exists(jsonConfigPath))
        {
            return AllureConfiguration.ReadFromJObject(
                JObject.Parse(File.ReadAllText(jsonConfigPath))
            );
        }

        var defaultJsonConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            AllureConstants.CONFIG_FILENAME
        );

        var mtpConfig = serviceProvider.GetConfiguration();
        var mtpResultsDir = mtpConfig["platformOptions:resultDirectory"];
        var defaultResultsDir = mtpResultsDir is not null
            ? Path.Combine(mtpResultsDir, AllureConstants.DEFAULT_RESULTS_FOLDER)
            : null;

        if (File.Exists(defaultJsonConfigPath))
        {
            var json = JObject.Parse(File.ReadAllText(defaultJsonConfigPath));
            json["allure"] ??= new JObject();
            if (defaultResultsDir is not null)
            {
                json["allure"]!["directory"] ??= defaultResultsDir;
            }
            return AllureConfiguration.ReadFromJObject(json);
        }

        return AllureConfiguration.ReadFromJObject(
            defaultResultsDir is not null
                ? new JObject
                {
                    { "allure", new JObject { { "directory", defaultResultsDir } } },
                }
                : []
        );
    };

    Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory = static (serviceProvider, _) =>
        serviceProvider
            .GetLoggerFactory()
            .CreateLogger("Allure.TestingPlatform");

    Func<IServiceProvider, AllureConfiguration, bool> isEnabled = static (_, _) => true;

    Func<IServiceProvider, AllureConfiguration, ICorrelationService> correlationServiceFactory =
        static (_, _) => new SessionUidCorrelation();

    Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory =
        static (_, cfg) => new FileSystemResultsWriter(cfg.Directory, cfg.IndentOutput);

    Func<IServiceProvider, AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory =
        static (_, cfg) => [];

    Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory =
        static (_, deps) => new AllureLifecycle(
            deps.Config,
            deps.Writer,
            new Dictionary<Type, ITypeFormatter>(deps.TypeFormatters)
        );

    public IAllureRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    )
    {
        this.loggerFactory = loggerFactory;
        return this;
    }

    public IAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    )
    {
        this.configurationFactory = configurationFactory;
        return this;
    }

    public IAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    )
    {
        this.isEnabled = isEnabled;
        return this;
    }

    public IAllureRegistrationContext UseCorrelationService(
        Func<IServiceProvider, AllureConfiguration, ICorrelationService> correlationServiceFactory
    )
    {
        this.correlationServiceFactory = correlationServiceFactory;
        return this;
    }

    public IAllureRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    )
    {
        this.writerFactory = writerFactory;
        return this;
    }

    public IAllureRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    )
    {
        this.lifecycleFactory = lifecycleFactory;
        return this;
    }

    public IAllureRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory
    )
    {
        this.typeFormattersFactory = typeFormattersFactory;
        return this;
    }

    public IAllureRuntimeBuildResult Build(IServiceProvider serviceProvider)
    {
        var configuration = this.configurationFactory(serviceProvider);
        var logger = loggerFactory(serviceProvider, configuration);
        var settings = new AllureExtensionSettings(
            IsEnabled: AllureTestingPlatformCliOptions.IsAllureEnabled(
                serviceProvider.GetCommandLineOptions()
            ) && this.isEnabled(serviceProvider, configuration)
        );

        return new AllureRuntimeBuildResult(
            configuration: configuration,
            logger: logger,
            settings: settings,
            correlationServiceFactory: (cfg) => this.correlationServiceFactory(serviceProvider, cfg),
            writerFactory: (cfg) => this.writerFactory(serviceProvider, cfg),
            typeFormattersFactory: (cfg) => this.typeFormattersFactory(serviceProvider, cfg),
            lifecycleFactory: (ctx) => this.lifecycleFactory(serviceProvider, ctx)
        );
    }
}
