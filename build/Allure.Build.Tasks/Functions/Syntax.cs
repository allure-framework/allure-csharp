using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.Tasks.Functions;

public static class Syntax
{
    public static bool IsValidNamespace(string namespaceName)
        => namespaceName
            .Split('.')
            .All(SyntaxFacts.IsValidIdentifier);
}