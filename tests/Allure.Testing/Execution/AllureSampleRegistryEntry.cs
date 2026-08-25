namespace Allure.Testing.Execution;

/// <summary>
/// Contains the data required to run a specific sample project and access its output.
/// </summary>
/// <remarks>
/// Instances are typically created and populated by the testing infrastructure. Use the
/// auto-generated <c>AllureSampleRegistry</c> class to access them.
/// </remarks>
/// <param name="TestingPlatform">
/// Defines which testing platform, VSTest or MTP, the runner should use to execute the samples.
/// </param>
/// <param name="RegistryId">
/// The ID of the registry that contains the sample, typically its namespace.
/// This value is only used for diagnostics.
/// </param>
/// <param name="SampleId">The ID of the sample in its registry.</param>
/// <param name="ProjectFilePath">The path to the <c>.csproj</c> file of the sample project.</param>
/// <param name="DefaultResultsPath">
/// The path to a directory where Allure result files are stored by default.
/// </param>
/// <param name="TargetFramework">
/// A target framework moniker for the sample project.
/// Example: <c>"net10.0"</c>.
/// </param>
/// <param name="BuildConfiguration">
/// A configuration to use when building the sample.
/// Example: <c>"Debug"</c>.
/// </param>
/// <param name="IsPreRunFlow">
/// If <c>true</c>, indicates that the runner should use the pre-run flow.
/// In this flow, the runner does not run the sample.
/// Instead, it parses result files prepared in advance in the directory specified by
/// <paramref name="DefaultResultsPath"/>.
/// These results are typically prepared by <c>dotnet msbuild -t:Allure_RunTestSamples</c>.
/// </param>
/// <param name="LegacyCommons">
/// Setting this flag to <see langword="true"/> enables compatibility with the legacy
/// <c>Allure.Net.Commons</c> library.
/// </param>
public record struct AllureSampleRegistryEntry(
    TestingPlatform TestingPlatform,
    string RegistryId,
    string SampleId,
    string ProjectFilePath,
    string DefaultResultsPath,
    string TargetFramework,
    string BuildConfiguration,
    bool IsPreRunFlow,
    bool LegacyCommons
);
