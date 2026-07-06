namespace Allure.Build.SourceGenerators.Samples;

sealed record class RegistryEntry(
    string RegistryNamespace,
    string SampleName,
    string SourcePath,
    string ProjectFilePath,
    string ProjectRelativePath,
    string ResultsDirectory
);
