using System.Text.Json;
using Allure.Testing;
using Allure.Testing.Assertions.Model;
using TUnit.Assertions.Core;

namespace Allure.Xunit.v3.Tests.Steps;

class StepTests
{
    const string SampleNamespace = "Allure.Xunit.v3.Tests.Samples.Steps.StepApi";

    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.StepApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(12);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task StepAttributeWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(FullName("AttributeSteps"))
            .With.StepsMatching([
                PassedLeaf("Void"),
                PassedLeaf("Return"),
                PassedLeaf("Async"),
                PassedLeaf("AsyncReturn"),
                PassedLeaf("Renamed"),
                step => step.HasName("Parameters")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        parameter => parameter.HasName("plain")
                            .And.HasValue("1")
                            .And.HasNoMode(),
                        parameter => parameter.HasName("renamed")
                            .And.HasValue("3")
                            .And.HasMode(AllureParameterMode.Default),
                        parameter => parameter.HasName("masked")
                            .And.HasValue("4")
                            .And.HasMode(AllureParameterMode.Masked),
                        parameter => parameter.HasName("hidden")
                            .And.HasValue("5")
                            .And.HasMode(AllureParameterMode.Hidden),
                    ])
                    .And.HasStepsMatching([]),
            ]);
    }

    [Test]
    [Arguments("AllureApiSyncLambdaSteps", "AllureApi sync lambda")]
    [Arguments("AllureApiAsyncLambdaSteps", "AllureApi async lambda")]
    [Arguments("AllureInProcessApiSyncLambdaSteps", "AllureInProcessApi sync lambda")]
    [Arguments("AllureInProcessApiAsyncLambdaSteps", "AllureInProcessApi async lambda")]
    public async Task RuntimeLambdaAndContextOperationsWork(
        string className,
        string stepName
    )
    {
        await Assert.That(results.Value).HasSingleTestResult(FullName(className))
            .With.StepsMatching([
                step => step.HasName(stepName)
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        parameter => parameter.HasName("context").And.HasValue("works"),
                    ])
                    .And.HasStepsMatching([]),
            ]);
    }

    [Test]
    public async Task SyncStepsCanBeNested()
    {
        await AssertNestedSteps(
            "NestedAllureApiSyncSteps",
            "Outer sync step",
            "Inner sync step"
        );
    }

    [Test]
    public async Task AsyncStepsCanBeNested()
    {
        await AssertNestedSteps(
            "NestedAllureApiAsyncSteps",
            "Outer async step",
            "Inner async step"
        );
    }

    [Test]
    public async Task AttributeAndRuntimeStepsCanBeMixed()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            FullName("MixedAttributeAndApiSteps")
        ).With.StepsMatching([
            step => step.HasName("Attribute outer")
                .And.HasStatus(AllureStatus.Passed)
                .And.HasParametersMatching([])
                .And.HasStepsMatching([PassedLeaf("Runtime inner")]),
            step => step.HasName("Runtime outer")
                .And.HasStatus(AllureStatus.Passed)
                .And.HasParametersMatching([])
                .And.HasStepsMatching([PassedLeaf("Attribute inner")]),
        ]);
    }

    [Test]
    [Arguments("SyncContextOperationFromSubstep", "Sync parent renamed from child", "Sync child")]
    [Arguments("AsyncContextOperationFromSubstep", "Async parent renamed from child", "Async child")]
    public async Task ContextOperationFromSubstepTargetsParent(
        string className,
        string parentName,
        string childName
    )
    {
        await Assert.That(results.Value).HasSingleTestResult(FullName(className))
            .With.StepsMatching([
                step => step.HasName(parentName)
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        parameter => parameter.HasName("substep context")
                            .And.HasValue("works"),
                    ])
                    .And.HasStepsMatching([PassedLeaf(childName)]),
            ]);
    }

    [Test]
    public async Task StepsCanRunConcurrently()
    {
        var steps = GetSteps("ConcurrentAllureApiSteps");
        var expectedNames = Enumerable.Range(1, 5)
            .Select(index => $"Concurrent step {index}");

        await Assert.That(steps.Length).IsEqualTo(5);
        await Assert.That(steps.Select(Name)).IsEquivalentTo(expectedNames);
        await Assert.That(steps.Select(Status))
            .IsEquivalentTo(Enumerable.Repeat("passed", 5));
        await Assert.That(steps.SelectMany(Steps)).IsEmpty();
    }

    [Test]
    public async Task ConcurrentNestedStepsDoNotInterleave()
    {
        var parents = GetSteps("NestedConcurrentAllureApiSteps");
        var expectedParents = Enumerable.Range(1, 3)
            .Select(index => $"Parent {index}");

        await Assert.That(parents.Length).IsEqualTo(3);
        await Assert.That(parents.Select(Name)).IsEquivalentTo(expectedParents);

        foreach (var parent in parents)
        {
            var parentNumber = Name(parent).Split(' ')[1];
            var children = Steps(parent);
            var expectedChildren = Enumerable.Range(1, 3)
                .Select(index => $"Child {parentNumber}.{index}");

            await Assert.That(Status(parent)).IsEqualTo("passed");
            await Assert.That(children.Length).IsEqualTo(3);
            await Assert.That(children.Select(Name)).IsEquivalentTo(expectedChildren);
            await Assert.That(children.Select(Status))
                .IsEquivalentTo(Enumerable.Repeat("passed", 3));
            await Assert.That(children.SelectMany(Steps)).IsEmpty();
        }
    }

    static async Task AssertNestedSteps(
        string className,
        string outerName,
        string innerName
    )
    {
        await Assert.That(results.Value).HasSingleTestResult(FullName(className))
            .With.StepsMatching([
                step => step.HasName(outerName)
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([PassedLeaf(innerName)]),
            ]);
    }

    static Func<IAssertionSource<AllureStepResult>, IAssertion> PassedLeaf(
        string name
    ) => step => step.HasName(name)
        .And.HasStatus(AllureStatus.Passed)
        .And.HasParametersMatching([])
        .And.HasStepsMatching([]);

    static JsonElement[] GetSteps(string className) => results.Value.TestResults
        .Single(result => result.Json.GetProperty("name").GetString() == FullName(className))
        .Json.GetProperty("steps")
        .EnumerateArray()
        .ToArray();

    static JsonElement[] Steps(JsonElement item) => item.GetProperty("steps")
        .EnumerateArray()
        .ToArray();

    static string Name(JsonElement item) => item.GetProperty("name").GetString()!;

    static string Status(JsonElement item) => item.GetProperty("status").GetString()!;

    static string FullName(string className) =>
        $"{SampleNamespace}.{className}.TestMethod";
}
