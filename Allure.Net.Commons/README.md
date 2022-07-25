## Allure.Net.Commons  [![](http://img.shields.io/nuget/vpre/Allure.Net.Commons.svg?style=flat)](https://www.nuget.org/packages/Allure.Net.Commons)
.Net implementation of [Allure java-commons](https://github.com/allure-framework/allure-java/tree/master/allure-java-commons).

Can be targeted either by legacy .net 4.5+ or .net standard 2.* projects.

Use this library to create custom Allure adapters for .Net test frameworks.

### Configuration
Allure lifecycle is configured via json file with default name `allureConfig.json`. NuGet package installs `allureConfig.Template.json` which you can use as an example. There are 2 ways to specify config file location:
-  set ALLURE_CONFIG environment variable to the full path of json config file. This option is preferable for .net core projects which utilize nuget libraries directly from nuget packages folder. See this example of setting it via code: https://github.com/allure-framework/allure-csharp/blob/bdf11bd3e1f41fd1e4a8fd22fa465b90b68e9d3f/Allure.Commons.NetCore.Tests/AllureConfigTests.cs#L13-L15

- place `allureConfig.json` to the location of `Allure.Commons.dll`. This option can be used with .net classic projects which copy all referenced package libraries into binary folder. Do not forget to set 'Copy to Output Directory' property to 'Copy always' or 'Copy if newer' in your test project or set it in .csproj:
```
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
All
Link pattern placeholders will be replaced with URL value of corresponding link type, e.g.

`link(type: "issue", url: "BUG-01") => https://example.org/BUG-01`

### AllureLifecycle
[AllureLifecycle](https://github.com/allure-framework/allure-csharp/blob/main/Allure.Commons/AllureLifecycle.cs) class provides methods for test engine events processing.

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

### Step Events
* StartStep
* UpdateStep
* StopStep

#### Attachment Events
* AddAttachment - adds attachment to the current lifecycle executable item
* AddScreenDiff - adds needed artifacts to the test case with given uuid to be used with [screen-diff-plugin](https://github.com/allure-framework/allure2/tree/master/plugins/screen-diff-plugin)

#### Utility Methods
* CleanupResultDirectory - can be used in test run setup to clean old result files

### Troubleshooting
...
