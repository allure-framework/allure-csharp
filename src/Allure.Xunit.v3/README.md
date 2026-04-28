# Allure.Xunit.v3

> Alpha preview of an Allure adapter for [xUnit.net v3](https://xunit.net/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

## Status

This package is an **alpha** and is under active development.

Current focus:

- validate xUnit v3 integration boundaries,
- stabilize reporter wiring and output behavior,
- keep existing `Allure.Xunit` (xUnit v2) behavior unchanged.

## Scope and limitations (important)

xUnit v3 custom runner reporters are supported only in **in-process execution**.

Use:

- direct test executable execution, or
- `dotnet run --project <test-project> -- -reporter allure`

Not supported by xUnit v3 custom reporters:

- `dotnet test`
- Visual Studio Test Explorer
- multi-assembly runners

## Package design contract

`Allure.Xunit.v3` is designed as a **contract-only xUnit extension**:

- depends on xUnit v3 reporter contracts for compile/build,
- does not transitively pin or leak xUnit packages to consumers.

## Getting started (alpha)

1. Add `Allure.Xunit.v3` to an xUnit v3 test project.
2. Ensure your test project runs in-process (for example via `dotnet run`).
3. Select reporter switch `allure`.
4. Generate the report from produced `allure-results`.

## Runtime API scope requirement (xUnit v3)

Under Microsoft Testing Platform execution, runtime `AllureApi` calls are captured only inside an explicit runtime scope:

```csharp
using (AllureRuntimeScope.Begin())
{
    AllureApi.AddEpic("foo");
    AllureApi.SetDescription("details");
}
```

This applies to runtime mutations executed in the test body and `Dispose`.

## Examples

See:

- [Allure.Xunit.v3.Examples](../../examples/Allure.Xunit.v3.Examples)

## Existing xUnit v2 adapter

For production-ready xUnit v2 usage, continue using:

- [Allure.Xunit](https://www.nuget.org/packages/Allure.Xunit)
