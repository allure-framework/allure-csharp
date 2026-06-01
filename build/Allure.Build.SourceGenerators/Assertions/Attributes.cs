namespace Allure.Build.SourceGenerators.Assertions;

public static class Attributes
{
    public static string CallerArgumentExpressionFor(string parameter) =>
        $"[{Types.CallerArgumentExpression}(nameof({parameter}))]";
}
