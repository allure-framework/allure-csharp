using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Links;

public class AddLinkTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddLinkByUrlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42");

        await Assert.That(endpoint.SyncApi.AddLink((link) =>
            link.Url is "https://example.test/reference/42"
                && link.Name is null
                && link.Type is null
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is null
                    && link.Type is null,
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is null
                    && link.Type is null,
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(
            "https://example.test/reference/42",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAndNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42", "Reference page");

        await Assert.That(endpoint.SyncApi.AddLink((link) =>
            link.Url is "https://example.test/reference/42"
                && link.Name is "Reference page"
                && link.Type is null
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is "Reference page"
                    && link.Type is null,
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is "Reference page"
                    && link.Type is null,
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(
            "https://example.test/reference/42",
            "Reference page",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(endpoint.SyncApi.AddLink((link) =>
            link.Url is "https://example.test/reference/42"
                && link.Name is "Reference page"
                && link.Type is "documentation"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is "Reference page"
                    && link.Type is "documentation",
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link.Url is "https://example.test/reference/42"
                    && link.Name is "Reference page"
                    && link.Type is "documentation",
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkModelRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink(link);

        await Assert.That(endpoint.SyncApi.AddLink((value) => ReferenceEquals(value, link)))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync(link);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (value) => ReferenceEquals(value, link),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncResultTaskForwardedToCaller()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(link);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkModelAsyncWithTokenRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync(link, cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (value) => ReferenceEquals(value, link),
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncWithTokenResultTaskForwardedToCaller()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(link, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
