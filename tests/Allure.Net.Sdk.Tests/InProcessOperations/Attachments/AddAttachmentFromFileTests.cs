using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Attachments;

public class AddAttachmentFromFileTests
{
    [Test]
    public async Task AddAttachmentFromFileCopiesFileAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        const string path = "/input/report.json";

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddAttachmentFromFile(
                path,
                "report",
                "application/json",
                ".json"
            );
        });

        var attachment = test.Attachments.Single();
        await Assert.That(attachment.Name).IsEqualTo("report");
        await Assert.That(attachment.Type).IsEqualTo("application/json");
        await Assert.That(attachment.Source).EndsWith(".json");
        await Assert.That(
            environment.Destination.FileAttachments[attachment.Source]
        ).IsEqualTo(path);
    }

    [Test]
    public async Task AddAttachmentFromFileAsyncCopiesFileAndAddsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        const string path = "/input/report.json";

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddAttachmentFromFileAsync(
                path,
                "report",
                "application/json",
                ".json",
                CancellationToken.None
            );
        });

        var attachment = test.Attachments.Single();
        await Assert.That(
            environment.Destination.FileAttachments[attachment.Source]
        ).IsEqualTo(path);
    }

    [Test]
    public async Task AddAttachmentFromFileThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddAttachmentFromFile(
                "/input/report.json",
                "report",
                null,
                ".json"
            )
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddAttachmentFromFileAsyncThrowsIfNoExecutableItemRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddAttachmentFromFileAsync(
                "/input/report.json",
                "report",
                null,
                ".json",
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
