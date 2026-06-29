using System;
using System.IO;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
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
    {
        configPath ??= Environment.GetEnvironmentVariable(
            AllureConstants.ALLURE_CONFIG_ENV_VARIABLE
        );

        if (configPath is not null && !File.Exists(configPath))
        {
            throw new FileNotFoundException(
                $"The configuration ile '{configPath}' does not exist."
            );
        }

        var mtpConfig = serviceProvider.GetConfiguration();
        var mtpResultsDir = mtpConfig["platformOptions:resultDirectory"];
        var defaultResultsDir = mtpResultsDir is not null
            ? Path.Combine(mtpResultsDir, AllureConstants.DEFAULT_RESULTS_FOLDER)
            : null;

        JObject configJson = File.Exists(configPath)
            ? JObject.Parse(File.ReadAllText(configPath))
            : Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AllureConstants.CONFIG_FILENAME
            ) is { } defaultConfigPath && File.Exists(defaultConfigPath)
                ? JObject.Parse(File.ReadAllText(configPath))
                : [];

        if (TestingPlatformFunctions.TryGetDefaultAllureResultsPath(serviceProvider, out var allureResultsDir))
        {
            configJson["allure"] ??= new JObject();
            configJson["allure"]!["directory"] ??= defaultResultsDir;
        }

        var allureSection = configJson["allure"] is { } allureObject
            ? allureObject
            : configJson;

        return allureSection?.ToObject<TConfiguration>()
            ?? new();
    }
}