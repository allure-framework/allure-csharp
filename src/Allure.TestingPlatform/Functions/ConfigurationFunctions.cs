using System;
using System.IO;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.Services;
using Newtonsoft.Json.Linq;

namespace Allure.TestingPlatform.Functions;

public static class ConfigurationFunctions
{
    public static AllureConfiguration ReadConfiguration<TConfiguration>(
        IServiceProvider serviceProvider,
        string? configPath = null
    )
        where TConfiguration : AllureConfiguration, new()
     =>
        ReadConfiguration<TConfiguration>(
            Environment.GetEnvironmentVariable,
            serviceProvider,
            configPath
        );

    public static AllureConfiguration ReadConfiguration<TConfiguration>(
        Func<string, string?> getEnvironmentVariable,
        IServiceProvider serviceProvider,
        string? configPath = null
    )
        where TConfiguration : AllureConfiguration, new()
    {
        configPath ??= getEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE);

        if (configPath is not null && !File.Exists(configPath))
        {
            throw new FileNotFoundException(
                $"The configuration file '{configPath}' does not exist."
            );
        }

        JObject configJson = File.Exists(configPath)
            ? JObject.Parse(File.ReadAllText(configPath))
            : Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AllureConstants.CONFIG_FILENAME
            ) is { } defaultConfigPath && File.Exists(defaultConfigPath)
                ? JObject.Parse(File.ReadAllText(defaultConfigPath))
                : [];

        var allureSection = configJson["allure"] is { } allureObject
            ? allureObject
            : configJson;

        var cliOptions = serviceProvider.GetCommandLineOptions();
        if (AllureCliOptionsProvider.GetResultsDirectoryValue(cliOptions) is { Length: >0 } cliProvidedDirectory)
        {
            allureSection["directory"] = cliProvidedDirectory;
        }
        else if (allureSection["directory"] is null
            && TestingPlatformFunctions.TryGetDefaultAllureResultsPath(serviceProvider, out var allureResultsDir))
        {
            allureSection["directory"] = allureResultsDir;
        }

        return allureSection?.ToObject<TConfiguration>()
            ?? new();
    }
}