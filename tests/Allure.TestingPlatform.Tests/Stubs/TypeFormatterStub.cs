using Allure.Net.Commons;

namespace Allure.TestingPlatform.Tests.Stubs;

public class TypeFormatterStub<T>(string stubValue) : TypeFormatter<T>
{
    public override string Format(T value) => stubValue;
}