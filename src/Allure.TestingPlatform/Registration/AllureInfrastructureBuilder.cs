using System;
using System.Collections.Generic;
using System.IO;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Microsoft.Testing.Platform.Services;
using Newtonsoft.Json.Linq;

namespace Allure.TestingPlatform.Registration;

public class AllureInfrastructureBuilder : IAllureRegistrationContext
{
    Func<IServiceProvider, AllureConfiguration> configurationFactory = (_) =>
    {
        var configEnvVarName = AllureConstants.ALLURE_CONFIG_ENV_VARIABLE;
        var jsonConfigPath = Environment.GetEnvironmentVariable(
            configEnvVarName
        );

        if (jsonConfigPath != null && !File.Exists(jsonConfigPath))
        {
            throw new FileNotFoundException(
                $"Couldn't find '{jsonConfigPath}' specified " +
                    $"in {configEnvVarName} environment variable"
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

        if (File.Exists(defaultJsonConfigPath))
        {
            return AllureConfiguration.ReadFromJObject(
                JObject.Parse(File.ReadAllText(defaultJsonConfigPath))
            );
        }

        return AllureConfiguration.ReadFromJObject(
            JObject.Parse("{}")
        );
    };

    Func<IServiceProvider, AllureConfiguration, bool> isEnabled = (serviceProvider, _) =>
        AllureCliOptionsProvider.IsAllureEnabled(
            serviceProvider.GetCommandLineOptions()
        );

    Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory =
        (_, cfg) => new FileSystemResultsWriter(cfg.Directory, cfg.IndentOutput);

    Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory =
        (_, deps) => new AllureLifecycle(deps.Config, deps.Writer, deps.TypeFormatters);

    Func<IServiceProvider, AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory =
        (_, cfg) => [];

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

    public IAllureInfrastructure Build(IServiceProvider serviceProvider)
    {
        var config = this.configurationFactory(serviceProvider);
        var isEnabled = this.isEnabled(serviceProvider, config);
        var writer = this.writerFactory(serviceProvider, config);
        var typeFormatters = this.typeFormattersFactory(serviceProvider, config);
        var lifecycle = this.lifecycleFactory(serviceProvider, new(config, writer, typeFormatters));

        return new AllureInfrastructure(isEnabled, config, writer, lifecycle, typeFormatters);
    }
}
