using System.Collections.Generic;

namespace Allure.Model;

public sealed class Scope
{
    required public string Uuid { get; set; }

    required public string? Name { get; set; }

    public List<string> Children { get; init; } = [];

    public List<FixtureResult> Befores { get; init; } = [];

    public List<FixtureResult> Afters { get; init; } = [];
}
