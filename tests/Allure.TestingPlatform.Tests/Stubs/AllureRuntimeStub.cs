using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureRuntimeStub(
    AllureConfiguration config,
    LoggerSpy logger,
    bool isEnabled,
    ICorrelationService correlationService,
    InMemoryResultsWriter writer,
    AllureLifecycle lifecycle,
    Dictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureRuntime
{
    public AllureConfiguration Config => config;

    public ILogger Logger => logger;

    public bool IsEnabled => isEnabled;

    public ICorrelationService CorrelationService => correlationService;

    public IAllureResultsWriter Writer => writer;

    public AllureLifecycle Lifecycle => lifecycle;

    public Dictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;

    IReadOnlyDictionary<Type, ITypeFormatter> IAllureRuntime.TypeFormatters => TypeFormatters;
}