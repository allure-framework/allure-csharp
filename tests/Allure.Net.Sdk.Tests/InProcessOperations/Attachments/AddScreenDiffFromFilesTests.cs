using System.Text.Json;
using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Attachments;

public class AddScreenDiffFromFilesTests
{
    [Test]
    public async Task AddScreenDiffFromFilesReadsFilesAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var paths = await CreateScreenFiles();

        try
        {
            environment.Run(current =>
            {
                current.Runtime.LifecycleApi.StartTest(test);
                AllureApi.AddScreenDiffFromFiles(
                    paths.Expected,
                    paths.Actual,
                    paths.Diff
                );
            });

            await AssertDiff(environment, test);
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncReadsFilesAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var paths = await CreateScreenFiles();

        try
        {
            await environment.RunAsync(async current =>
            {
                current.Runtime.LifecycleApi.StartTest(test);
                await AllureApi.AddScreenDiffFromFilesAsync(
                    paths.Expected,
                    paths.Actual,
                    paths.Diff,
                    CancellationToken.None
                );
            });

            await AssertDiff(environment, test);
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var paths = await CreateScreenFiles();

        try
        {
            environment.Run(current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                AllureInProcessApi.SetUp("fixture", _ =>
                    AllureApi.AddScreenDiffFromFiles(
                        paths.Expected,
                        paths.Actual,
                        paths.Diff
                    )
                );
            });

            await Assert.That(scope.Befores.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var paths = await CreateScreenFiles();

        try
        {
            environment.Run(current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                AllureInProcessApi.SetUp("fixture", _ =>
                    AllureInProcessApi.Step("step", _ =>
                        AllureApi.AddScreenDiffFromFiles(
                            paths.Expected,
                            paths.Actual,
                            paths.Diff
                        )
                    )
                );
            });

            var fixture = scope.Befores.Single();
            await Assert.That(fixture.Attachments).IsEmpty();
            await Assert.That(fixture.Steps.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var paths = await CreateScreenFiles();

        try
        {
            await environment.RunAsync(async current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                await AllureInProcessApi.SetUpAsync(
                    "fixture",
                    (_, token) => AllureApi.AddScreenDiffFromFilesAsync(
                        paths.Expected,
                        paths.Actual,
                        paths.Diff,
                        token
                    ),
                    CancellationToken.None
                );
            });

            await Assert.That(scope.Befores.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();
        var paths = await CreateScreenFiles();

        try
        {
            environment.Run(current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                current.Runtime.LifecycleApi.StartTest(test);
                AllureInProcessApi.SetUp("fixture", _ =>
                    AllureApi.AddScreenDiffFromFiles(
                        paths.Expected,
                        paths.Actual,
                        paths.Diff
                    )
                );
            });

            await Assert.That(test.Attachments).IsEmpty();
            await Assert.That(scope.Befores.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();
        var paths = await CreateScreenFiles();

        try
        {
            await environment.RunAsync(async current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                current.Runtime.LifecycleApi.StartTest(test);
                await AllureInProcessApi.SetUpAsync(
                    "fixture",
                    (_, token) => AllureApi.AddScreenDiffFromFilesAsync(
                        paths.Expected,
                        paths.Actual,
                        paths.Diff,
                        token
                    ),
                    CancellationToken.None
                );
            });

            await Assert.That(test.Attachments).IsEmpty();
            await Assert.That(scope.Befores.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var paths = await CreateScreenFiles();

        try
        {
            await environment.RunAsync(async current =>
            {
                current.Runtime.LifecycleApi.StartTestScope(scope);
                await AllureInProcessApi.SetUpAsync(
                    "fixture",
                    (_, token) => AllureInProcessApi.StepAsync(
                        "step",
                        (_, stepToken) =>
                            AllureApi.AddScreenDiffFromFilesAsync(
                                paths.Expected,
                                paths.Actual,
                                paths.Diff,
                                stepToken
                            ),
                        token
                    ),
                    CancellationToken.None
                );
            });

            var fixture = scope.Befores.Single();
            await Assert.That(fixture.Attachments).IsEmpty();
            await Assert.That(fixture.Steps.Single().Attachments)
                .HasSingleItem();
        }
        finally
        {
            DeleteScreenFiles(paths);
        }
    }

    static async Task<ScreenFiles> CreateScreenFiles()
    {
        var prefix = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-screen-{Guid.NewGuid():N}"
        );
        var paths = new ScreenFiles(
            $"{prefix}-expected.png",
            $"{prefix}-actual.png",
            $"{prefix}-diff.png"
        );
        await File.WriteAllBytesAsync(paths.Expected, [1]);
        await File.WriteAllBytesAsync(paths.Actual, [2]);
        await File.WriteAllBytesAsync(paths.Diff, [3]);
        return paths;
    }

    static void DeleteScreenFiles(ScreenFiles paths)
    {
        File.Delete(paths.Expected);
        File.Delete(paths.Actual);
        File.Delete(paths.Diff);
    }

    static async Task AssertDiff(
        AllureApiTestEnvironment environment,
        AllureTestResult test
    )
    {
        var attachment = test.Attachments.Single();
        using var descriptor = JsonDocument.Parse(
            environment.Destination.ByteAttachments[attachment.Source]
        );
        await Assert.That(
            descriptor.RootElement.GetProperty("expected").GetString()
        ).IsEqualTo("data:image/png;base64,AQ==");
        await Assert.That(
            descriptor.RootElement.GetProperty("actual").GetString()
        ).IsEqualTo("data:image/png;base64,Ag==");
        await Assert.That(
            descriptor.RootElement.GetProperty("diff").GetString()
        ).IsEqualTo("data:image/png;base64,Aw==");
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };

    sealed record ScreenFiles(string Expected, string Actual, string Diff);
}
