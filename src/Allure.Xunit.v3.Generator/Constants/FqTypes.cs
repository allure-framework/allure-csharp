namespace Allure.Xunit.Generator.Constants;

static class FqTypes
{
    public const string ITestApplicationBuilder = $"global::{Types.ITestApplicationBuilder}";

    public const string AllureXunitAttribute = $"global::{Types.AllureXunitAttribute}";

    public const string ExcludeFromCodeCoverage = "global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage";

    public const string TestPlatformTestFramework = "global::Xunit.MicrosoftTestingPlatform.TestPlatformTestFramework";

    public const string IAllureXunitRegistrationContext = $"global::{Namespaces.AllureXunitRegistration}.IAllureXunitRegistrationContext";

    public const string Task_Int = "global::System.Threading.Tasks.Task<int>";

    public const string AllureXunitTestPlan = $"global::{Namespaces.AllureXunit}.AllureXunitTestPlan";

    public const string ImmutableArray = $"global::System.Collections.Immutable.ImmutableArray";

    public const string ImmutableArray_String = $"{ImmutableArray}<string>";

    public const string ImmutableDictionary = $"global::System.Collections.Immutable.ImmutableDictionary";

    public const string ImmutableDictionary_Int_ImmutableArray_String = $"{ImmutableDictionary}<int, {ImmutableArray_String}>";

    public const string ImmutableDictionaryBuilder_Int_ImmutableArray_String = $"{ImmutableDictionary_Int_ImmutableArray_String}.Builder";

    public static string Action(string argumentType) =>
        $"global::System.Action<{argumentType}>";

    public static string Action(string argument1Type, string argument2Type) =>
        $"global::System.Action<{argument1Type}, {argument2Type}>";

    public static string AllureIdAttribute = $"global::{Types.AllureIdAttribute}";

}
