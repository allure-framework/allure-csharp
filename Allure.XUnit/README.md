# Allure.XUnit

[![Nuget](https://img.shields.io/nuget/v/Allure.XUnit?style=flat)](https://www.nuget.org/packages/Allure.XUnit)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.XUnit?style=flat)](https://www.nuget.org/packages/Allure.XUnit)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.xunit?label=downloads&style=flat)

> An Allure adapter for [xUnit.net](https://xunit.net/).

Allure.XUnit supports .NET Core 2.0 or later, or any .NET runtime that
implements .NET Standard 2.1.

## Quick start

Install the Allure.XUnit package and run the tests normally. In many cases
allure should start automatically. The result files are created in the
`allure-results` directory in the target directory.
If that didn't happen, check out the `Running tests in a CI pipeline` section.

## Further readings

Learn more from [the documentation for Allure NUnit](https://allurereport.org/docs/nunit/).

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/Allure.XUnit.Examples).

## Notes

### AllureXunit and AllureXunitTheory deprecation
Previously all test methods had to be marked with the AllureXunit or
AllureXunitTheory attributes. There is no such need anymore.
These attributes are still supported, but we advice you to use the built-in
Fact and Theory attributes instead.

The AllureXunit and AllureXunitTheory attributes might be removed in future releases.

### Allure.XUnit.StepExtensions deprecation
There is no more need to use separate Allure.XUnit.StepExtensions package. You
should remove it from dependencies and use attributes from
[Allure.XUnit.Attributes.Steps namespace](Attributes/Steps) directly.

### Allure.Xunit.Steps deprecation

The new `Allure.Net.Commons.AllureApi` facade class was designed specificially
for test authors to enhance the Allure report. Prefer using functions in this
class over the ones from `Allure.Xunit.Steps`.

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

1. Report arguments of theories in rare case they aren't reported by xUnit.net itself.
2. Implement selectie run.

Sometimes MonoMod.Core fails to patch making those features unavailable. We
know two such cases:

1. Running under .NET 8.0
2. Running on ARM64

Issue [#369] contains some additional details.

[#369]: https://github.com/allure-framework/allure-csharp/issues/369
