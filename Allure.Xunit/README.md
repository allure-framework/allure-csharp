# Allure.Xunit

[![Nuget](https://img.shields.io/nuget/v/Allure.Xunit?style=flat)](https://www.nuget.org/packages/Allure.Xunit)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.Xunit?style=flat)](https://www.nuget.org/packages/Allure.Xunit)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.xunit?label=downloads&style=flat)

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

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/Allure.Xunit.Examples).

## Notes

### Namespace consolidated to Allure.Xunit

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

There is no more need to use separate Allure.XUnit.StepExtensions package. You
should uninstall it and use attributes from
[Allure.Xunit.Attributes.Steps namespace](Attributes/Steps) directly.

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

We rely on Harmony that in turn uses MonoMod.Core to:

1. Report arguments of theories in case they aren't reported by xUnit.net itself.
2. Implement selectie run (test plans).

Those features are unavailable on ARM64 due to limitations of MonoMod.Core.
Additionally, they might not work in some other rare circumstances.

Issue [#369] contains some additional details.

[#369]: https://github.com/allure-framework/allure-csharp/issues/369

## FAQ

### Why the Allure results directory is empty?

If you run your tests, but there is no Allure results directory (or it's empty),
xUnit.net may have preferred another reporter instead of `allure`.

You can force xUnit.net to select the right reporter by providing it to the
runner. For `xunit.runner.visualstudio`, it could be done with the
`xUnit.ReporterSwitch` run setting.

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
[xUnit.net 00:00:00.53] Allure reporter enabled
```

> You might need to increase the verbosity level with `--verbosity=detailed` to
> see xUnit.net's logs.

### How to run Allure.Xunit together with another reporter?

xUnit.net only uses one reporter per run. But Allure Xunit allows you to bypass
that limitation. Learn more [here](https://allurereport.org/docs/xunit-configuration/#allurexunitrunnerreporter).

### How to use Allure Xunit in a CI environment?

A CI-specific reporter might kick in addition to Allure Xunit in some CI
environments like Azure DevOps or TeamCity. The result is that both reporters
are available to run. In such cases, xUnit.net may select any of them.

  - If xUnit.net selects Allure Xunit, the tests will be reported both to the
    CI server and as Allure results.
  - If a CI-specific reporter is selected, the tests will only be reported to
    the CI server.

To fix that, make sure that [xUnit.net always picks Allure Xunit](#why-the-allure-results-directory-is-empty).
