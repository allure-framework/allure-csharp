using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Attachments;

public class AddAttachmentTests
{
    [Test]
    public async Task AddAttachmentWritesContentAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var content = new MemoryStream([1, 2, 3]);

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddAttachment(
                "attachment",
                content,
                "application/octet-stream",
                ".bin"
            );
        });

        var attachment = test.Attachments.Single();
        await Assert.That(attachment.Name).IsEqualTo("attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.Source).EndsWith(".bin");
        await Assert.That(
            environment.Destination.ByteAttachments[attachment.Source]
        ).IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task AddAttachmentAsyncWritesContentAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var content = new MemoryStream([1, 2, 3]);

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddAttachmentAsync(
                "attachment",
                content,
                "application/octet-stream",
                ".bin",
                CancellationToken.None
            );
        });

        var attachment = test.Attachments.Single();
        await Assert.That(
            environment.Destination.ByteAttachments[attachment.Source]
        ).IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task AddAttachmentAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
            {
                using var content = new MemoryStream([1]);
                AllureApi.AddAttachment("attachment", content);
            });
        });

        await Assert.That(scope.Befores.Single().Attachments)
            .HasSingleItem();
    }

    [Test]
    public async Task AddAttachmentPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
                AllureInProcessApi.Step("step", _ =>
                {
                    using var content = new MemoryStream([1]);
                    AllureApi.AddAttachment("attachment", content);
                })
            );
        });

        var fixture = scope.Befores.Single();
        await Assert.That(fixture.Attachments).IsEmpty();
        await Assert.That(fixture.Steps.Single().Attachments).HasSingleItem();
    }

    [Test]
    public async Task AddAttachmentAsyncAddsToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) =>
                {
                    using var content = new MemoryStream([1]);
                    await AllureApi.AddAttachmentAsync(
                        "attachment",
                        content,
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
    public async Task AddAttachmentPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.SetUp("fixture", _ =>
            {
                using var content = new MemoryStream([1]);
                AllureApi.AddAttachment("attachment", content);
            });
        });

        await Assert.That(test.Attachments).IsEmpty();
        await Assert.That(scope.Befores.Single().Attachments).HasSingleItem();
    }

    [Test]
    public async Task AddAttachmentAsyncPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) =>
                {
                    using var content = new MemoryStream([1]);
                    await AllureApi.AddAttachmentAsync(
                        "attachment",
                        content,
                        token
                    );
                },
                CancellationToken.None
            );
        });

        await Assert.That(test.Attachments).IsEmpty();
        await Assert.That(scope.Befores.Single().Attachments).HasSingleItem();
    }

    [Test]
    public async Task AddAttachmentAsyncPrioritizesCurrentStepOverFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) => await AllureInProcessApi.StepAsync(
                    "step",
                    async (_, stepToken) =>
                    {
                        using var content = new MemoryStream([1]);
                        await AllureApi.AddAttachmentAsync(
                            "attachment",
                            content,
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
    public async Task AddAttachmentThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var content = new MemoryStream([1]);

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddAttachment("attachment", content)
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddAttachmentAsyncThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var content = new MemoryStream([1]);

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddAttachmentAsync(
                "attachment",
                content,
                null,
                "",
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
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
