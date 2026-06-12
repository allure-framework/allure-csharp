using System;
using System.Collections.Generic;
using System.IO;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Newtonsoft.Json.Linq;

namespace Allure.TestingPlatform.Registration;

public class AllureInfrastructureBuilder : IAllureRegistrationContext
{
    Func<AllureConfiguration> configurationFactory = () =>
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

    Func<AllureConfiguration, IAllureResultsWriter> writerFactory =
        (cfg) => new FileSystemResultsWriter(cfg.Directory, cfg.IndentOutput);

    Func<AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory =
        (deps) => new AllureLifecycle(deps.Config, deps.Writer, deps.TypeFormatters);

    Func<AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory =
        (cfg) => [];

    public IAllureRegistrationContext UseConfiguration(Func<AllureConfiguration> configurationFactory)
    {
        this.configurationFactory = configurationFactory;
        return this;
    }

    public IAllureRegistrationContext UseWriter(Func<AllureConfiguration, IAllureResultsWriter> writerFactory)
    {
        this.writerFactory = writerFactory;
        return this;
    }

    public IAllureRegistrationContext UseLifecycle(Func<AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory)
    {
        this.lifecycleFactory = lifecycleFactory;
        return this;
    }

    public IAllureRegistrationContext UseTypeFormatters(Func<AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory)
    {
        this.typeFormattersFactory = typeFormattersFactory;
        return this;
    }

    public IAllureInfrastructure Build()
    {
        var config = this.configurationFactory();
        var writer = this.writerFactory(config);
        var typeFormatters = this.typeFormattersFactory(config);
        var lifecycle = this.lifecycleFactory(new(config, writer, typeFormatters));

        return new AllureInfrastructure(config, writer, lifecycle, typeFormatters);
    }
}
