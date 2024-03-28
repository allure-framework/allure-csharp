# Allure.Xunit

[![Nuget](https://img.shields.io/nuget/v/Allure.Xunit?style=flat)](https://www.nuget.org/packages/Allure.Xunit)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.Xunit?style=flat)](https://www.nuget.org/packages/Allure.Xunit)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.xunit?label=downloads&style=flat)

> An Allure adapter for [xUnit.net](https://xunit.net/).

Allure Xunit supports .NET Core 2.0 or later, or any .NET runtime that
implements .NET Standard 2.1.

## Quick start

Install the Allure.Xunit package and run the tests normally. In many cases
allure should start automatically. The result files are created in the
`allure-results` directory in the target directory.
If that didn't happen, check out the `Running tests in a CI pipeline` section.

## Further readings

Learn more from [the documentation for Allure Xunit](https://allurereport.org/docs/xunit/).

Some examples are available [here](https://github.com/allure-framework/allure-csharp/tree/main/Allure.Xunit.Examples).

## Notes

### Allure.Xunit.StepExtensions deprecation
There is no more need to use separate Allure.XUnit.StepExtensions package. You
should uninstall it and use attributes from
[Allure.Xunit.Attributes.Steps namespace](Attributes/Steps) directly.

### Namespace change
Previously, the package used a mix of `Allure.Xunit` and `Allure.XUnit`
namespaces. Starting from 2.12.0, you should only use `Allure.Xunit`. Some parts
of the public API are still accessible through the old namespace, but that
access is deprecated now and will be removed in the future.

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
