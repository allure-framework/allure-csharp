# Allure.XUnit

[![Nuget](https://img.shields.io/nuget/v/Allure.XUnit)](https://www.nuget.org/packages/Allure.XUnit/)

Allure.XUnit is library for display xunit tests in Allure report.

Allure.XUnit supports .NET Core 2.0 and later.

## Attributes:

* AllureDescription
* AllureParentSuite
* AllureFeature
* AllureTag
* AllureSeverity
* AllureIssue
* AllureOwner
* AllureSuite
* AllureSubSuite
* AllureLink
* AllureEpic
* AllureLabel
* AllureId

All attributes are optional.

### Attributes usage

Most of the attributes can be used both on methods and classes.

| Attribute | Method | Class |
|:------------------|:---:|:---:|
| AllureDescription |  x  |     |
| AllureParentSuite |  x  |  x  |
| AllureFeature     |  x  |  x  |
| AllureTag         |  x  |  x  |
| AllureSeverity    |  x  |  x  |
| AllureIssue       |  x  |  x  |
| AllureOwner       |  x  |  x  |
| AllureSuite       |  x  |  x  |
| AllureSubSuite    |  x  |  x  |
| AllureLink        |  x  |  x  |
| AllureEpic        |  x  |  x  |
| AllureLabel       |  x  |  x  |
| AllureId          |  x  |     |

To override attribute value you can use `overwrite` param in attribute definition.
In other case multiple values will be written in test results.

Example:
```c#
[AllureSuite("Suite A")]
public class TestClass
{
    [Fact(DisplayName = "Test Name")]
    [AllureSuite("Suite B", overwrite: true))]
    public void TestMethod
    {
    }
}
```

### AllureXunit and AllureXunitTheory deprecation
Previously all test methods had to be marked with the AllureXunit or 
AllureXunitTheory attributes. There is no such need anymore.
These attributes are still supported, but we advice you to use the built-in
Fact and Theory attributes instead.

The AllureXunit and AllureXunitTheory attributes might be removed in future releases.

## Steps
Use `AllureStepAttribute`, `AllureBeforeAttribute`, `AllureAfterAttribute`

See [Examples](../Allure.XUnit.Examples/ExampleStepAttributes.cs).

### Allure.XUnit.StepExtensions deprecation
There is no more need to use separate Allure.XUnit.StepExtensions package - you can simply remove it from dependencies and use attributes from [Allure.XUnit.Attributes.Steps namespace](Attributes/Steps) directly.

```c#
using Allure.XUnit.Attributes.Steps;

...

    [AllureStep("Check that {someNumber} is the answer to life, the universe, and everything")]
    private void NestedStep([Name("number")] int someNumber, [Skip] bool skippedBoolean = true)
    {
        Assert.Equal(42, someNumber);
    }
```

## Attachments
Use [`AllureAttachments`](AllureAttachments.cs) class with its methods. (AttachmentAttribute coming soon)

## Running

Just run `dotnet test`.

`allure-results` directory with result appears after running tests in target directory.

## Known issues

### Incompatibility with other runner reporters (#368)

Allure-xunit is implemented as an xunit runner reporter, hence it's incompatible
with other runner reporters such as `teamcity`, `json` or `verbose` (from
[xunit.runner.reporters]). Only one reporter could be active at a time.

If you have other reporter active but want to use allure-xunit, you have to
manually enable the `allure` reporter by running `dotnet test` with the
`RunConfiguration.ReporterSwitch` run setting set to `allure`:

```shell
dotnet test -- RunConfiguration.ReporterSwitch=allure
```

Alternatively, you may add this setting to your `.runsettings` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <ReporterSwitch>allure</ReporterSwitch>
  </<RunConfiguration>
</RunSettings>
```

### Arguments of some theories might be unreported

Under rare circumstances arguments of some theories might be missing in the
report produced by allure-xunit. #369 contains some additional details.

If you are affected by this, you may switch to the `Debug` configuration as a
workaround until we come up with a solution.

## Examples

See [Examples](../Allure.XUnit.Examples).

[xunit.runner.reporters]: https://www.nuget.org/packages/xunit.runner.reporters/
