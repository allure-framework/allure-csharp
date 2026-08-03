using System.Collections.Generic;

namespace Allure.Model;

/// <summary>
/// Represents a test result written for Allure Report.
/// </summary>
public sealed class TestResult : ExecutableItem
{
    /// <summary>
    /// Gets the unique identifier of this test result.
    /// </summary>
    required public string Uuid { get; init; }

    /// <summary>
    /// Gets or sets the test's fully qualified name.
    /// </summary>
    /// <remarks>
    /// A full name must uniquely identify a test case. It must not depend
    /// on test parameters.
    /// </remarks>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the test case the result belongs to.
    /// </summary>
    /// <remarks>
    /// This value must have a fixed length and uniquely identify the test case
    /// the result belongs to (up to a collision).
    /// It must not depend on test parameters.
    /// </remarks>
    public string? TestCaseId { get; set; }

    /// <summary>
    /// Gets or sets the identifier used to associate the test with its history.
    /// </summary>
    /// <remarks>
    /// This value must have a fixed length and uniquely identify a test being
    /// run (up to a collision). It must depend on all non-excluded Allure
    /// parameters collected through the test execution.
    /// </remarks>
    public string? HistoryId { get; set; }

    /// <summary>
    /// Gets the hierarchy of titles leading to the test.
    /// </summary>
    public List<string> TitlePath { get; init; } = [];

    /// <summary>
    /// Gets the test's labels.
    /// </summary>
    public List<Label> Labels { get; init; } = [];

    /// <summary>
    /// Gets the test's links.
    /// </summary>
    public List<Link> Links { get; init; } = [];
}
