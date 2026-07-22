using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddTagsTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddTagsRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTags(["fast", "api"]);

        await Assert.That(
            endpoint.SyncApi.AddLabels(
                (labels) => labels.Select(label => (label.Name, label.Value))
                    .SequenceEqual([("tag", "fast"), ("tag", "api")])
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTagsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTags([]);
    }

    [Test]
    public async Task AddTagsAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTagsAsync([]);
    }

    [Test]
    public async Task AddTagsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTagsAsync([], CancellationToken.None);
    }
    [Test]
    public async Task AddTagsSupportsVarArgs()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTags("fast", "api");

        await Assert.That(
            endpoint.SyncApi.AddLabels(
                (labels) => labels.Select(label => (label.Name, label.Value))
                    .SequenceEqual([("tag", "fast"), ("tag", "api")])
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTagsAsyncRoutedToEndpoint()
    {
        IEnumerable<string> tags = ["fast", "api"];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTagsAsync(tags);

        await Assert.That(
            endpoint.AsyncApi.AddLabelsAsync(
                (labels) => labels.Select(label => (label.Name, label.Value))
                    .SequenceEqual([("tag", "fast"), ("tag", "api")]),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTagsAsyncSupportsVarArgs()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTagsAsync("fast", "api");

        await Assert.That(
            endpoint.AsyncApi.AddLabelsAsync(
                (labels) => labels.Select(label => (label.Name, label.Value))
                    .SequenceEqual([("tag", "fast"), ("tag", "api")]),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTagsAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTagsAsync("fast", "api");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTagsAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTagsAsync(["fast", "api"], cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLabelsAsync(
                (labels) => labels.Select(label => (label.Name, label.Value))
                    .SequenceEqual([("tag", "fast"), ("tag", "api")]),
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTagsAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTagsAsync(["fast", "api"], default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
