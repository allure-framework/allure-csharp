using Allure.Abstractions;

namespace Allure.Sdk.Registration;

public interface IPreparedInProcessRouteBuilder
{
    IAllureRuntimeRoute Build();
}
