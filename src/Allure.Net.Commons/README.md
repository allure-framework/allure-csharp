# Allure.Net.Commons

[![Nuget release](https://img.shields.io/nuget/v/Allure.Net.Commons?style=flat)](https://www.nuget.org/packages/Allure.Net.Commons)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.Net.Commons?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Net.Commons)


> Allure.Net.Commons is a library for creating custom Allure adapters for .NET test frameworks.

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

The library can be used by any project that targets a framework compatible with
.NET Standard 2.0 (.NET Framework 4.6.1+, .NET Core 2.0+, .NET 5.0+, and more).
See the complete list [here](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#select-net-standard-version).

## Note for users of Mac with Apple silicon

If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```

## Configuration

The Allure lifecycle is configured via a JSON file with the default name
`allureConfig.json`. NuGet package installs `allureConfig.Template.json`, which
you can use as an example. There are two ways to specify config file location:

  - Set the ALLURE_CONFIG environment variable to the full path of the file.
  - Add `allureConfig.json` to the project and ensure it's copied to the
    project output directory next to `Allure.Net.Commons.dll`:
    ```xml
    <ItemGroup>
      <None Update="allureConfig.json">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      </None>
    </ItemGroup>
    ```

The Allure lifecycle will start with the default configuration settings if no `allureConfig.json` is provided.

The unparsed configuration can be accessed via
`AllureLifeCycle.Instance.JsonConfiguration`. Adapters can use it to read
extended configuration properties they need. Check an example
[here](https://github.com/allure-framework/allure-csharp/blob/bf869a27828fa9b374b1e27dd972eb702be7d864/Allure.Xunit/AllureXunitConfiguration.cs#L33-L36).


The parsed configuration object can be accessed via
`AllureLifeCycle.Instance.Configuration`.

An example of the configuration file is provided below:

```json
{
  "allure": {
    "directory": "allure-results",
    "title": "custom run title",
    "links":
    [
      "https://example.org/{link}",
      "https://example.org/{issue}",
      "https://example.org/{tms}"
    ],
    "failExceptions": [
      "MyNamespace.MyAssertionException"
    ]
  }
}
```

The `directory` property defaults to `"allure-results"`.

All link pattern placeholders will be replaced with the URL value of the
corresponding link type. Given the configuration above, the following
transformation will be made:

```
link(type: "issue", url: "BUG-01") => https://example.org/BUG-01
```

`failExceptions` must be an array of strings, each representing the full name of
an exception type. If an unhandled exception occurs whose type matches one of
the provided types, the test/step/fixture is considered failed. Otherwise, it's
considered broken. An exception's type matches a name if:

  1. Its full name equals the provided name, OR
  2. One of its base classes matches the name, OR
  3. It implements an interface that matches the name.

## Attribute API

Use the attributes defined in the `Allure.Net.Commons.Attributes` namespaces:

|Attribute|Effect|Apply to|Notes|
|--|--|--|--|
|`[AllureAfter]`          |Creates a tear down fixture from the method call.                      |Method.                  |Uses AspectInjector under the hood.|
|`[AllureAttachment]`     |Creates an attachment from the method's return value.                  |Method.                  |Only supports `string` and `byte[]` return types. Uses AspectInjector under the hood.|
|`[AllureBddHierarchy]`   |Adds the `epic`, `feature`, and `story` labels to test results at once.|Method, class, interface.|This is a shorthand for `[AllureEpic]`, `AllureFeature`, and `[AllureStory]`.|
|`[AllureBefore]`         |Creates a setup fixture from the method or constructor call.           |Method, constructor.     |Uses AspectInjector under the hood.|
|`[AllureDescription]`    |Sets a description of test results.                                    |Method, class, interface.|If applied multiple times with `Append` set, all the strings are joined with `\n\n`.|
|`[AllureDescriptionHtml]`|Sets a description of test results in raw HTML.                        |Method, class, interface.|If applied multiple times with `Append` set, all the strings are joined (no separator used).|
|`[AllureEpic]`           |Adds the `epic` label to test results.                                 |Method, class, interface.|Discards the default BDD hierarchy.|
|`[AllureFeature]`        |Adds the `feature` label to test results.                              |Method, class, interface.|Discards the default BDD hierarchy.|
|`[AllureId]`             |Applies the `ALLURE_ID` label to test results.                         |Method.                  |-|
|`[AllureIssue]`          |Adds an issue link to test results                                     |Method, class, interface.|If a short ID is used instead of a full URL, a link template must exists in the config.|
|`[AllureLabel]`          |Adds a custom label to test results.                                   |Method, class, interface.|-|
|`[AllureLink]`           |Adds a link to test results.                                           |Method, class, interface.|If a short ID is used instead of a full URL, a link template must exists in the config.|
|`[AllureMeta]`           |Applies a custom set of attributes to test results.                    |Method, class, interface.|See #406.|
|`[AllureName]`           |Sets a display name of test results.                                   |Method, class, interface.|When applied to a test class, affects the value of the  default `suite` label created from this class.|
|`[AllureOwner]`          |Applies the `owner` label to test results.                             |Method, class, interface.|-|
|`[AllureParameter]`      |Affects how method arguments are converted to Allure parameters.       |Parameter.               |Allows for ignoring the parameters, see #482.|
|`[AllureParentSuite]`    |Applies the `parentSuite` label to test results.                       |Method, class, interface.|Discards the default suite hierarchy.|
|`[AllureSeverity]`       |Applies the `severity` label to test results.                          |Method, class, interface.|-|
|`[AllureStep]`           |Creates Allure steps from method calls.                                |Method.                  |Uses AspectInjector under the hood.|
|`[AllureStory]`          |Applies the `story` label to test results.                             |Method, class, interface.|Discards the default BDD hierarchy.|
|`[AllureSubSuite]`       |Applies the `subSuite` label to test results.                          |Method, class, interface.|Discards the default suite hierarchy.|
|`[AllureSuite]`          |Applies the `suite` label to test results.                             |Method, class, interface.|Discards the default suite hierarchy.|
|`[AllureSuiteHierarchy]` |Applies the `parentSuite`, `suite`, and `subSuite` labels at once.     |Method, class, interface.|This is a shorthand for `[AllureParentSuite]`, `[AllureSuite]` and `[AllureSubSuite]`.|
|`[AllureTag]`            |Applies the `tag` labels to test results.                              |Method, class, interface.|-|
|`[AllureTmsItem]`        |Applies a TMS item link to test results                                |Method, class, interface.|If a short ID is used instead of a full URL, a link template must exists in the config.|

Most of the attributes are straightforward and can be applied on a method, class, or interface:

  - When applied to a method, they affect test results produced by this method.
  - When applied to a class, they affect test results produced by all methods of this class and its subclasses.
  - When applied to an interface, they affect test results produced by all methods of classes that implement the interface.

### AOP attributes

Applying `[AllureStep]` automatically wraps method calls into Allure step. Call arguments are automatically converted to Allure parameters.

Similarly, `[AllureBefore]` and `[AllureAfter]` wrap method calls into fixtures. Apply these to
setup/teardown methods that are invoked by the framework to add fixtures to the report.

Apply `[AllureAttachment]` to a method that returns `string`, `byte[]`, `System.IO.Stream`,
or async versions of these types (e.g., `Task<string>`) to automatically attach returned values
each time the method is called.

Apply `[AllureAttachmentFile]` to a method that returns `string`, `FileInfo`, or async versions
of these types (e.g., `Task<string>`) to automatically attach files on each call. The returned
values are treated as file paths.

All four attributes mentioned above requires rely on AspectInjector. If you disable AspectInjector
(e.g., by setting the `AspectInjector_Enabled` MSBuild property to `false`), the attributes won't
do anything.

### Controlling parameters

There are two contexts where Allure automatically converts method arguments into Allure parameters:

  - When creating a test result.
  - When creating a step result.

In both contexts, `[AllureParameter]` can be used to affect the conversion:

  - Set `Ignored` to skip the argument.
  - Use `Name` to explicitly name the parameter.
  - Use `Mode` to mask or hide the value (the original value will still exist in the result files).
  - Set `Excluded` to ignore the parameter when identifying the test that corresponds to the test
    result (see [here](https://allurereport.org/docs/history-and-retries/#common-pitfall-a-test-s-retries-are-displayed-as-separate-tests)).

Example:

```csharp
using Allure.Net.Commons;

public class MyTests
{
    public void MyParameterizedTestMethod(
        [AllureParameter(Ignore = true)] IUserService users,
        [AllureParameter(Name = "User")] string userName,
        [AllureParameter(Mode = ParameterMode.Masked)] string password,
        [AllureParameter(Excluded = true)] DateTime timestamp
    )
    {
        // ...
    }
}
```

### Attribute composition

You can compose multiple attributes in two ways.

#### Composition via an interface

Apply the attributes to a new interface and apply the interface to the classes the attributes
should affect:

```csharp
using Allure.Net.Commons.Attributes;

[AllureEpic("My epic")]
[AllureFeature("My feature")]
[AllureIssue("125")]
[AllureTag("acceptance")]
public interface IMyFeatureTests { }

public class MyTestClass : IMyFeatureTests
{
    // tests
}

public class AnotherTestClass : IMyFeatureTests
{
    // more tests
}
```

#### Composition via `[AllureMeta]`

Define a new attribute class that derives from `AllureMetaAttribute` and apply the attributes
to this class. Use the new class to apply all the attributes to test classes and methods:

```csharp
using System;
using Allure.Net.Commons.Attributes;

[AllureEpic("My epic")]
[AllureFeature("My feature")]
[AllureIssue("125")]
[AllureTag("acceptance")]
public class MyFeatureAttribute : AllureMetaAttribute { }

[MyFeature]
public class MyTestClass : IMyFeatureTests
{
    // tests
}

public class AnotherTestClass : IMyFeatureTests
{
    [Test]
    [MyFeature]
    public void MyTestMethod()
    {
        // test body
    }
}
```

### Descriptions

If `[AllureDescription]` is applied multiple times on different levels, the most specific one wins,
unless you set `Append`. In such a case, all strings are joined with `\n\n` creating separate
markdown paragraphs:

```csharp
[AllureDescription("Paragraph 1", Append = true)]
class Base { }

[AllureDescription("Paragraph 2", Append = true)]
class TestClass : Base
{
    [Test]
    [AllureDescription("Paragraph 3", Append = true)] // Without Append, the description will be just "Paragraph 3"
    public void MyTest()
    {
        // test body
    }
}
```

For `[AllureDescriptionHtml]`, the behavior is almost the same. The only difference is that no
separator is used to join multiple strings. Use block elements to keep the descriptions separate.

## Runtime API

Use this API to enhance the report at runtime.

### The AllureApi facade

Use the `Allure.Net.Commons.AllureApi` class to access the most commonly used
functions.

#### Metadata

* `SetTestName`
* `SetFixtureName`
* `SetStepName`
* `SetDescription`
* `SetDescriptionHtml`
* `AddLabels`
* `AddLabel`
* `SetSeverity`
* `SetOwner`
* `SetAllureId`
* `AddTags`
* `AddLinks`
* `AddLink`
* `AddIssue`
* `AddTmsItem`

#### Hierrarchies

* `AddParentSuite`
* `AddSuite`
* `AddSubSuite`
* `AddEpic`
* `AddFeature`
* `AddStory`

#### Lambda steps

* `Step(string, Action): void` - step action.
* `Step<T>(string, Func<T>): T` - step function.
* `Step(string, Func<Task>): Task` - async step action.
* `Step<T>(string, Func<Task<T>>): T` - async step function.

#### Noop step

* `Step(string)`

#### Attachments

* AddAttachment - adds an attachment to the current step, fixture, or test.
* AddScreenDiff - adds needed artifacts to the current test case to be used with [screen-diff-plugin](https://github.com/allure-framework/allure2/tree/main/plugins/screen-diff-plugin)

### The ExtendedApi facade

Use this class to access some less commonly used functions.

#### Explicit step management

> [!NOTE]
> Use the functions below only if lambda steps don't suit your needs.

* `StartStep(string): void`
* `StartStep(string, Action<StepResult>): void`
* `PassStep(): void`
* `PassStep(Action<StepResult>): void`
* `FailStep(): void`
* `FailStep(Action<StepResult>): void`
* `BreakStep(): void`
* `BreakStep(Action<StepResult>): void`


#### Lambda fixtures

* `Before(string, Action): void` - setup fixture action.
* `Before<T>(string, Func<T>): T` - setup fixture function.
* `Before(string, Func<Task>): Task` - async setup fixture action.
* `Before<T>(string, Func<Task<T>>): T` - async setup fixture function.
* `After(string, Action): void` - teardown fixture action.
* `After<T>(string, Func<T>): T` - teardown fixture function.
* `After(string, Func<Task>): Task` - async teardown fixture action.
* `After<T>(string, Func<Task<T>>): T` - async teardown fixture function.

#### Explicit fixture management

> [!NOTE]
> Use the functions below only if lambda fixtures don't suit your needs.

* `StartBeforeFixture(string): void`
* `StartAfterFixture(string): void`
* `PassFixture(): void`
* `PassFixture(Action<FixtureResult>): void`
* `FailFixture(): void`
* `FailFixture(Action<FixtureResult>): void`
* `BreakFixture(): void`
* `BreakFixture(Action<FixtureResult>): void`


## The integration API

This API is designed for adapter or library authors. You may still use it as a
test author, but we recommend considering the Attribute/Runtime API first.

### AllureLifecycle

The [AllureLifecycle](https://github.com/allure-framework/allure-csharp/blob/main/src/Allure.Net.Commons/AllureLifecycle.cs)
class provides methods to manipulate the Allure context while responding to the
test framework's events. Use `AllureLifecycle.Instance` property to access it.

#### Fixture context control

* StartBeforeFixture
* StartAfterFixture
* UpdateFixture
* StopFixture

#### Test context control

* ScheduleTestCase
* StartTestCase
* UpdateTestCase
* StopTestCase
* WriteTestCase

#### Step context control

* StartStep
* UpdateStep
* StopStep

#### Utility Methods

* CleanupResultDirectory - can be used in test run setup to clean old result files

#### Context capturing

The methods above operate on the current Allure context. This context
flows naturally as a part of ExecutionContext and is subject to the same
constraints. Notably, changes made in an async callee can't be observed
by the caller.

Use the following methods of `AllureLifecycle` to capture the current Allure
context and to operate on the captured context later:

* Context
* RunInContext

Example:

```csharp
public static async Task Caller(ScenarioContext scenario)
{
    await Callee(scenario);
    AllureLifecycle.Instance.RunInContext(
        scenario.Get<AllureContext>(), // Get the previously captured context
        () =>
        {
            // The test context required by the below methods wouldn't be
            // visible if they weren't wrapped with RunInContext.
            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();
        }
    );
}

public static async Task Callee(ScenarioContext scenario)
{
    AllureLifecycle.Instance.StartTestCase(
        new(){ uuid = Guid.NewGuid().ToString() }
    );

    // Capture the context in an object of the test framework's object model
    scenario.Set(AllureLifecycle.Instance.Context);
}
```
