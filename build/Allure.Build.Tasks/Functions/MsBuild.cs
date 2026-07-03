using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Allure.Build.Tasks.Functions;

public static class MsBuild
{
    public const string MSBuildThisFileDirectory = $"$(MSBuildThisFileDirectory)";

    public static string NormalizePath(params IEnumerable<string> fragments) =>
        $"$([MSBuild]::NormalizePath({string.Join(", ", fragments.Select(f => $"'{f}'"))}))";

    public static string NormalizeDirectory(params IEnumerable<string> fragments) =>
        $"$([MSBuild]::NormalizeDirectory({string.Join(", ", fragments.Select(f => $"'{f}'"))}))";

    public static string NormalizeToThisFileDirectory(params IEnumerable<string> fragments) =>
        NormalizePath([MSBuildThisFileDirectory, ..fragments]);

    public static string NormalizeDirectoryToThisFileDirectory(params IEnumerable<string> fragments) =>
        NormalizeDirectory([MSBuildThisFileDirectory, ..fragments]);

    public static XElement GetImport(
        string importPath
    ) => new(
        "Import",
        new XAttribute(
            "Project",
            NormalizeToThisFileDirectory(importPath)
        )
    );

    public static XElement GetImportFromParentDir(
        string fileName
    )
    {
        var xx = NormalizeDirectoryToThisFileDirectory("..");
        return new(
            "Import",
            new XAttribute(
                "Project",
                $"$([MSBuild]::GetPathOfFileAbove('{fileName}', '{xx}'))"
            ),
            new XAttribute(
                "Condition",
                $"'' != $([MSBuild]::GetPathOfFileAbove('{fileName}', '{xx}'))"
            )
        );
    }

    public static XElement GetPropertyGroup(params IEnumerable<(string key, string value)> props) => new(
        "PropertyGroup",
        props.Select(static (kv) => new XElement(kv.key, kv.value))
    );

    public static XElement GetItemGroup(
        string itemName,
        params IEnumerable<IEnumerable<(string key, string value)>> items
    ) => new(
        "ItemGroup",
        items.Select((meta) => new XElement(
            itemName,
            meta.Select(static (kv) => new XAttribute(kv.key, kv.value))
        ))
    );

    public static XElement GetItemGroup<TSource>(
        IEnumerable<TSource> source,
        string itemName,
        Func<TSource, IEnumerable<(string key, string value)>> selector
    ) =>
        GetItemGroup(itemName, source.Select(selector));
}