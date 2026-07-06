namespace Allure.Testing.Execution;

/// <summary>
/// Identifies the testing platform used to execute a test sample project.
/// </summary>
public enum TestingPlatform
{
    /// <summary>
    /// Executes the test sample project through the VSTest platform,
    /// such as via the VSTest-based <c>dotnet test</c> flow.
    /// </summary>
    VsTest,

    /// <summary>
    /// Executes the test sample project through Microsoft Testing Platform,
    /// i.e., via the MTP-based <c>dotnet run</c> flow.
    /// </summary>
    MicrosoftTestingPlatform,
}
