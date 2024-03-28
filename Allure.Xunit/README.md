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

Install the Allure.Xunit package and run the tests normally. In many cases
allure should start automatically. The result files are created in the
`allure-results` directory in the target directory.
If that didn't happen, check out the `Running tests in a CI pipeline` section.

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
