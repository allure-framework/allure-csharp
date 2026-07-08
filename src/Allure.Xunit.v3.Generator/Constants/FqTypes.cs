namespace Allure.Xunit.Generator.Constants;

static class FqTypes
{
    public const string ITestApplicationBuilder = $"global::{Types.ITestApplicationBuilder}";

    public const string AllureXunitAttribute = $"global::{Types.AllureXunitAttribute}";

    public const string ExcludeFromCodeCoverage = "global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage";

    public const string TestPlatformTestFramework = "global::Xunit.MicrosoftTestingPlatform.TestPlatformTestFramework";

    public const string IStandaloneAllureRegistrationContext = "global::Allure.TestingPlatform.Registration.IStandaloneAllureRegistrationContext";

    public const string Task_Int = "global::System.Threading.Tasks.Task<int>";

    public const string TestPlanFunctions = "global::Allure.Xunit.Functions.TestPlanFunctions";

    public const string IReadOnlyList_String = "global::System.Collections.Generic.IReadOnlyList<string>";

    public const string List_String = "global::System.Collections.Generic.List<string>";

    public const string IReadOnlyDictionary_Int_StringList = $"global::System.Collections.Generic.IReadOnlyDictionary<int, {IReadOnlyList_String}>";

    public const string Dictionary_Int_StringList = $"global::System.Collections.Generic.Dictionary<int, {IReadOnlyList_String}>";

    public static string Action(string argumentType) =>
        $"global::System.Action<{argumentType}>";

    public static string Action(string argument1Type, string argument2Type) =>
        $"global::System.Action<{argument1Type}, {argument2Type}>";
}
