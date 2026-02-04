namespace Allure.Build.SourceGenerators;

public record class AllureSample(
    string Path,
    string SampleName,
    string RegistryNamespace,
    string ProjectFilePath,
    string ProjectRelativePath,
    string ResultsDirectory
);
