namespace Allure.Xunit.Generator.Constants;

static class SeeCrefs
{
    public const string ConsoleRunner_Run = $"{FqMethods.ConsoleRunner_Run}(string[])";

    public const string AddAllureXunit =
        $"{FqMethods.AddAllureXunit}({FqTypes.ITestApplicationBuilder}, "
            + $"global::System.Action{{{FqTypes.IStandaloneAllureRegistrationContext}}})";
}
