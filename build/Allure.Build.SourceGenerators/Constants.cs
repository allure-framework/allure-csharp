namespace Allure.Build.SourceGenerators;

internal static class Constants
{
    public const string NAMESPACE_NAME = "Allure.Testing";
    public const string MSBUILD_PROPS_FILENAME = "AllureBuildProperties.g.cs";
    public const string MSBUILD_PROPS_CLASSNAME = "AllureBuildProperties";
    public const string MSBUILD_PROPS_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{MSBUILD_PROPS_CLASSNAME}";
    public const string REGISTRY_ENTRY_CLASSNAME = "AllureSampleRegistryEntry";
    public const string REGISTRY_ENTRY_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{REGISTRY_ENTRY_CLASSNAME}";
    public const string REGISTRY_CLASSNAME = "AllureSampleRegistry";
    public const string REGISTRY_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{REGISTRY_CLASSNAME}";
    public const string REGISTRY_FILENAME = "AllureSampleRegistry.g.cs";
    public const string PROJECT_SUFFIX_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_ProjectSuffix";
    public const string RUNNER_CLASSNAME = "AllureSampleRunner";
    public const string RUNNER_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{RUNNER_CLASSNAME}";
    public const string PROP_SOLUTION_DIR = "Allure_SampleSolutionDir";
    public const string PROP_SOLUTION_NAME = "Allure_SampleSolutionName";
    public const string PROP_TARGET_FRAMEWORK = "Allure_SampleSelectedTargetFramework";
    public const string PROP_CONFIGURATION = "Allure_SampleConfiguration";
    public const string PROP_PRERUN_FLOW = "Allure_PreRunTestingFlow";
    public const string PROP_RESULTS_DIRECTORY_FMT = "Allure_SampleResultsDirectoryFormat";
    public const string EDITOR_PROP_PROPERTY_NAMES = "build_property.Allure_PropertyNames";
}