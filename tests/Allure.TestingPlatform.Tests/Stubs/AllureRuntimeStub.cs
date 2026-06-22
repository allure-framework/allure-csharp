using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureRuntimeStub(
    bool isEnabled,
    AllureConfiguration config,
    ICorrelationService correlationService,
    InMemoryResultsWriter writer,
    AllureLifecycle lifecycle,
    Dictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureRuntime
{
    public bool IsEnabled => isEnabled;

    public AllureConfiguration Config => config;

    public ICorrelationService CorrelationService => correlationService;

    public IAllureResultsWriter Writer => writer;

    public AllureLifecycle Lifecycle => lifecycle;

    public Dictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;

    IReadOnlyDictionary<Type, ITypeFormatter> IAllureRuntime.TypeFormatters => TypeFormatters;
}