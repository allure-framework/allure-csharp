# Allure NUnit adapter

[![Nuget release](https://img.shields.io/nuget/v/Allure.NUnit?style=flat)](https://www.nuget.org/packages/Allure.NUnit)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.NUnit?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.NUnit)

> An Allure adapter for [NUnit](https://nunit.org/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

## Quick start

- Install the Allure.NUnit package.
- Configure it via allureConfig.json.
- Apply the `[Allure.NUnit.AllureNUnit]` attribute to test fixtures.
- Use other attributes from `Allure.NUnit.Attributes` if needed.
- Use the functions from `Allure.Net.Commons.AllureApi` if needed.

## Further readings

Learn more from [the documentation for Allure NUnit](https://allurereport.org/docs/nunit/).

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/examples/Allure.NUnit.Examples).

## Notes

### New in 2.15.0: the common Attribute API

Use the attributes in `Allure.Net.Commons.Attributes` instead of `Allure.NUnit.Attributes`. Read more details [here](https://github.com/allure-framework/allure-csharp/pull/647).

In most cases, the migration is as simple as swapping the using directive:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;
using Allure.NUnit;
using NUnit.Framework;

[AllureFeature("My feature")]
class MyTestClass
{
    [AllureStory("My story")]
    [Test]
    public void MyTestMethod()
    {

    }
}
```

In some cases, the usage must be updated. They are listed below.

#### `[AllureDescription]`

Set `Append` to keep the concatenation behavior:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;
using NUnit.Framework;

-[AllureDescription("First description")]
+[AllureDescription("First description", Append = true)]
class MyTestClass
{
-    [AllureDescription("Second description")]
+    [AllureDescription("Second description", Append = true)]
    [Test]
    public void MyTestMethod()
    {

    }
}
```

Use `[AllureDescriptionHtml]` instead of setting `Html`:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;
using NUnit.Framework;

class MyTestClass
{
-    [AllureDescription("<p>Html text</p>", Html = true)]
+    [AllureDescriptionHtml("<p>Html text</p>")]
    [Test]
    public void MyTestMethod()
    {
    }
}
```

#### `[AllureFeature]`, `[AllureStory]` with multiple values

Use multiple `[AllureFeature]` or `[AllureStory]` attributes instead:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureFeature("Feature 1", "Feature 2")]
+[AllureFeature("Feature 1")]
+[AllureFeature("Feature 2")]
-[AllureStory("Story 1", "Story 2")]
+[AllureStory("Story 1")]
+[AllureStory("Story 2")]
class MyTestClass
{
}
```

#### `[AllureLink]`, `[AllureIssue]`, `[AllureTms]`

Pass the URL or ID as the only positional argument. Use the `Title` property to pass the display
text. Also, use `[AllureTmsItem]` instead of `[AllureTms]`:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureLink("Homepage", "https://allurereport.org")]
+[AllureLink("https://allurereport.org", Title = "Homepage")]
-[AllureIssue("ISSUE-123", "123")]
+[AllureIssue("123", Title = "ISSUE-123")]
-[AllureTms("TMS-345", "345")]
+[AllureTmsItem("345", Title = "TMS-345")]
class MyTestClass
{
}
```

#### `[AllureSeverity]`

Always pass an explicit value as the argument:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;

-[AllureSeverity]
+[AllureSeverity(SeverityLevel.normal)]
class MyTestClass
{
}
```

#### `[Name]` and `[Skip]`

Use `[AllureParameter]` with `Name` and `Ignore` correspondingly:

```diff
- using Allure.NUnit.Attributes;
+ using Allure.Net.Commons.Attributes;

class MyTestClass
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

#### Deprecation notice

Attributes from the `Allure.NUnit.Attributes` namespace will be deprecated in one of the future
releases. Please, migrate to `Allure.Net.Commons.Attributes`.

### New in 2.12.0: Namespace changed to Allure.NUnit

Starting from 2.12.0, the namespace `NUnit.Allure` is deprecated. The API in
that namespace still works, but it will be removed in the future. Please use
`Allure.NUnit` instead.

> The `[NUnit.Allure.Core.AllureNUnit]` attribute should be replaced with
> `[Allure.NUnit.AllureNUnit]`:

```c#
using Allure.NUnit; // <- Note the namespace
using NUnit.Framework;

[AllureNUnit]
class MyTests
{
    [Test]
    public void TestMethod()
    {
        /* ... */
    }
}
```

### Deprecations and removals in 2.12.0

The following user API methods are now deprecated:

  - In `NUnit.Allure.Core.AllureExtensions`:
      - All overloads of `WrapInStep` - use `Allure.Net.Commons.AllureApi.Step`
        instead.
      - `WrapSetUpTearDownParams` - had no effect; can safely be replaced with
        the direct call of the provided delegate.
  - `NUnit.Allure.Core.AllureNUnitAttribute` - use
    `Allure.NUnit.AllureNUnitAttribute` instead.
  - Other classes and methods in `NUnit.Allure` - change the namespace to
    `Allure.NUnit`.

The following previously deprecated user API classes and methods were removed:

  - In `NUnit.Allure.Core.AllureExtensions`:
    - `AddScreenDiff` - use `Allure.Net.Commons.AllureApi.AddScreenDiff`
      instead.
  - `NUnit.Allure.Core.AllureNUnitAttribute`'s constructor overload that takes
    `bool wrapIntoStep` - the `wrapIntoStep` parameter had no effect and can be
    safely removed now.
  - In `NUnit.Allure.Core.AllureNUnitHelper`:
    - `WrapInStep` - use `Allure.Net.Commons.AllureApi.Step` instead.
  - `NUnit.Allure.Core.StepsHelper` - use functions from
    `Allure.Net.Commons.AllureApi` and `Allure.Net.Commons.ExtendedApi` instead.
  - In `Allure.Net.Commons.AllureLifecycle`:
    - `AddAttachment` - use `Allure.Net.Commons.AllureApi.AddAttachment`
      instead.
    - `AddScreenDiff` - use `Allure.Net.Commons.AllureApi.AddScreenDiff`
      instead.
  - `Allure.Net.Commons.Steps.CoreStepsHelper` - use functions from
    `Allure.Net.Commons.AllureApi` and `Allure.Net.Commons.ExtendedApi` instead.

### For users of Mac with Apple silicon

If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```