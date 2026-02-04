namespace Allure.Testing;

/// <summary>
/// Contains the data required to run a specific sample project and access its output.
/// </summary>
/// <remarks>
/// Instances are typically created and filled by the testing infrastructure. Use the
/// auto-generated <c>Allure.Testing.AllureSampleRegistry</c> class to access them.
/// </remarks>
/// <param name="RegistryId">
/// The id (i.e., the namespace) of the registry that contains the sample.
/// </param>
/// <param name="SampleId">The id of the sample in its registry.</param>
/// <param name="ProjectFilePath">The path to the <c>.csproj</c> file of the sample project.</param>
/// <param name="DefaultResultsPath">
/// The path to a directory where Allure result files are stored by default.
/// </param>
/// <param name="TargetFramework">
/// A target framework moniker that specifies the target framework for the sample project.
/// Example: <c>"net10.0"</c>.
/// </param>
/// <param name="BuildConfiguration">
/// A configuration to use when building the sample.
/// Example: <c>"Debug"</c>.
/// </param>
/// <param name="IsPreRunFlow">
/// If <c>true</c>, indicates the pre-run flow should be respected by the runner.
/// In this flow, the runner don't run the sample.
/// Instead, it parses the result files that must be prepared in advance in the directory
/// pointed by <paramref name="DefaultResultsPath"/>.
/// The results are typically prepared by <c>dotnet msbuild -t:Allure_RunTestSamples</c>.
/// </param>
public record struct AllureSampleRegistryEntry(
    string RegistryId,
    string SampleId,
    string ProjectFilePath,
    string DefaultResultsPath,
    string TargetFramework,
    string BuildConfiguration,
    bool IsPreRunFlow
);
