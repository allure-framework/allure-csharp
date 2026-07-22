using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddStoryTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddStoryRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddStory("foo");

        await Assert.That(
            endpoint.SyncApi.AddLabel(
                (label) => label.Name == "story" && label.Value == "foo"
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddStoryDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddStory("No endpoint value");
    }

    [Test]
    public async Task AddStoryAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddStoryAsync("No endpoint value");
    }

    [Test]
    public async Task AddStoryAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddStoryAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task AddStoryAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddStoryAsync("foo");

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "story" && label.Value == "foo",
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddStoryAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddStoryAsync("foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddStoryAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddStoryAsync("foo", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "story" && label.Value == "foo",
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddStoryAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddStoryAsync("foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
