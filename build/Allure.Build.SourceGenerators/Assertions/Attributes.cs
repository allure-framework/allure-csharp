namespace Allure.Build.SourceGenerators.Assertions;

static class Attributes
{
    public static string CallerArgumentExpressionFor(string parameter) =>
        $"[{Types.CallerArgumentExpression}(nameof({parameter}))]";
}
