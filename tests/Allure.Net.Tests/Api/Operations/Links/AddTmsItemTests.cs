using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Links;

public class AddTmsItemTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddTmsItemByUrlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTmsItem("https://tracker.example.test/case/42");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://tracker.example.test/case/42",
            Name: null,
            Type: LinkType.TmsItem,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTmsItemByUrlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTmsItem("https://tracker.example.test/no-endpoint");
    }

    [Test]
    public async Task AddTmsItemByUrlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTmsItemAsync("https://tracker.example.test/no-endpoint");
    }

    [Test]
    public async Task AddTmsItemByUrlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTmsItemAsync(
            "https://tracker.example.test/no-endpoint",
            cancellationToken: default
        );
    }

    [Test]
    public void AddTmsItemByUrlAndNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTmsItem(
            "https://tracker.example.test/no-endpoint",
            "Test case without endpoint"
        );
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTmsItemAsync(
            "https://tracker.example.test/no-endpoint",
            "Test case without endpoint"
        );
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTmsItemAsync(
            "https://tracker.example.test/no-endpoint",
            "Test case without endpoint",
            CancellationToken.None
        );
    }
    [Test]
    public async Task AddTmsItemByUrlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/case/42",
                    Name: null,
                    Type: LinkType.TmsItem,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTmsItemByUrlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTmsItemByUrlAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/case/42",
                    Name: null,
                    Type: LinkType.TmsItem,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTmsItemByUrlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTmsItemAsync(
            "https://tracker.example.test/case/42",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTmsItem("https://tracker.example.test/case/42", "Test case 42");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://tracker.example.test/case/42",
            Name: "Test case 42",
            Type: LinkType.TmsItem,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42", "Test case 42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/case/42",
                    Name: "Test case 42",
                    Type: LinkType.TmsItem,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42", "Test case 42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42", "Test case 42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/case/42",
                    Name: "Test case 42",
                    Type: LinkType.TmsItem,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTmsItemByUrlAndNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTmsItemAsync("https://tracker.example.test/case/42", "Test case 42", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

}
