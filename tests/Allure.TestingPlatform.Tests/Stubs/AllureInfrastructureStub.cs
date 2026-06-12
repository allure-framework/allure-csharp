using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureInfrastructureStub(
    AllureConfiguration config,
    InMemoryResultsWriter writer,
    AllureLifecycle lifecycle,
    Dictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureInfrastructure
{
    public AllureConfiguration Config => config;

    public IAllureResultsWriter Writer => writer;

    public AllureLifecycle Lifecycle => lifecycle;

    public Dictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;

    IReadOnlyDictionary<Type, ITypeFormatter> IAllureInfrastructure.TypeFormatters => TypeFormatters;
}