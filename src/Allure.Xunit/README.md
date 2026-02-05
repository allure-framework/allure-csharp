# Allure.Xunit

[![Nuget release](https://img.shields.io/nuget/v/Allure.Xunit?style=flat)](https://www.nuget.org/packages/Allure.Xunit)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.Xunit?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Xunit)

> An Allure adapter for [xUnit.net](https://xunit.net/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

Allure Xunit supports the following frameworks:

  - .NET Core 3.1,
  - .NET 5.0 or greater.

## Quick start

Install the Allure.Xunit package and run the tests normally. In many cases,
allure should start automatically. The result files are created in the
`allure-results` directory in the target directory.
If that didn't happen, check out the [Why the Allure results directory is empty?](#why-the-allure-results-directory-is-empty)
section.

## Further readings

Learn more from [the documentation for Allure Xunit](https://allurereport.org/docs/xunit/).

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/examples/Allure.Xunit.Examples).

## Notes

### New in 2.15.0: the common Attribute API

Use the attributes in `Allure.Net.Commons.Attributes` instead of `Allure.Xunit.Attributes`. Read more details [here](https://github.com/allure-framework/allure-csharp/pull/647).

In most cases, the migration is straightforward:

```diff
- using Allure.Xunit.Attributes;
+ using Allure.Net.Commons.Attributes;
using Xunit;

[AllureFeature("My feature")]
public class MyTestClass
{
    [AllureStory("My story")]
    [Fact]
    public void MyTestMethod()
    {

    }
}
```

In some cases, the usage must be updated. Such cases are listed below.

#### `[AllureFeature]`, `[AllureStory]` with multiple values

Use multiple `[AllureFeature]` or `[AllureStory]` attributes instead:

```diff
- using Allure.Xunit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureFeature("Feature 1", "Feature 2")]
+[AllureFeature("Feature 1")]
+[AllureFeature("Feature 2")]
-[AllureStory("Story 1", "Story 2")]
+[AllureStory("Story 1")]
+[AllureStory("Story 2")]
public class MyTestClass
{
}
```

#### `[AllureLink]`, `[AllureIssue]`

Pass the URL or ID as the only positional argument. Use the `Title` property to pass the display
text:

```diff
- using Allure.Xunit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureLink("Homepage", "https://allurereport.org")]
+[AllureLink("https://allurereport.org", Title = "Homepage")]
-[AllureIssue("ISSUE-123", "123")]
+[AllureIssue("123", Title = "ISSUE-123")]
public class MyTestClass
{
}
```

#### `[AllureSeverity]`

Always pass an explicit value as the argument:

```diff
- using Allure.Xunit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureSeverity]
+[AllureSeverity(SeverityLevel.normal)]
public class MyTestClass
{
}
```

#### `[Name]` and `[Skip]`

Use `[AllureParameter]` with `Name` and `Ignore` correspondingly:

```diff
- using Allure.Xunit.Attributes.Steps;
+ using Allure.Net.Commons.Attributes;

public class MyTestClass
{
    [AllureStep]
    public void MyStepMethod(
-        [Name("Foo")] int parameter1,
+        [AllureParameter(Name = "Foo")] int parameter1,
-        [Skip] int parameter2
+        [AllureParameter(Ignore = true)] int parameter2
    )
    {
    }
}
```

#### `[AllureId]`

Pass an integer value instead of a string:

```diff
- using Allure.Xunit.Attributes;
+ using Allure.Net.Commons.Attributes;
using Xunit;

public class MyTestClass
{
    [Fact]
-   [AllureId("102")]
+   [AllureId(102)]
    public void MyTestMethod()
    {
    }
}
```

#### The `Overwrite` argument

The new attributes don't have an equivalent of the `overwrite` argument that removes the existing
metadata. You should rearrange the attributes to avoid duplicates:

```diff
-using Allure.Xunit.Attributes;
+using Allure.Net.Commons.Attributes;
using Xunit;

-[AllureEpic(["Epic 1"], true)]
public class TestClass
{
    [Fact]
-   [AllureEpic(["Epic 2"], true)]
+   [AllureEpic("Epic 2")]
    public void Test1()
    {

    }

    [Fact]
-   [AllureEpic(["Epic 3"], true)]
+   [AllureEpic("Epic 3")]
    public void Test1()
    {

    }
}
```

#### Deprecation notice

Attributes from the `Allure.Xunit.Attributes` namespace will be deprecated in one of the future
releases. Please, migrate to `Allure.Net.Commons.Attributes`.

### New in 2.12.0: Namespaces consolidated to Allure.Xunit

Previously, the package contained a mix of `Allure.Xunit` and `Allure.XUnit`
namespaces. Starting from 2.12.0, you should only use `Allure.Xunit`. The API is
still accessible through the old namespace, but that access is deprecated now
and will be removed in the future.

### Deprecations and removals in 2.12.0

The following user API classes are now deprecated:

  - `Allure.XUnit.Attachments` - use `Allure.Xunit.Attachments` instead.
  - Attributes in `Allure.XUnit.Attributes.Steps` - use their counterparts from
    `Allure.Xunit.Attributes.Steps`.

The following previously deprecated user API classes and methods were removed:

  - Using-style steps/fixtures:
      - `Allure.Xunit.AllureAfter`
      - `Allure.Xunit.AllureBefore`
      - `Allure.Xunit.AllureStep`
      - `Allure.Xunit.AllureStepBase`

    Use the following alternatives instead:
      - Attributes from `Allure.Xunit.Attributes.Steps` (`[AllureAfter]`,
        `[AllureBefore]`, or `[AllureStep]`)
      - Functions from `Allure.Net.Commons.AllureApi` and
        `Allure.Net.Commons.ExtendedApi`
  - `Allure.Xunit.AllureAttachments` - use
    `Allure.Net.Commons.AllureApi.AddAttachment` instead.
  - `Allure.Xunit.Steps` - use functions from
    `Allure.Net.Commons.AllureApi` and `Allure.Net.Commons.ExtendedApi` instead.
  - In `Allure.Xunit.Attributes`:
      - `AllureXunitAttribute` - use `Xunit.FactAttribute` instead.
      - `AllureXunitTheoryAttribute` - use `Xunit.TheoryAttribute` instead.
  - In `Allure.Net.Commons.AllureLifecycle`:
    - `AddAttachment` - use `Allure.Net.Commons.AllureApi.AddAttachment`
      instead.
    - `AddScreenDiff` - use `Allure.Net.Commons.AllureApi.AddScreenDiff`
      instead.
  - `Allure.Net.Commons.Steps.CoreStepsHelper` - use functions from
    `Allure.Net.Commons.AllureApi` and `Allure.Net.Commons.ExtendedApi` instead.

### Allure.Xunit.StepExtensions deprecation

There is no more need to use the separate Allure.XUnit.StepExtensions package.
You should uninstall it and use attributes from the
`Allure.Xunit.Attributes.Steps` namespace directly.

## Known issues and limitations

### Rosetta is required for users on Mac with Apple silicon

If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```

### MonoMod.Core issues

We rely on Harmony (which in turn uses MonoMod.Core) to:

1. Report arguments of theories in case they aren't reported by xUnit.net itself.
2. Implement selectie run (test plans).

Those features might not work in some rare circumstances, especially when testing the `Release` configuration. If you're affected, try switching to `Debug` as a workaround.

Issue [#369] contains some additional details.

[#369]: https://github.com/allure-framework/allure-csharp/issues/369

## Troubleshooting

### Why the Allure results directory is empty?

If you run your tests, but there is no Allure results directory (or it's empty),
xUnit.net may have preferred another reporter instead of `allure`.

You can force xUnit.net to select the `allure` reporter by providing it to the
runner. For `xunit.runner.visualstudio`, it could be done with the
`xUnit.ReporterSwitch` run setting:

```
dotnet test <test-project-name> -- xUnit.ReporterSwitch=allure
```

Alternatively, you may provide that option via a `.runsettings` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <xUnit>
    <ReporterSwitch>allure</ReporterSwitch>
  </xUnit>
</RunSettings>
```

```
dotnet test -s <path-to-runsettings> <test-project-name>
```

> If you run the tests via an IDE, refer to that IDE's documentation to figure
> out how to provide a `.runsettings` file.

Check the test logs. If Allure.Xunit has run, the following entry should exist:

```
[xUnit.net 00:00:00.53] Allure.Xunit: Allure reporter enabled
```

> If you don't see xUnit.net logs, try increasing the verbosity level to `normal` or `detailed`:
>
> ```
> dotnet test --logger 'console;verbosity=normal'
> ```

### How to run Allure.Xunit together with another reporter?

xUnit.net only uses one reporter per run. But Allure Xunit allows you to bypass
that limitation. Learn more [here](https://allurereport.org/docs/xunit-configuration/#allurexunitrunnerreporter).

### How to use Allure Xunit in a CI environment?

A CI-specific reporter might be enabled in addition to Allure Xunit in some CI
environments like Azure DevOps or TeamCity. The result is that both reporters
become available to run. In such cases, xUnit.net may select any of them.

  - If xUnit.net selects Allure Xunit, the tests will be reported both to the
    CI server and as Allure results.
  - If a CI-specific reporter is selected, the tests will only be reported to
    the CI server.

To fix that, make sure that [xUnit.net always picks Allure Xunit](#why-the-allure-results-directory-is-empty).
