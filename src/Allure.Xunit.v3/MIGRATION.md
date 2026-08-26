# Migrate from Allure.Xunit to Allure.Xunit.v3

`Allure.Xunit` integrates Allure Report with xUnit.net v2 and is built on top
of `Allure.Net.Commons`. `Allure.Xunit.v3` is a different adapter for xUnit.net
v3 projects running on Microsoft Testing Platform. It's built on top of
`Allure.Net`, a new package that exposes a new API for test authors. Migrating
therefore involves both the xUnit.net upgrade and changes to the public Allure
API.

`Allure.Net` includes a compatibility API layer residing in the
`Allure.Net.Commons` namespace to assist in migration. The
intent is to keep some existing code compiling while it is migrated.
It's not intended as a permanent alternative to the new API. Remove all uses
of the `Allure.Net.Commons` namespace before considering the migration complete.

## Before you start

Commit or otherwise save your current code, then run the existing test suite and
keep a representative set of `allure-results`. Comparing the old and new results
helps catch reporting changes that compilation and test assertions cannot find.

This guide covers the Allure-specific changes. Follow the
[xUnit.net v3 migration guidance](https://xunit.net/docs/getting-started/v3/migration)
for changes to tests, fixtures, configuration, and runner behavior.

## Update the project

Remove the xUnit.net v2 and `Allure.Xunit` package references. Add the supported
xUnit.net v3 Microsoft Testing Platform package and `Allure.Xunit.v3`, and enable
the Microsoft Testing Platform runner:

```diff
<PropertyGroup>
+  <OutputType>Exe</OutputType>
+  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
</PropertyGroup>

<ItemGroup>
-  <PackageReference Include="Allure.Xunit" Version="..." />
-  <PackageReference Include="xunit" Version="..." />
-  <PackageReference Include="xunit.runner.visualstudio" Version="..." />
+  <PackageReference Include="Allure.Xunit.v3" Version="..." />
+  <PackageReference Include="xunit.v3.mtp-v2" Version="..." />
</ItemGroup>
```

`Allure.Xunit.v3` currently supports only `xunit.v3.mtp-v2`. Without
`UseMicrosoftTestingPlatformRunner` set to `true`, the adapter is not registered
and the project does not produce Allure results. xUnit.net v3 test projects are
executables and require a supported target framework; review those requirements
in the xUnit.net migration guide linked above.

Run the migrated test project with:

```bash
dotnet run --project ./MyTests.csproj
```

You can also use `dotnet test` when it is configured to run the project through
Microsoft Testing Platform.

## Find Allure results

`Allure.Xunit.v3` uses a different default result directory, which is stored
in the Microsoft Testing Platform test results folder. The path might look
like this:

```text
bin/Debug/net10.0/TestResults/allure-results
```

`Allure.Xunit.v3` supports customizing the results directory through the
configuration just as `Allure.Xunit` does.

## Update configuration

The old configuration shape is accepted for compatibility, but new projects
should use the current top-level property names:

```diff
-{
-  "allure": {
-    "title": "build-agent",
-    "directory": "allure-results",
-    "links": [
-      "https://issues.example/{issue}"
-    ]
-  }
-}
+{
+  "hostname": "build-agent",
+  "resultsDirectory": "allure-results",
+  "linkTemplates": {
+    "issue": {
+      "urlTemplate": "https://issues.example/{0}"
+    }
+  }
+}
```

Some properties have changed their names:

| Old property | New property | Notes |
| --- | --- | --- |
| `title` | `hostname` | Defines a value of the `host` label. |
| `directory` | `resultsDirectory` | A directory for Allure result files relative to the test process working directory. |
| `links` | `linkTemplates` | Now it must be a JSON object with keys being link types and values being link template objects. Each template object may include `urlTemplate` and `nameTemplate`: format strings with the `{0}` substitution fields. |

The following `Allure.Xunit` properties are not supported: `useLegacyIds`,
`xunitRunnerReporter`, `firstClassIntegrationTools`.

Make sure `allureConfig.json` is copied to the test application's output
directory, or set `ALLURE_CONFIG` to its path.

You may also rename the file to `allure.config.json`: both names are supported
with `allureConfig.json` taking precedence.

## Update namespaces

The current attributes and runtime APIs are in the `Allure` namespace. Model
types are in `Allure.Model`.

```diff
-using Allure.Xunit.Attributes;
-using Allure.Xunit.Attributes.Steps;
-using Allure.Net.Commons;
-using Allure.Net.Commons.Attributes;
+using Allure;
+using Allure.Model;
```

Do not add `using Allure.Model` unless the source file directly constructs or
examines model objects. Most tests only need `using Allure`.

## Migrate attributes

Attributes from both `Allure.Xunit.Attributes` and
`Allure.Net.Commons.Attributes` move to `Allure`:

```diff
-using Allure.Net.Commons.Attributes;
+using Allure;

 [AllureFeature("Checkout")]
 [AllureStory("Apply a promo code")]
```

Most attributes otherwise keep the same name and arguments. The following cases
need additional changes.

### Step and fixture attributes

Replace the old step namespace and fixture attribute names:

| Old attribute | New attribute |
| --- | --- |
| `Allure.Xunit.Attributes.Steps.AllureStep` | `Allure.AllureStep` |
| `Allure.Xunit.Attributes.Steps.AllureBefore` | `Allure.AllureSetUp` |
| `Allure.Xunit.Attributes.Steps.AllureAfter` | `Allure.AllureTearDown` |
| `Allure.Net.Commons.Attributes.AllureBefore` | `Allure.AllureSetUp` |
| `Allure.Net.Commons.Attributes.AllureAfter` | `Allure.AllureTearDown` |

```diff
-using Allure.Xunit.Attributes.Steps;
+using Allure;

-[AllureBefore("Prepare account")]
+[AllureSetUp("Prepare account")]
 public void PrepareAccount()
 {
 }
```

### Attributes with multiple features or stories

The xUnit.net v2 attributes accepted multiple feature or story values. Use one
attribute for each value:

```diff
-[AllureFeature("Payments", "Refunds")]
-[AllureStory("Full refund", "Partial refund")]
+[AllureFeature("Payments")]
+[AllureFeature("Refunds")]
+[AllureStory("Full refund")]
+[AllureStory("Partial refund")]
```

### Links and issues

Pass the URL or identifier as the positional argument and use `Title` for the
display text:

```diff
-[AllureLink("Allure", "https://allurereport.org")]
-[AllureIssue("Login is broken", "AUTH-123")]
+[AllureLink("https://allurereport.org", Title = "Allure")]
+[AllureIssue("AUTH-123", Title = "Login is broken")]
```

### Severity

`SeverityLevel` is replaced by `Allure.Model.Severity`. Its members use Pascal
case, and the severity argument is required:

```diff
-[AllureSeverity]
-[AllureSeverity(SeverityLevel.critical)]
+[AllureSeverity(Severity.Normal)]
+[AllureSeverity(Severity.Critical)]
```

### Allure ID

Pass an integer rather than a string:

```diff
-[AllureId("102")]
+[AllureId(102)]
```

### Step parameters

Replace `[Name]` and `[Skip]` with `[AllureParameter]`:

```diff
-using Allure.Xunit.Attributes.Steps;
+using Allure;

 [AllureStep]
 public void Submit(
-    [Name("Account")] string accountId,
-    [Skip] string accessToken)
+    [AllureParameter(Name = "Account")] string accountId,
+    [AllureParameter(Ignore = true)] string accessToken)
 {
 }
```

`AllureParameter.Excluded` has a different purpose from `Ignore`: an excluded
parameter remains visible in the report but does not affect the test history ID.
It's only relevant for test parameters and does not affect step or fixture
parameters.

### Overwrite behavior

The current metadata attributes do not support the old `overwrite` argument.
Remove metadata that should not be inherited or rearrange the attributes to
avoid adding the unwanted values:

```diff
-[AllureEpic("Default epic")]
 public class CheckoutTests
 {
     [Fact]
-    [AllureEpic("Checkout", true)]
+    [AllureEpic("Checkout")]
     public void SubmitsOrder()
     {
     }

     [Fact]
+    [AllureEpic("Default epic")]
     public void CalculatesOrderTotal()
     {
     }
 }
```

## Migrate AllureApi calls

Change `Allure.Net.Commons.AllureApi` to `Allure.AllureApi`. Most operations,
including labels, descriptions, links, steps, and screen diffs, have direct
counterparts.

```diff
-using Allure.Net.Commons;
+using Allure;

 AllureApi.AddFeature("Checkout");
```

Prefer using the new async counterparts when calling from an async method:

```diff
-AllureApi.AddFeature("Checkout");
+await AllureApi.AddFeatureAsync("Checkout");
```

Pay particular attention to the following overload changes.

### Severity

Use `Allure.Model.Severity` instead of `Allure.Net.Commons.SeverityLevel`:

```diff
-using Allure.Net.Commons;
+using Allure;
+using Allure.Model;

-AllureApi.SetSeverity(SeverityLevel.critical);
+AllureApi.SetSeverity(Severity.Critical);
```

### Asynchronous steps

Asynchronous overloads must have an `Async` suffix:

```diff
-await AllureApi.Step("Send request", async () => await client.SendAsync());
+await AllureApi.StepAsync("Send request", async () => await client.SendAsync());
```

Where available, pass the current xUnit.net v3 cancellation token to asynchronous
Allure APIs:

```csharp
await AllureApi.SetOwnerAsync(
    "Jane Doe",
    TestContext.Current.CancellationToken
);
```

### Step names

Use the single-parameter callback overload and call its `SetName` method:

```diff
-AllureApi.Step("Send request", () => { AllureApi.SetStepName("New name"); });
+AllureApi.Step("Send request", (step) => { step.SetName("New name"); });
```

Or call `AllureApi.SetName` when inside a step:

```diff
-AllureApi.Step("Send request", () => { AllureApi.SetStepName("New name"); });
+AllureApi.Step("Send request", () => { AllureApi.SetName("New name"); });
```

Async step context exposes the `SetNameAsync` method:

```diff
-await AllureApi.Step("Send request", async () => { AllureApi.SetStepName("New name"); });
+await AllureApi.StepAsync(
+    "Send request",
+    async (step, token) =>
+    {
+        await step.SetNameAsync("New name", token);
+    },
+    TestContext.Current.CancellationToken
+);
```

### Link argument order

The URL is now the first argument, followed by title, and then, for links, by type:

```diff
-AllureApi.AddLink("Allure", "https://allurereport.org");
-AllureApi.AddLink("Repository", "github", "https://github.com/allure-framework/allure-csharp");
-AllureApi.AddLink("Issue", "issue", "AUTH-123");
-AllureApi.AddIssue("Login is broken", "AUTH-123");
-AllureApi.AddTmsItem("Login test", "TMS-42");
+AllureApi.AddLink("https://allurereport.org", "Allure");
+AllureApi.AddLink("https://github.com/allure-framework/allure-csharp", "Repository", "github");
+AllureApi.AddLink("AUTH-123", "Issue", "issue");
+AllureApi.AddIssue("AUTH-123", "Login is broken");
+AllureApi.AddTmsItem("TMS-42", "Login test");
```

### Attachments

Use the `AddAttachmentFromFile` methods for paths. The path now always comes first:

```diff
-AllureApi.AddAttachment("response", "application/json", responsePath);
-AllureApi.AddAttachment(responsePath, "response");
+AllureApi.AddAttachmentFromFile(responsePath, "response", "application/json");
+AllureApi.AddAttachmentFromFile(responsePath, "response");
```

For in-memory attachments, name is mandatory and always comes first, followed by
content:

```diff
-AllureApi.AddAttachment("response 1", null, response1Bytes);
-AllureApi.AddAttachment("response 2", "application/json", response2Bytes);
-AllureApi.AddAttachment("response 3", "application/octet-stream", response3Bytes, ".bin");
+AllureApi.AddAttachment("response 1", response1Bytes);
+AllureApi.AddAttachment("response 2", response2Bytes, "application/json");
+AllureApi.AddAttachment("response 3", response3Bytes, "application/octet-stream", ".bin");
```

There is an overload for text attachments:

```diff
-AllureApi.AddAttachment("logs", "text/plain", Encoding.UTF8.GetBytes(logs));
+AllureApi.AddAttachment("logs", logs, "text/plain");
```

The same changes apply to `AddGlobalAttachment`:

```diff
-AllureApi.AddGlobalAttachment("environment", "text/plain", environmentPath);
-AllureApi.AddGlobalAttachment("logs", "text/plain", Encoding.UTF8.GetBytes(logs));
+AllureApi.AddGlobalAttachmentFromFile(
+    environmentPath,
+    "environment",
+    "text/plain"
+);
+AllureApi.AddGlobalAttachment("logs", logs, "text/plain");
```

For screen diffs passed as paths, use `AddScreenDiffFromFiles`:

```diff
-AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");
+AllureApi.AddScreenDiffFromFiles("expected.png", "actual.png", "diff.png");
```

The byte-array overload remains `AddScreenDiff`.

### Test parameters

Use `AddTestParameterFromObject` when Allure should format a value. Use
`AddTestParameter` when the value is already a string or when constructing a
`Parameter` explicitly:

```diff
-AllureApi.AddTestParameter("attempt", 3, ParameterMode.masked, true);
+AllureApi.AddTestParameterFromObject(
+    "attempt",
+    3,
+    ParameterMode.Masked,
+    true
+);
```

```csharp
AllureApi.AddTestParameter(new Parameter
{
    Name = "account",
    Value = accountId,
    Mode = ParameterMode.Masked,
    Excluded = true,
});
```

## Migrate type formatters

Replace `TypeFormatter<T>` implementations with parameter serialization rules.
For a direct conversion, derive the new rule from
`TypedParameterSerializationRule<T>`:

```diff
-using Allure.Net.Commons;
+using Allure.Sdk.Serialization;

-public sealed class AccountFormatter : TypeFormatter<Account>
+public sealed class AccountSerializationRule :
+    TypedParameterSerializationRule<Account>
 {
-    public override string Format(Account value) => value.Id;
+    protected override string Serialize(Account value) => value.Id;
 }
```

Unlike `AllureLifecycle.AddTypeFormatter`, serialization rules cannot be added
after the Allure runtime has been registered. They can only be configured as
part of runtime registration. For `Allure.Xunit.v3`, add the rule from an
`IAllureXunitRegistrationHook`:

```csharp
using Allure.Sdk.Registration;
using Allure.Xunit.Registration;

public sealed class ProjectAllureRegistration : IAllureXunitRegistrationHook
{
    public void SetUp(IAllureXunitRegistrationContext context)
    {
        context.ConfigureSerialization(
            rules => rules.AddRule(new AccountSerializationRule())
        );
    }
}
```

For a formatter that does not need a dedicated class, add a delegate rule
instead:

```csharp
context.ConfigureSerialization(
    rules => rules.AddDelegateRule<Account>(account => account.Id)
);
```

Select the hook before Allure registration begins. See
[Registration hook](README.md#registration-hook) for the available registration
mechanisms.

The old type formatter API matched only the exact runtime type. A rule derived
from `TypedParameterSerializationRule<T>` accepts any value compatible with
`T`. A rule for a base class therefore also handles its subclasses, and a rule
for an interface handles implementations of that interface. Delegate rules
created by `AddDelegateRule<T>` derive from the same typed rule and have the
same behavior. If multiple rules accept a value, the last added matching rule
has priority.

## Replace ExtendedApi

Replace `ExtendedApi.Before` and `ExtendedApi.After` with setup and teardown
operations from `AllureApi`:

```diff
-ExtendedApi.Before("Create account", CreateAccount);
-ExtendedApi.After("Delete account", DeleteAccount);
+AllureApi.SetUp("Create account", CreateAccount);
+AllureApi.TearDown("Delete account", DeleteAccount);
```

Use `SetUpAsync` and `TearDownAsync` for asynchronous delegates:

```diff
-await ExtendedApi.Before("Create account", CreateAccountAsync);
-await ExtendedApi.After("Delete account", DeleteAccountAsync);
+await AllureApi.SetUpAsync("Create account", CreateAccountAsync);
+await AllureApi.TearDownAsync("Delete account", DeleteAccountAsync);
```

The low-level `ExtendedApi` functions that separately start, pass, fail, break,
skip, or resolve a step or fixture do not have direct replacements. Restructure
the code so `AllureApi.Step`, `AllureApi.SetUp`, or `AllureApi.TearDown` encloses
the operation. These APIs determine the final status from the enclosed delegate.

## Replace AllureLifecycle

Prefer a focused `AllureApi` operation over editing a result model directly:

```diff
-AllureLifecycle.Instance.UpdateTestCase(result =>
-{
-    result.name = "Authorized checkout";
-    result.labels.Add(new Label { name = "tenant", value = tenant });
-});
+AllureApi.SetTestName("Authorized checkout");
+AllureApi.AddLabel("tenant", tenant);
```

Lifecycle-related operations that manually schedule, start, stop, write, or
manage test containers have no public replacement. They are intended for
integration authors and are not exposed by `Allure.Net`.

### Model access - not supported yet

> **Current limitation:** `Allure.Xunit.v3` does not yet support model updates.
> Calling any of the methods in the following table throws
> `NotImplementedException`. Replace model mutations
> with supported `AllureApi` operations where possible. If no high-level
> operation expresses the required behavior, that code cannot be migrated yet.

There are direct model API equivalents for the three commonly used lifecycle
updates:

| Old operation | New model operation |
| --- | --- |
| `UpdateTestCase(...)` | `AllureInProcessApi.UpdateTestResult(...)` |
| `UpdateFixture(...)` | `AllureInProcessApi.UpdateFixtureResult(...)` |
| `UpdateStep(...)` | `AllureInProcessApi.UpdateStepResult(...)` |

Note that unlike `Allure.Net.Commons`, `Allure.Net` schedules the callbacks
of the update methods to run at some moment in the future rather than running
them immediately. If you rely on the callback's side effect, switch to the
corresponding read operation (see the next section).

### Model read operations - not supported yet

> **Current limitation:** `Allure.Xunit.v3` does not yet support model reads.
> Calling any of the read methods of `AllureInProcessApi` throws
> `NotImplementedException`. Code that relies on the model data cannot be
> migrated yet.

If you rely on the value provided by such a callback, use the read operation
rather than an update:

```diff
-string? testName = null;
-AllureLifecycle.Instance.UpdateTestCase(result => testName = result.name);
+var testName = AllureInProcessApi.ReadTestResult(result => result.Name);
```

Otherwise, the variable may remain unchanged as the callback won't be invoked
immediately.

Unlike updates, reads synchronize with the lifecycle pausing the test method
until the operation is run by the Allure runtime.

## Migrate model types

Model types moved from `Allure.Net.Commons` to `Allure.Model`:

```diff
-using Allure.Net.Commons;
+using Allure.Model;
```

Their properties and enum members now use Pascal case:

| Old | New |
| --- | --- |
| `result.name` | `result.Name` |
| `result.status` | `result.Status` |
| `result.statusDetails` | `result.StatusDetails` |
| `result.descriptionHtml` | `result.DescriptionHtml` |
| `result.parameters` | `result.Parameters` |
| `result.attachments` | `result.Attachments` |
| `result.steps` | `result.Steps` |
| `test.uuid` | `test.Uuid` |
| `test.fullName` | `test.FullName` |
| `test.testCaseId` | `test.TestCaseId` |
| `test.historyId` | `test.HistoryId` |
| `test.labels` | `test.Labels` |
| `test.links` | `test.Links` |
| `Status.passed` | `Status.Passed` |
| `Stage.running` | `Stage.Running` |
| `ParameterMode.masked` | `ParameterMode.Masked` |
| `SeverityLevel.critical` | `Severity.Critical` |

Several model properties are now required. Object initializers must supply them:

```csharp
var parameter = new Parameter
{
    Name = "browser",
    Value = "Firefox",
};

var label = new Label
{
    Name = "component",
    Value = "checkout",
};
```

Required properties include:

- `ExecutableItem.Name`;
- `TestResult.Uuid`;
- `TestResultScope.Uuid`;
- `Label.Name` and `Label.Value`;
- `Link.Url`;
- `Parameter.Name` and `Parameter.Value`;
- `Attachment.Name` and `Attachment.Source`;
- `StatusDetails.Message`.

Prefer the `AllureApi` overloads and model factory methods, such as
`Label.Feature(...)`, over constructing model objects directly.

## Verify the migration

Before removing the old packages or compatibility suppressions, search the
solution for remaining legacy APIs:

```bash
rg 'Allure\.Xunit\.Attributes|Allure\.Net\.Commons|AllureLifecycle|TypeFormatter|ExtendedApi'
```

Or, on Windows with no `ripgrep` installed:

```powershell
Get-ChildItem -Filter "*.cs" -Recurse | Select-String 'Allure\.Xunit\.Attributes|Allure\.Net\.Commons|AllureLifecycle|TypeFormatter|ExtendedApi'
```

Then:

1. Build without suppressing the compatibility API's obsolete warnings.
2. Run all tests through Microsoft Testing Platform.
3. Confirm that new files appear in the expected `allure-results` directory.
4. Generate an Allure report and compare test names, hierarchy, labels, links,
   parameters, steps, fixtures, attachments, and statuses with the baseline.
5. Confirm that no direct model read or update remains; these operations are not
   supported by `Allure.Xunit.v3` yet.
