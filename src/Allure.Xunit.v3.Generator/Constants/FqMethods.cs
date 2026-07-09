namespace Allure.Xunit.Generator.Constants;

static class FqMethods
{
    public const string ConsoleRunner_Run = "global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run";

    public const string Error_WriteLine = "global::System.Console.Error.WriteLine";

    public const string Enumerable_Any = "global::System.Linq.Enumerable.Any";

    public const string AddAllureXunit = "global::Allure.Xunit.AllureXunitExtensions.AddAllureXunit";

    public const string ImmutableArray_Create = $"{FqTypes.ImmutableArray}.Create";

    public const string ImmutableDictionary_CreateBuilder_Int_String = $"global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, {FqTypes.ImmutableArray_String}>";

    public const string AddXunitPreExecutionFilters = $"global::Allure.Xunit.AllureXunitTestPlan.{MethodNames.AddXunitPreExecutionFilters}";

    public const string AllureXunitRunner_RunAsync = $"global::Allure.Xunit.AllureXunitRunner.RunAsync";
}
