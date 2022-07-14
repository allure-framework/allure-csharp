## Preview build
**Changes**
- AllureStep attribute reworked
- AllureBefore attribute should work as expected
- AllureAfter attribute should work as expected
- Steps methods should be reworked either deprecated soon (Let's discuss!)
- AllureAttachments marked as Obsolete - use Attachments instead

# Allure.XUnit

[![Nuget](https://img.shields.io/nuget/v/Allure.XUnit)](https://www.nuget.org/packages/Allure.XUnit/)

Allure.XUnit is library for display xunit tests in Allure report.

Allure.XUnit supports .NET Core 2.0 and later.

## Attributes:

* AllureXunit
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
* AllureXunitTheory - attribute for display parametrized tests. Use ```InlineData```, ```MemberData```, ```ClassData```,
  XUnit attributes for pass parameters.

All methods have to be tagged by attribute AllureXunit instead of Fact, or AllureXunitTheory instead of Theory for
display in allure report. Other attributes are optional.

### Attributes usage

Most of the attributes can be used both on methods and classes.

| Attribute | Method | Class |
|:------------------|:---:|:---:|
| AllureXunit       |  x  |     |
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
| AllureXunitTheory |  x  |     |
| AllureId          |  x  |     |

To override attribute value you can use `overwrite` param in attribute definition.
In other case multiple values will be written in test results.

Example:
```c#
[AllureSuite("Suite A")]
public class TestClass
{
    [AllureXunit(DisplayName = "Test Name")]
    [AllureSuite("Suite B", overwrite: true))]
    public void TestMethod
    {
    }
}
```

## Steps
Use `AllureStepAttribute`, `AllureBeforeAttribute`, `AllureAfterAttribute`

See [Examples](../Allure.XUnit.Examples/ExampleStepAttributes.cs).

### Allure.XUnit.StepExtensions deprecation
There is no more need to use separate Allure.XUnit.StepExtensions package - you can simple remove it from dependencies and use attributes from [Allure.XUnit.Attributes.Steps namespace](Attributes/Steps) directly.

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
Use [`AllureAttachments`](AllureAttachments.cs) class with it's methods. (AttachmentAttribute coming soon)

## Running

Just run `dotnet test`.

`allure-results` directory with result appears after running tests in target directory.

## Examples

See [Examples](../Allure.XUnit.Examples)
