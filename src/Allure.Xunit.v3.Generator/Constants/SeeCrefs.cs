namespace Allure.Xunit.Generator.Constants;

static class SeeCrefs
{
    public const string ConsoleRunner_Run = $"{FqMembers.ConsoleRunner_Run}(string[])";

    public const string AddAllureXunit =
        $"{FqMembers.AddAllureXunit}({FqTypes.ITestApplicationBuilder}, "
            + $"global::System.Action{{{FqTypes.IAllureXunitRegistrationContext}}})";
}
