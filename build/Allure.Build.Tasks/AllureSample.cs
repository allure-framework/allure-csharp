using System.Collections.Immutable;

namespace Allure.Build.Tasks;

public record class AllureSample(
    string Path,
    string SampleName,
    string RegistryNamespace,
    string ProjectName,
    ImmutableArray<(string key, string value)> MsbuildProperties,
    bool WellDefined
);