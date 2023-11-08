# Allure.XUnit

[![Nuget](https://img.shields.io/nuget/v/Allure.XUnit)](https://www.nuget.org/packages/Allure.XUnit/)

Allure.XUnit helps you to create Allure reports from your xUnit tests.

Allure.XUnit supports .NET Core 2.0 or later, and any .NET runtime compatible
with .NET Standard 2.1.

## How to run

Install the Allure.XUnit package and run the tests normally. In many cases
allure should start automatically. The result files are created in the
`allure-results` directory in the target directory.
If that didn't happen, check out the `Running tests in a CI pipeline` section.

### Configuration

If you need to configure allure-xunit, create `allureConfig.json` and make sure
it is copied to the output directory:

  - In Visual Studio select the file and set the following properties:
      - Build Action: Content
      - Copy to Output Directory: Always/Copy if newer
    OR
  - In the `.csproj` file make sure the following entry exists:
    ```xml
    <ItemGroup>
        <Content Include="allureConfig.json">
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </Content>
    </ItemGroup>
    ```

An example of the config:

```json
{
    "allure": {
        "allure-directory": "allure-results",
        "links": [
            "https://github.com/allure-framework/allure-csharp/issues/{issue}"
        ]
    }
}
```

### Running tests in a CI pipeline

There might be a chance another xunit reporter kicks in disabling allure. This
typically happens in a CI pipeline when a pipeline-specific reporter detects
the CI environment and signals xunit that it's ready to be used. Xunit's
reporter pick is unreliable in such a case.

If that happens, you should explicitly tell xunit to use the `allure` reporter.
How exactly - depends on what runner you use. For the most popular one -
`xunit.runner.visualstudio` - you either specify it via the CLI argument:

```
dotnet test -- RunConfiguration.ReporterSwitch=allure
```

Or use a `.runsettings` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <ReporterSwitch>allure</ReporterSwitch>
  </RunConfiguration>
</RunSettings>
```

Now pass the file to `dotnet test`:

```
dotnet test -s <path-to-.runsettings-file>
```

Or specify its path using your IDE.

By default Allure.XUnit tries to use additional environmentally enabled
reporter (if any), so your tests are reported both to allure and, say,
TeamCity. Due to how xunit configuration works, that behavior occures
regardless of xunit's `NoAutoReporters` setting. If you want to disable this
behavior, check out the `Configuring a secondary reporter` section.

### Configuring a secondary reporter

Use the `xunitRunnerReporter` property of `allureConfig.json` to control
an additional reporter used by Allure.XUnit:

```json
{
    "allure": {
        "xunitRunnerReporter": "auto|none|<switch>|<AQ-classname>"
    }
}
```

We support the following values:
  - `auto` - default value. Allure.XUnit uses the first environmentally
    enabled reporter it finds.
  - `none` - use this value to disable an additional reporter.
  - `<switch>` - Allure.XUnit looks for the reporter with the specified
    runner switch. If such reporter exists, it's used. Otherwise, the error is
    thrown.
  - `<AQ-classname>` - the assembly qualified class name is used to find the
    reporter's class. If no such class can be found, the error is thrown.


## Attributes:

The following attributes are at your disposal:

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
There is no more need to use separate Allure.XUnit.StepExtensions package. You
should remove it from dependencies and use attributes from
[Allure.XUnit.Attributes.Steps namespace](Attributes/Steps) directly.

```c#
using Allure.XUnit.Attributes.Steps;

...

    [AllureStep("Check that {someNumber} is the answer to life, the universe, and everything")]
    private void NestedStep([Name("number")] int someNumber, [Skip] bool skippedBoolean = true)
    {
        Assert.Equal(42, someNumber);
    }
```

### Allure.Xunit.Steps deprecation

The new `Allure.Net.Commons.AllureApi` facade class was designed specificially
for test authors to enhance the Allure report. Prefer using functions in this
class over the ones from `Allure.Xunit.Steps`.

## Attachments
Use the [`Attachments`](Attachments.cs) class with its methods as well as
attachment methods from the `Allure.Net.Commons.AllureApi` class.

## Runtime API
Use the functions from `Allure.Net.Commons.AllureApi` to enhance the report at
runtime.

## Known issues and limitations

### Rosetta is required for users on Mac with Apple silicon
If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```

### Arguments of some theories might be unreported

Under rare circumstances arguments of some theories might be missing in the
report produced by allure-xunit. Issue [#369] contains some additional details.

If you are affected by this, you may switch to the `Debug` configuration as a
workaround until we come up with a solution.

### Selective run might not work

Under rare circumstances selective run feature might not work.
Issue [#369] contains some additional details.

If you are affected by this, you may try to switch to the `Debug` configuration
as a workaround until we come up with a solution.

## Examples

See [Examples](../Allure.XUnit.Examples).

[#369]: https://github.com/allure-framework/allure-csharp/issues/369
[#381]: https://github.com/allure-framework/allure-csharp/issues/381
