namespace Allure.Abstractions;

public interface IAllureBackendDispatcher
{
    string Name { get; }

    bool IsCurrent { get; }

    IAllureTestRuntimeBackend Backend { get; }
}
