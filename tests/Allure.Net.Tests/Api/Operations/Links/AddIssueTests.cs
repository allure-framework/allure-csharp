using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Links;

public class AddIssueTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddIssueByUrlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddIssue("https://tracker.example.test/issue/42");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://tracker.example.test/issue/42",
            Name: null,
            Type: LinkType.Issue,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddIssueByUrlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddIssue("https://tracker.example.test/no-endpoint");
    }

    [Test]
    public async Task AddIssueByUrlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddIssueAsync("https://tracker.example.test/no-endpoint");
    }

    [Test]
    public async Task AddIssueByUrlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddIssueAsync(
            "https://tracker.example.test/no-endpoint",
            cancellationToken: default
        );
    }

    [Test]
    public void AddIssueByUrlAndNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddIssue(
            "https://tracker.example.test/no-endpoint",
            "Issue without endpoint"
        );
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddIssueAsync(
            "https://tracker.example.test/no-endpoint",
            "Issue without endpoint"
        );
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddIssueAsync(
            "https://tracker.example.test/no-endpoint",
            "Issue without endpoint",
            CancellationToken.None
        );
    }
    [Test]
    public async Task AddIssueByUrlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddIssueAsync("https://tracker.example.test/issue/42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/issue/42",
                    Name: null,
                    Type: LinkType.Issue,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddIssueByUrlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddIssueAsync("https://tracker.example.test/issue/42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddIssueByUrlAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddIssueAsync("https://tracker.example.test/issue/42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/issue/42",
                    Name: null,
                    Type: LinkType.Issue,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddIssueByUrlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddIssueAsync(
            "https://tracker.example.test/issue/42",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddIssueByUrlAndNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddIssue("https://tracker.example.test/issue/42", "Issue 42");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://tracker.example.test/issue/42",
            Name: "Issue 42",
            Type: LinkType.Issue,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddIssueAsync("https://tracker.example.test/issue/42", "Issue 42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/issue/42",
                    Name: "Issue 42",
                    Type: LinkType.Issue,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddIssueAsync("https://tracker.example.test/issue/42", "Issue 42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddIssueAsync("https://tracker.example.test/issue/42", "Issue 42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://tracker.example.test/issue/42",
                    Name: "Issue 42",
                    Type: LinkType.Issue,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddIssueByUrlAndNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddIssueAsync("https://tracker.example.test/issue/42", "Issue 42", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
