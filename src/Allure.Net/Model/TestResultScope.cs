using System.Collections.Generic;

namespace Allure.Model;

/// <summary>
/// Represents a container that groups test results with the fixtures that
/// affected them.
/// </summary>
public sealed class TestResultScope
{
    /// <summary>
    /// Gets or sets the scope's unique identifier.
    /// </summary>
    required public string Uuid { get; set; }

    /// <summary>
    /// Gets or sets the scope's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the identifiers of test results in this scope.
    /// </summary>
    public List<string> Children { get; init; } = [];

    /// <summary>
    /// Gets the setup fixture results.
    /// </summary>
    public List<FixtureResult> Befores { get; init; } = [];

    /// <summary>
    /// Gets the teardown fixture results.
    /// </summary>
    public List<FixtureResult> Afters { get; init; } = [];
}
