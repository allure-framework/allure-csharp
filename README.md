# Allure c# Integrations   ![](https://bakanych.visualstudio.com/_apis/public/build/definitions/60930d4b-6231-4f3e-a77f-40e16c32a13d/3/badge)

## Allure.Commons  [![](http://img.shields.io/nuget/vpre/Allure.Commons.svg?style=flat)](https://www.nuget.org/packages/Allure.Commons)
.Net implementation of [Allure java-commons](https://github.com/allure-framework/allure-java/tree/master/allure-java-commons).

Can be targeted either by .net 4.6.* or .net standard 2.* projects.

Use this library to create custom Allure adapters for .Net test frameworks.

### Configuration
Allure lifecycle is configured via json file with default name `allureConfig.json`. NuGet package installs `allureConfig.Template.json` which you can use as a template. There are 2 ways to specify config file location: 
-  set ALLURE_CONFIG environment variable to the full path of json config file. This option is preferable for .net core projects which utilize nuget libraries directly from nuget packages folder. See this example of setting it via code: https://github.com/allure-framework/allure-csharp/blob/bdf11bd3e1f41fd1e4a8fd22fa465b90b68e9d3f/Allure.Commons.NetCore.Tests/AllureConfigTests.cs#L13-L15

- place `allureConfig.json` to the location of `Allure.Commons.dll`. This option can be used with .net classic projects which copy all referenced package libraries into binary folder. Do not forget to set 'Copy to Output Directory' property to 'Copy always' or 'Copy if newer' in your test project or set it in .csproj:
```
<ItemGroup>
<None Update="allureConfig.json">
<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
</ItemGroup>
```

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
[AllureLifecycle](https://github.com/allure-framework/allure-csharp/blob/master/Allure.Commons/AllureLifecycle.cs) class provides methods for test engine events processing.

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



## SpecFlow Adapter  [![](http://img.shields.io/nuget/vpre/SpecFlow.Allure.svg?style=flat)](https://www.nuget.org/packages/SpecFlow.Allure) [![NuGet](https://img.shields.io/nuget/dt/SpecFlow.Allure.svg)](https://www.nuget.org/packages/SpecFlow.Allure)
Currently supports [SpecFlow](http://specflow.org/) v2.1 - 2.4.
Please use corresponding NuGet package version.
### Installation
Make sure your test project targets .net 4.6.1 or higher.

Install the latest version of [Specflow.Allure](https://www.nuget.org/packages/SpecFlow.Allure) nuget package.
### Configuration
The plugin uses Allure.Commons json configuration extended with custom sections.
### Custom host name
In case if you want to customize host name which is displayed in Allure Timeline section, please configure `allure.title` property in json configuraion file.
#### If you use NUnit
Default value for allure.directory in allureConfig.json is "allure-results", default working directory in NUnit 3.* is the working directory of console runner. If you don't want to place allure results into NUnit default working folder please either set absolute path in allure.config or set working directory for NUnit in your test setup, e.g.:
``` csharp
[OneTimeSetUp]
public void Init()
{
   Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
}
```
### Usage
Just run your SpecFlow scenarios and find `allure-results` folder ready to generate Allure report.

### Features
#### Grouping
You can structure your scenarios in 3 Allure hierarchies using feature and scenario tags. 
Please read [report structure](https://docs.qameta.io/allure/latest/#_report_structure) Allure documentation section for additional details. Hierarchies consist of the following levels:

**Suites**
* Parent Suite
* * Suite
* * * Sub Suite

**Behaviors**
* Epic
* * Feature
* * * Story

**Packages**
* Package
* * Class
* * * Method

The plugin uses `allure:grouping` configuration section to parse tags with the regular expression. If the expression contains the group, it will be used as hierarchy level name otherwise entire match will be used. E.g:

`^mytag.*` : any tags starting with `@mytag` will be used for grouping.

`^type:(ui|api)` : `@ui` or `@api` tags from regex pattern will be used for grouping.

Check this [config example](https://github.com/allure-framework/allure-csharp/blob/master/Tests.SpecRun/allureConfig.json) as a starting point.

#### Links
You can add links to your scenarios using tags. Tag and link patterns can be configured in `allureConfig.json`
``` json
{
  "allure": {
    "links": [
      "https://myissuetracker.org/{issue}",
      "https://mytestmanagementsystem.org?test={tms}"
    ]
  },
  "specflow": {
    "links": {
      "link": "^link:(.+)",
      "issue": "^\\d+",
      "tms": "^tms:(\\d+)"
    }
  }
}
```
The following scenario
``` cucumber
@123456 @tms:42 @link:http://example.org 
Scenario: I do like click on links in Allure report 
```
will have three links in Allure report:
[123456](https://myissuetracker.org/123456), [42](https://mytestmanagementsystem.org?test=tms-42) and http://example.org. 

In case there are links, which are generated during tests, then they can be added in code via AllureLifecycle:
``` c#
AllureLifecycle.UpdateTestCase(testResult.uuid, tc =>
            {
                tc.links.Add(new Link()
                {
                    name = "Example link",
                    url = "http://example.com"
                });
            });
```
This will show for scenario link with Text: Example link; and url: "http://example.com".

#### Severity
You can add Severity for your scenarios using tags. It can be configured in `allureConfig.json`
``` json
 "labels": {
      "severity": "^(normal|blocker|critical|minor|trivial)"
    },
```
The following scenario
``` cucumber
@critical
Scenario: ....
```
will set current scenario severity in Allure report as Blocker

#### Tables conversion
Table arguments in SpecFlow steps can be converted either to step csv-attacments or step parameters in the Allure report. The conversion is configurable in `specflow:stepArguments` config section.
With `specflow:stepArguments:convertToParameters` set to `true` the following table arguments will be represented as parameters:
* one row tables
* two column tables with the headers matching both `specflow:stepArguments:paramNameRegex` and `specflow:stepArguments:paramValueRegex` regular expressions.

<table>
<th>SpecFlow</th>
<th>Allure</th>
<tr>
<td>

![](https://github.com/allure-framework/allure-csharp/blob/master/img/wiki-step-all.PNG)

</td>
<td>

![](https://github.com/allure-framework/allure-csharp/blob/master/img/allure-step-all.PNG)

</td>
</tr>
</table>

#### Attachments
You can add attachments to an Allure report from your step bindings:
```csharp
using Allure.Commons;
...
AllureLifecycle.AddAttachment(path, "Attachment Title");
// or
AllureLifecycle.AddAttachment("Attachment Title", "application/txt", "path");
// where "application/txt" - type of your attachment
```
