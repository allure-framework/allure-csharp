# Allure.Net.Commons

[![](http://img.shields.io/nuget/vpre/Allure.Net.Commons.svg?style=flat)](https://www.nuget.org/packages/Allure.Net.Commons)
.Net implementation of [Allure java-commons](https://github.com/allure-framework/allure-java/tree/master/allure-java-commons).

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
* `SetDescriptionHtml`
* `AddLabels`
* `AddLabel`
* `SetLabel`
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
* `SetParentSuite`
* `AddSuite`
* `SetSuite`
* `AddSubSuite`
* `SetSubSuite`
* `AddEpic`
* `SetEpic`
* `AddFeature`
* `SetFeature`
* `AddStory`
* `SetStory`

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
test author, but we recommend considering the Runtime API first.

### AllureLifecycle

The [AllureLifecycle](https://github.com/allure-framework/allure-csharp/blob/main/Allure.Net.Commons/AllureLifecycle.cs)
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
