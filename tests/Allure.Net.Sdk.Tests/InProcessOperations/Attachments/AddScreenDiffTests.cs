using System.Text.Json;
using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Attachments;

public class AddScreenDiffTests
{
    [Test]
    public async Task AddScreenDiffWritesDescriptorAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddScreenDiff(expected, actual, diff);
        });

        await AssertDiff(environment, test);
    }

    [Test]
    public async Task AddScreenDiffAsyncWritesDescriptorAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddScreenDiffAsync(
                expected,
                actual,
                diff,
                CancellationToken.None
            );
        });

        await AssertDiff(environment, test);
    }

    [Test]
    public async Task AddScreenDiffAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
            {
                using var expected = new MemoryStream([1]);
                using var actual = new MemoryStream([2]);
                using var diff = new MemoryStream([3]);
                AllureApi.AddScreenDiff(expected, actual, diff);
            });
        });

        await Assert.That(scope.Befores.Single().Attachments)
            .HasSingleItem();
    }

    [Test]
    public async Task AddScreenDiffPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
                AllureInProcessApi.Step("step", _ =>
                {
                    using var expected = new MemoryStream([1]);
                    using var actual = new MemoryStream([2]);
                    using var diff = new MemoryStream([3]);
                    AllureApi.AddScreenDiff(expected, actual, diff);
                })
            );
        });

        var fixture = scope.Befores.Single();
        await Assert.That(fixture.Attachments).IsEmpty();
        await Assert.That(fixture.Steps.Single().Attachments).HasSingleItem();
    }

    [Test]
    public async Task AddScreenDiffAsyncAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) =>
                {
                    using var expected = new MemoryStream([1]);
                    using var actual = new MemoryStream([2]);
                    using var diff = new MemoryStream([3]);
                    await AllureApi.AddScreenDiffAsync(
                        expected,
                        actual,
                        diff,
                        token
                    );
                },
                CancellationToken.None
            );
        });

        await Assert.That(scope.Befores.Single().Attachments)
            .HasSingleItem();
    }

    [Test]
    public async Task AddScreenDiffAsyncPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) => await AllureInProcessApi.StepAsync(
                    "step",
                    async (_, stepToken) =>
                    {
                        using var expected = new MemoryStream([1]);
                        using var actual = new MemoryStream([2]);
                        using var diff = new MemoryStream([3]);
                        await AllureApi.AddScreenDiffAsync(
                            expected,
                            actual,
                            diff,
                            stepToken
                        );
                    },
                    token
                ),
                CancellationToken.None
            );
        });

        var fixture = scope.Befores.Single();
        await Assert.That(fixture.Attachments).IsEmpty();
        await Assert.That(fixture.Steps.Single().Attachments).HasSingleItem();
    }

    [Test]
    public async Task AddScreenDiffThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddScreenDiff(expected, actual, diff)
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddScreenDiffAsyncThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddScreenDiffAsync(
                expected,
                actual,
                diff,
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static async Task AssertDiff(
        AllureApiTestEnvironment environment,
        AllureTestResult test
    )
    {
        var attachment = test.Attachments.Single();
        await Assert.That(attachment.Name).IsEqualTo("diff-1");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/vnd.allure.image.diff");
        await Assert.That(attachment.Source).EndsWith(".json");

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
}
