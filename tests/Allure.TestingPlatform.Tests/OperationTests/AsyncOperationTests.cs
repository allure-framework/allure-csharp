using Allure.Model;
using Allure.Abstractions;

namespace Allure.TestingPlatform.Tests.OperationTests;

[InheritsTests]
public class AsyncOperationTests : OperationTestsBase
{
    protected override Task AddAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    ) =>
        AllureApi.AddAttachmentAsync(
            name,
            content,
            mediaType,
            fileExtension,
            CancellationToken.None
        );

    protected override Task AddAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    ) =>
        AllureApi.AddAttachmentFromFileAsync(
            path,
            name,
            mediaType,
            fileExtension,
            CancellationToken.None
        );

    protected override Task AddGlobalAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    ) =>
        AllureApi.AddGlobalAttachmentAsync(
            name,
            content,
            mediaType,
            fileExtension,
            CancellationToken.None
        );

    protected override Task AddGlobalAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    ) =>
        AllureApi.AddGlobalAttachmentFromFileAsync(
            path,
            name,
            mediaType,
            fileExtension,
            CancellationToken.None
        );

    protected override Task AddGlobalError(StatusDetails statusDetails) =>
        AllureApi.AddGlobalErrorAsync(statusDetails, CancellationToken.None);

    protected override Task AddLabel(Label label) =>
        AllureApi.AddLabelAsync(label, CancellationToken.None);

    protected override Task AddLabels(IEnumerable<Label> labels) =>
        AllureApi.AddLabelsAsync(labels, CancellationToken.None);

    protected override Task AddLink(Link link) =>
        AllureApi.AddLinkAsync(link, CancellationToken.None);

    protected override Task AddLinks(IEnumerable<Link> links) =>
        AllureApi.AddLinksAsync(links, CancellationToken.None);

    protected override Task AddScreenDiff(
        Stream expected,
        Stream actual,
        Stream diff
    ) =>
        AllureApi.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            CancellationToken.None
        );

    protected override Task AddScreenDiffFromFiles(
        string expectedPath,
        string actualPath,
        string diffPath
    ) =>
        AllureApi.AddScreenDiffFromFilesAsync(
            expectedPath,
            actualPath,
            diffPath,
            CancellationToken.None
        );

    protected override Task AddTestParameter(Parameter parameter) =>
        AllureApi.AddTestParameterAsync(parameter, CancellationToken.None);

    protected override Task SetDescription(string description) =>
        AllureApi.SetDescriptionAsync(description, CancellationToken.None);

    protected override Task SetDescriptionHtml(string descriptionHtml) =>
        AllureApi.SetDescriptionHtmlAsync(
            descriptionHtml,
            CancellationToken.None
        );

    protected override Task SetFixtureName(string newName) =>
        AllureApi.SetFixtureNameAsync(newName, CancellationToken.None);

    protected override Task SetLabel(string value) =>
        AllureApi.SetOwnerAsync(value, CancellationToken.None);

    protected override Task SetName(string newName) =>
        AllureApi.SetNameAsync(newName, CancellationToken.None);

    protected override Task SetTestName(string newName) =>
        AllureApi.SetTestNameAsync(newName, CancellationToken.None);

    protected override Task SetUpAction(string name, Action body) =>
        AllureApi.SetUpAsync(
            name,
            (_, _) =>
            {
                body();
                return Task.CompletedTask;
            },
            CancellationToken.None
        );

    protected override Task<int> SetUpFunction(string name, Func<int> body) =>
        AllureApi.SetUpAsync(
            name,
            (_, _) => Task.FromResult(body()),
            CancellationToken.None
        );

    protected override Task SetFixtureContextName(string newName) =>
        AllureApi.SetUpAsync(
            "fixture",
            (context, token) => context.SetNameAsync(newName, token),
            CancellationToken.None
        );

    protected override Task AddFixtureContextParameter(Parameter parameter) =>
        AllureApi.SetUpAsync(
            "fixture",
            (context, token) => context.AddParameterAsync(parameter, token),
            CancellationToken.None
        );

    protected override async Task SetNameOnDisposedFixtureContext()
    {
        var context = await CaptureDisposedFixtureContext();
        await context.SetNameAsync("renamed fixture", CancellationToken.None);
    }

    protected override async Task AddParameterToDisposedFixtureContext(
        Parameter parameter
    )
    {
        var context = await CaptureDisposedFixtureContext();
        await context.AddParameterAsync(parameter, CancellationToken.None);
    }

    protected override async Task ReadDisposedFixtureContext()
    {
        var context = await CaptureDisposedFixtureContext();
        context.TryReadFixtureResult(fixture => fixture.Name, out _);
    }

    protected override async Task UpdateDisposedFixtureContext()
    {
        var context = await CaptureDisposedFixtureContext();
        context.UpdateFixtureResult(fixture => fixture.Name = "renamed fixture");
    }

    protected override Task StepCompleted(
        string name,
        Status status,
        StatusDetails statusDetails
    ) =>
        AllureApi.StepAsync(
            name,
            status,
            statusDetails,
            CancellationToken.None
        );

    protected override Task StepAction(string name, Action body) =>
        AllureApi.StepAsync(
            name,
            (_, _) =>
            {
                body();
                return Task.CompletedTask;
            },
            CancellationToken.None
        );

    protected override Task<int> StepFunction(string name, Func<int> body) =>
        AllureApi.StepAsync(
            name,
            (_, _) => Task.FromResult(body()),
            CancellationToken.None
        );

    protected override Task SetStepContextName(string newName) =>
        AllureApi.StepAsync(
            "step",
            (context, token) => context.SetNameAsync(newName, token),
            CancellationToken.None
        );

    protected override Task AddStepContextParameter(Parameter parameter) =>
        AllureApi.StepAsync(
            "step",
            (context, token) => context.AddParameterAsync(parameter, token),
            CancellationToken.None
        );

    protected override async Task SetNameOnDisposedStepContext()
    {
        var context = await CaptureDisposedStepContext();
        await context.SetNameAsync("renamed step", CancellationToken.None);
    }

    protected override async Task AddParameterToDisposedStepContext(
        Parameter parameter
    )
    {
        var context = await CaptureDisposedStepContext();
        await context.AddParameterAsync(parameter, CancellationToken.None);
    }

    protected override async Task ReadDisposedStepContext()
    {
        var context = await CaptureDisposedStepContext();
        context.TryReadStepResult(step => step.Name, out _);
    }

    protected override async Task UpdateDisposedStepContext()
    {
        var context = await CaptureDisposedStepContext();
        context.UpdateStepResult(step => step.Name = "renamed step");
    }

    static async Task<IAllureInProcessAsyncFixtureContext> CaptureDisposedFixtureContext()
    {
        IAllureInProcessAsyncFixtureContext context = null;
        await AllureInProcessApi.SetUpAsync(
            "fixture",
            current =>
            {
                context = current;
                return Task.CompletedTask;
            }
        );
        return context;
    }

    static async Task<IAllureInProcessAsyncStepContext> CaptureDisposedStepContext()
    {
        IAllureInProcessAsyncStepContext context = null;
        await AllureInProcessApi.StepAsync(
            "step",
            current =>
            {
                context = current;
                return Task.CompletedTask;
            }
        );
        return context;
    }

    protected override Task RunNestedSteps(string outerName, string innerName) =>
        AllureApi.StepAsync(
            outerName,
            (_, token) => AllureApi.StepAsync(
                innerName,
                (_, _) => Task.CompletedTask,
                token
            ),
            CancellationToken.None
        );

    protected override async Task RunConcurrentSteps(
        params string[] names
    )
    {
        var started = 0;
        var release = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        async Task Body()
        {
            if (Interlocked.Increment(ref started) == names.Length)
            {
                release.SetResult();
            }
            await release.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }

        await Task.WhenAll(
            [.. names.Select(n => AllureApi.StepAsync(
                n,
                (_, _) => Body(),
                CancellationToken.None
            ))]
        );
    }

    protected override async Task RunNestedConcurrentSteps(
        IReadOnlyDictionary<string, string[]> stepNames
    )
    {
        var parentsStarted = 0;
        var childrenStarted = 0;
        var releaseParents = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var releaseChildren = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var childCount = stepNames.Sum(pair => pair.Value.Length);

        async Task RunChildren(string[] names)
        {
            await Task.WhenAll(
                names.Select(name => AllureApi.StepAsync(
                    name,
                    async (_, _) =>
                    {
                        if (Interlocked.Increment(ref childrenStarted) == childCount)
                        {
                            releaseChildren.SetResult();
                        }
                        await releaseChildren.Task.WaitAsync(
                            TimeSpan.FromSeconds(10)
                        );
                    },
                    CancellationToken.None
                ))
            );
        }

        await Task.WhenAll(
            stepNames.Select(pair => AllureApi.StepAsync(
                pair.Key,
                async (_, _) =>
                {
                    if (Interlocked.Increment(ref parentsStarted) == stepNames.Count)
                    {
                        releaseParents.SetResult();
                    }
                    await releaseParents.Task.WaitAsync(TimeSpan.FromSeconds(10));
                    await RunChildren(pair.Value);
                },
                CancellationToken.None
            ))
        );
    }

    protected override Task RunAllureSetUpAttribute(
        int first,
        int second,
        int third
    ) =>
        InstrumentedSetUpAsync(first, second, third);

    protected override Task<int> RunAllureSetUpAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        InstrumentedSetUpFunctionAsync(first, second, third);

    protected override Task RunAllureTearDownAttribute(
        int first,
        int second,
        int third
    ) =>
        InstrumentedTearDownAsync(first, second, third);

    protected override Task<int> RunAllureTearDownAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        InstrumentedTearDownFunctionAsync(first, second, third);

    protected override Task RunAllureStepAttribute(
        int first,
        int second,
        int third
    ) =>
        InstrumentedStepAsync(first, second, third);

    protected override Task<int> RunAllureStepAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        InstrumentedStepFunctionAsync(first, second, third);

    protected override Task TearDownAction(string name, Action body) =>
        AllureApi.TearDownAsync(
            name,
            (_, _) =>
            {
                body();
                return Task.CompletedTask;
            },
            CancellationToken.None
        );

    protected override Task<int> TearDownFunction(string name, Func<int> body) =>
        AllureApi.TearDownAsync(
            name,
            (_, _) => Task.FromResult(body()),
            CancellationToken.None
        );

    [AllureSetUp("Set up")]
    static async Task InstrumentedSetUpAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.CompletedTask;

    [AllureSetUp("Set up function")]
    static async Task<int> InstrumentedSetUpFunctionAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.FromResult(first + second + third);

    [AllureTearDown("Tear down")]
    static async Task InstrumentedTearDownAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.CompletedTask;

    [AllureTearDown("Tear down function")]
    static async Task<int> InstrumentedTearDownFunctionAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.FromResult(first + second + third);

    [AllureStep("Step")]
    static async Task InstrumentedStepAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.CompletedTask;

    [AllureStep("Step function")]
    static async Task<int> InstrumentedStepFunctionAsync(
        int first,
        int second,
        int third
    ) =>
        await Task.FromResult(first + second + third);
}
