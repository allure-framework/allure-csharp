namespace Allure.Build.SourceGenerators;

internal static class Constants
{
    public const string ARGUMENT_EXCEPTION_CLASSNAME_FULL = "global::System.ArgumentException";
    public const string NAMESPACE_NAME = "Allure.Testing";
    public const string EXECUTION_NAMESPACE_NAME = $"{NAMESPACE_NAME}.Execution";
    public const string TESTING_PLATFORM_ENUM_NAME = "TestingPlatform";
    public const string TESTING_PLATFORM_ENUM_NAME_FULL = $"global::{EXECUTION_NAMESPACE_NAME}.TestingPlatform";
    public const string TESTING_PLATFORM_VSTEST = $"VsTest";
    public const string TESTING_PLATFORM_VSTEST_FULL = $"{TESTING_PLATFORM_ENUM_NAME_FULL}.{TESTING_PLATFORM_VSTEST}";
    public const string TESTING_PLATFORM_MTP = $"MicrosoftTestingPlatform";
    public const string TESTING_PLATFORM_MTP_FULL = $"{TESTING_PLATFORM_ENUM_NAME_FULL}.{TESTING_PLATFORM_MTP}";
    public const string MSBUILD_PROPS_FILENAME = "AllureBuildProperties.g.cs";
    public const string MSBUILD_PROPS_CLASSNAME = "AllureBuildProperties";
    public const string MSBUILD_PROPS_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{MSBUILD_PROPS_CLASSNAME}";
    public const string REGISTRY_ENTRY_CLASSNAME = "AllureSampleRegistryEntry";
    public const string REGISTRY_ENTRY_CLASSNAME_FULL = $"global::{EXECUTION_NAMESPACE_NAME}.{REGISTRY_ENTRY_CLASSNAME}";
    public const string REGISTRY_CLASSNAME = "AllureSampleRegistry";
    public const string REGISTRY_FILENAME = "AllureSampleRegistry.g.cs";
    public const string SAMPLE_NAME_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_SampleName";
    public const string REGISTRY_NAMESPACE_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_RegistryNamespace";
    public const string PROJECT_FILE_PATH_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_ProjectFilePath";
    public const string PROJECT_RELATIVE_PATH_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_ProjectRelativePath";
    public const string RESULTS_DIRECTORY_METADATA_NAME = "build_metadata.AdditionalFiles.Allure_ResultsDirectory";
    public const string RUNNER_CLASSNAME = "AllureSampleRunner";
    public const string RUNNER_CLASSNAME_FULL = $"global::{NAMESPACE_NAME}.{RUNNER_CLASSNAME}";
    public const string PROP_TESTING_PLATFORM = "Allure_TestingPlatform";
    public const string PROP_TARGET_FRAMEWORK = "Allure_SampleSelectedTargetFramework";
    public const string PROP_CONFIGURATION = "Allure_SampleConfiguration";
    public const string PROP_LEGACY_COMMONS = "Allure_LegacyCommons";
    public const string PROP_PRERUN_FLOW = "Allure_PreRunTestingFlow";
    public const string EDITOR_PROP_PROPERTY_NAMES = "build_property.Allure_PropertyNames";
}