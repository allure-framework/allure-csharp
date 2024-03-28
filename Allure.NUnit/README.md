# Allure NUnit adapter

[![Nuget](https://img.shields.io/nuget/v/Allure.NUnit?style=flat)](https://www.nuget.org/packages/Allure.NUnit)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.Nunit?style=flat)](https://www.nuget.org/packages/Allure.NUnit)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.nunit?label=downloads&style=flat)

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

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/Allure.NUnit.Examples).

## Notes

### Namespace changed to Allure.NUnit

Starting from 2.12.0, the namespace `NUnit.Allure` is deprecated. The API in
that namespace still works, but it will be removed in the future. Please, use
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
