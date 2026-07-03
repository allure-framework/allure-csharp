using System.Collections.Immutable;

namespace Allure.Build.Tasks.DataTypes;

public record class AllureSample(
    string Path,
    string SampleName,
    string RegistryNamespace,
    string ProjectName,
    ImmutableArray<(string key, string value)> MsbuildProperties,
    string ItemType,
    ImmutableArray<(string key, string value)> ItemMetadata,
    bool WellDefined
);