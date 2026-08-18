using Allure.Model;

namespace Allure.TestingPlatform.Tests.OperationTests;

[InheritsTests]
public class SyncOperationTests : OperationTestsBase
{
    protected override Task AddAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    )
    {
        AllureApi.AddAttachment(name, content, mediaType, fileExtension);
        return Task.CompletedTask;
    }

    protected override Task AddAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    )
    {
        AllureApi.AddAttachmentFromFile(path, name, mediaType, fileExtension);
        return Task.CompletedTask;
    }

    protected override Task AddGlobalAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    )
    {
        AllureApi.AddGlobalAttachment(name, content, mediaType, fileExtension);
        return Task.CompletedTask;
    }

    protected override Task AddGlobalAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    )
    {
        AllureApi.AddGlobalAttachmentFromFile(path, name, mediaType, fileExtension);
        return Task.CompletedTask;
    }

    protected override Task AddGlobalError(StatusDetails statusDetails)
    {
        AllureApi.AddGlobalError(statusDetails);
        return Task.CompletedTask;
    }

    protected override Task AddLabel(Label label)
    {
        AllureApi.AddLabel(label);
        return Task.CompletedTask;
    }

    protected override Task AddLabels(IEnumerable<Label> labels)
    {
        AllureApi.AddLabels(labels);
        return Task.CompletedTask;
    }

    protected override Task AddLink(Link link)
    {
        AllureApi.AddLink(link);
        return Task.CompletedTask;
    }

    protected override Task AddLinks(IEnumerable<Link> links)
    {
        AllureApi.AddLinks(links);
        return Task.CompletedTask;
    }

    protected override Task AddScreenDiff(
        Stream expected,
        Stream actual,
        Stream diff
    )
    {
        AllureApi.AddScreenDiff(expected, actual, diff);
        return Task.CompletedTask;
    }

    protected override Task AddScreenDiffFromFiles(
        string expectedPath,
        string actualPath,
        string diffPath
    )
    {
        AllureApi.AddScreenDiffFromFiles(expectedPath, actualPath, diffPath);
        return Task.CompletedTask;
    }

    protected override Task AddTestParameter(Parameter parameter)
    {
        AllureApi.AddTestParameter(parameter);
        return Task.CompletedTask;
    }

    protected override Task SetDescription(string description)
    {
        AllureApi.SetDescription(description);
        return Task.CompletedTask;
    }

    protected override Task SetDescriptionHtml(string descriptionHtml)
    {
        AllureApi.SetDescriptionHtml(descriptionHtml);
        return Task.CompletedTask;
    }

    protected override Task SetFixtureName(string newName)
    {
        AllureApi.SetFixtureName(newName);
        return Task.CompletedTask;
    }

    protected override Task SetLabel(string value)
    {
        AllureApi.SetOwner(value);
        return Task.CompletedTask;
    }

    protected override Task SetName(string newName)
    {
        AllureApi.SetName(newName);
        return Task.CompletedTask;
    }

    protected override Task SetTestName(string newName)
    {
        AllureApi.SetTestName(newName);
        return Task.CompletedTask;
    }

    protected override Task SetUpAction(string name, Action body)
    {
        AllureApi.SetUp(name, _ => body());
        return Task.CompletedTask;
    }

    protected override Task<int> SetUpFunction(string name, Func<int> body) =>
        Task.FromResult(AllureApi.SetUp(name, _ => body()));

    protected override Task StepCompleted(
        string name,
        Status status,
        StatusDetails statusDetails
    )
    {
        AllureApi.Step(name, status, statusDetails);
        return Task.CompletedTask;
    }

    protected override Task StepAction(string name, Action body)
    {
        AllureApi.Step(name, _ => body());
        return Task.CompletedTask;
    }

    protected override Task<int> StepFunction(string name, Func<int> body) =>
        Task.FromResult(AllureApi.Step(name, _ => body()));

    protected override Task RunNestedSteps(string outerName, string innerName)
    {
        AllureApi.Step(
            outerName,
            _ => AllureApi.Step(innerName, _ => { })
        );
        return Task.CompletedTask;
    }

    protected override async Task RunConcurrentSteps(
        params string[] names
    )
    {
        var started = 0;
        var release = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        void Body()
        {
            if (Interlocked.Increment(ref started) == names.Length)
            {
                release.SetResult();
            }
            if (!release.Task.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException("The steps did not run concurrently.");
            }
        }

        await Task.WhenAll(
            [.. names.Select(n => Task.Run(() => AllureApi.Step(n, _ => Body())))]
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

        void WaitForAll(
            ref int started,
            int count,
            TaskCompletionSource release
        )
        {
            if (Interlocked.Increment(ref started) == count)
            {
                release.SetResult();
            }
            if (!release.Task.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("The steps did not run concurrently.");
            }
        }

        Task RunOnDedicatedThread(Action action) =>
            Task.Factory.StartNew(
                action,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

        await Task.WhenAll(
            stepNames.Select(pair => RunOnDedicatedThread(() =>
                AllureApi.Step(pair.Key, _ =>
                {
                    WaitForAll(
                        ref parentsStarted,
                        stepNames.Count,
                        releaseParents
                    );
                    Task.WaitAll([
                        .. pair.Value.Select(child => RunOnDedicatedThread(() =>
                            AllureApi.Step(child, _ => WaitForAll(
                                ref childrenStarted,
                                childCount,
                                releaseChildren
                            ))
                        )),
                    ]);
                })
            ))
        );
    }

    protected override Task RunAllureSetUpAttribute(
        int first,
        int second,
        int third
    )
    {
        InstrumentedSetUp(first, second, third);
        return Task.CompletedTask;
    }

    protected override Task<int> RunAllureSetUpAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        Task.FromResult(InstrumentedSetUpFunction(first, second, third));

    protected override Task RunAllureTearDownAttribute(
        int first,
        int second,
        int third
    )
    {
        InstrumentedTearDown(first, second, third);
        return Task.CompletedTask;
    }

    protected override Task<int> RunAllureTearDownAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        Task.FromResult(InstrumentedTearDownFunction(first, second, third));

    protected override Task RunAllureStepAttribute(
        int first,
        int second,
        int third
    )
    {
        InstrumentedStep(first, second, third);
        return Task.CompletedTask;
    }

    protected override Task<int> RunAllureStepAttributeFunction(
        int first,
        int second,
        int third
    ) =>
        Task.FromResult(InstrumentedStepFunction(first, second, third));

    protected override Task TearDownAction(string name, Action body)
    {
        AllureApi.TearDown(name, _ => body());
        return Task.CompletedTask;
    }

    protected override Task<int> TearDownFunction(string name, Func<int> body) =>
        Task.FromResult(AllureApi.TearDown(name, _ => body()));

    [AllureSetUp("Set up")]
    static void InstrumentedSetUp(int first, int second, int third) { }

    [AllureSetUp("Set up function")]
    static int InstrumentedSetUpFunction(int first, int second, int third) =>
        first + second + third;

    [AllureTearDown("Tear down")]
    static void InstrumentedTearDown(int first, int second, int third) { }

    [AllureTearDown("Tear down function")]
    static int InstrumentedTearDownFunction(int first, int second, int third) =>
        first + second + third;

    [AllureStep("Step")]
    static void InstrumentedStep(int first, int second, int third) { }

    [AllureStep("Step function")]
    static int InstrumentedStepFunction(int first, int second, int third) =>
        first + second + third;
}
