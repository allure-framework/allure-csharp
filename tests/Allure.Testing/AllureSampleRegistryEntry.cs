namespace Allure.Testing;

/// <summary>
/// Contains the data required to run a specific sample project and access its output.
/// </summary>
/// <remarks>
/// Instances are typically created and filled by the testing infrastructure. Use the
/// auto-generated <c>Allure.Testing.AllureSampleRegistry</c> class to access them.
/// </remarks>
/// <param name="Id">The id (the project suffix) of the sample.</param>
/// <param name="ProjectPath">The path to the <c>.csproj</c> file.</param>
/// <param name="DefaultResultsPath">
/// The path to a directory where Allure result files are stored by default.
/// </param>
/// <param name="TargetFramework">
/// A target framework moniker that specifies the target framework for the sample project.
/// Example: <c>net10.0</c>.
/// </param>
/// <param name="BuildConfiguration">
/// A configuration to use when building the sample.
/// Example: <c>Debug</c>.
/// </param>
public record class AllureSampleRegistryEntry(
    string Id,
    string ProjectPath,
    string DefaultResultsPath,
    string TargetFramework,
    string BuildConfiguration
);
