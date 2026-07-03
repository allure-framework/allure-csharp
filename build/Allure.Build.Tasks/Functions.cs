using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.Tasks;

public static class Functions
{
    public static bool IsValidNamespace(string namespaceName)
        => namespaceName
            .Split('.')
            .All(SyntaxFacts.IsValidIdentifier);

    public static string GetGreatestCommonPrefix(IEnumerable<string> paths)
    {
        var first = paths.First();
        var rest = paths.Skip(1).ToList();

        string prefix = first;

        while ((prefix = Path.GetDirectoryName(prefix)) is not null)
        {
            if (IsCommonPrefix(rest, prefix))
            {
                return prefix;
            }
        }

        return "";
    }

    public static bool IsCommonPrefix(List<string> files, string prefix) =>
        files.All((path) =>
            path.StartsWith(prefix)
                && path.Length > prefix.Length
                && path[prefix.Length] == Path.DirectorySeparatorChar);

    public static string NormalizePath(string path)
    {
        if (Path.DirectorySeparatorChar != '/')
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);
        }

        if (Path.DirectorySeparatorChar != '\\')
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
        }

        return path;
    }

    public static string GetNormalizedFullPath(string path, string basePath) =>
        Path.GetFullPath(NormalizePath(path), basePath);

    public static string ResolveToNewBase(string path, string basePath, string newBasePath) =>
        Path.GetRelativePath(newBasePath, GetNormalizedFullPath(path, basePath));
}