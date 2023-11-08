# Allure.Net.Commons  [![](http://img.shields.io/nuget/vpre/Allure.Net.Commons.svg?style=flat)](https://www.nuget.org/packages/Allure.Net.Commons)
.Net implementation of [Allure java-commons](https://github.com/allure-framework/allure-java/tree/master/allure-java-commons).

Use this library to create custom Allure adapters for .NET test frameworks.

Can be used by any project targeting a netstandard2.0 compatible framework
including .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5.0+ and more. See the
full list [here](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#select-net-standard-version).

## Note for users of Mac with Apple silicon

If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```

## Configuration
Allure lifecycle is configured via json file with default name `allureConfig.json`. NuGet package installs `allureConfig.Template.json` which you can use as an example. There are 2 ways to specify config file location:
-  set ALLURE_CONFIG environment variable to the full path of json config file. This option is preferable for .net core projects which utilize nuget libraries directly from nuget packages folder. See this example of setting it via code: https://github.com/allure-framework/allure-csharp/blob/bdf11bd3e1f41fd1e4a8fd22fa465b90b68e9d3f/Allure.Commons.NetCore.Tests/AllureConfigTests.cs#L13-L15

- place `allureConfig.json` to the location of `Allure.Commons.dll`. This option can be used with .net classic projects which copy all referenced package libraries into binary folder. Do not forget to set 'Copy to Output Directory' property to 'Copy always' or 'Copy if newer' in your test project or set it in .csproj:
   ```xml
   <ItemGroup>
     <None Update="allureConfig.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
     </None>
   </ItemGroup>
   ```

Allure lifecycle will start with default configuration settings if `allureConfig.json` is not found.

Raw json configuration can be accessed from `AllureLifeCycle.Instance.JsonConfiguration` to extend configuration by adapters. See extension example here: https://github.com/allure-framework/allure-csharp/blob/bdf11bd3e1f41fd1e4a8fd22fa465b90b68e9d3f/Allure.SpecFlowPlugin/PluginHelper.cs#L20-L29


Base configuration params are stored in `AllureLifeCycle.Instance.Configuration`

Allure configuration section is used to setup output directory and link patterns, e.g.:
```
{
  "allure": {
    "directory": "allure-results", // optional, default value is "allure-results"
    "title": "custom run title", // optional
    "links": //optional 
    [
      "https://example.org/{link}",
      "https://example.org/{issue}",
      "https://example.org/{tms}"
    ]
  }
}
```

All link pattern placeholders will be replaced with URL value of corresponding link type, e.g.

`link(type: "issue", url: "BUG-01") => https://example.org/BUG-01`

## The end user API

This API is designed for test authors. Use it to enhance the report created
from your tests.

### The AllureApi facade

Use the `Allure.Net.Commons.AllureApi` class to access an end user's runtime
API.

#### Lambda steps

* `Step(string, Action): void` - step action.
* `Step<T>(string, Func<T>): T` - step function.
* `Step(string, Func<Task>): Task` - async step action.
* `Step<T>(string, Func<Task<T>>): T` - async step function.

#### Noop step
* `Step(string)`

#### Attachments
* AddAttachment - adds an attachment to the current step, fixture, or test.
* AddScreenDiff - adds needed artifacts to the current test case to be used with [screen-diff-plugin](https://github.com/allure-framework/allure2/tree/master/plugins/screen-diff-plugin)

#### Explicit step management

> [!NOTE]
> Use these functions only if lambda steps doesn't suit your needs.

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
> Use these functions only if lambda fixtures doesn't suit your needs.

* `StartBeforeFixture(string): void`
* `StartAfterFixture(string): void`
* `PassFixture(): void`
* `PassFixture(Action<FixtureResult>): void`
* `FailFixture(): void`
* `FailFixture(Action<FixtureResult>): void`
* `BreakFixture(): void`
* `BreakFixture(Action<FixtureResult>): void`


## The integration API

This API is designed for those who want to integrate Allure with a test
framework or library. You may still use it if you just want to improve the
report created from the tests you write, but we strongly recommend you to
consider using the end user API first.

### AllureLifecycle
[AllureLifecycle](https://github.com/allure-framework/allure-csharp/blob/main/Allure.Commons/AllureLifecycle.cs)
class provides methods for test engine events processing.

Use `AllureLifecycle.Instance` property to access.

#### Fixture Events
* StartBeforeFixture
* StartAfterFixture
* UpdateFixture
* StopFixture

#### Testcase Events
* StartTestCase
* UpdateTestCase
* StopTestCase
* WriteTestCase

#### Step Events
* StartStep
* UpdateStep
* StopStep

#### Utility Methods
* CleanupResultDirectory - can be used in test run setup to clean old result files

#### Context capturing
The methods above operate on the current Allure context. This context
flows naturally as a part of ExecutionContext and is subject to the same
constraints. Particularly, changes made in an async callee can't be observed
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
        scenario.Get<AllureContext>(),
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

    // Pass Allure context to the caller via ScenarioContext
    scenario.Set(AllureLifecycle.Instance.Context);
}
```

#### Obsoleted methods
Methods with explicit uuid parameters are deprecated. Migrate to their
uuid-less counterparts that operate on the current Allure context.

## Troubleshooting
...
