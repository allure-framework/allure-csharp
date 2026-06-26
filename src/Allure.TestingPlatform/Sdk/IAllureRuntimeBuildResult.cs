using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntimeBuildResult
{
    ILogger Logger { get; }

    AllureConfiguration Configuration { get; }

    IAllureExtensionSettings ExtensionSettings { get; }

    IAllureRuntime CreateRuntime();
}