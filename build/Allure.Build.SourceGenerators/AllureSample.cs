namespace Allure.Build.SourceGenerators;

public record class SampleRegistryEntry(
    string RegistryNamespace,
    string SampleName,
    string SourcePath,
    string ProjectFilePath,
    string ProjectRelativePath,
    string ResultsDirectory
);
