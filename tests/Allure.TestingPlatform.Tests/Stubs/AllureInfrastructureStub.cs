using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureInfrastructureStub(
    bool isEnabled,
    AllureConfiguration config,
    ICorrelationDefinition correlationDefinition,
    InMemoryResultsWriter writer,
    AllureLifecycle lifecycle,
    Dictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureInfrastructure
{
    public bool IsEnabled => isEnabled;

    public AllureConfiguration Config => config;

    public ICorrelationDefinition CorrelationDefinition => correlationDefinition;

    public IAllureResultsWriter Writer => writer;

    public AllureLifecycle Lifecycle => lifecycle;

    public Dictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;

    IReadOnlyDictionary<Type, ITypeFormatter> IAllureInfrastructure.TypeFormatters => TypeFormatters;
}