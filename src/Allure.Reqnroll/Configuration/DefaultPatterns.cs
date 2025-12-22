namespace Allure.ReqnrollPlugin.Configuration;
static class DefaultPatterns
{
    internal const string OWNER = @"^allure\.owner:(.+)$";
    internal const string SEVERITY = @"^(trivial|minor|normal|critical|blocker)$";
    internal const string LABEL = @"^allure\.label\.([^:]+):(.+)$";
    internal const string PARENT_SUITE = @"^allure\.parentSuite:(.+)$";
    internal const string SUITE = @"^allure\.suite:(.+)$";
    internal const string SUB_SUITE = @"^allure\.subSuite:(.+)$";
    internal const string EPIC = @"^allure\.epic:(.+)$";
    internal const string STORY = @"^allure\.story:(.+)$";
    internal const string LINK = @"^allure\.link:(.+)$";
    internal const string ISSUE = @"^allure\.issue:(.+)$";
    internal const string TMS = @"^allure\.tms:(.+)$";
}
