using System.Collections.Immutable;

namespace Allure.Abstractions;

public interface IAllureRuntimeRoute
{
    string Id { get; }

    bool IsCurrent { get; }

    ImmutableHashSet<string> SupressRuntimes { get; }

    IAllureRuntime Backend { get; }
}
