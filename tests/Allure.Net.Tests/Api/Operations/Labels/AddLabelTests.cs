using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddLabelTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddLabelRoutedToEndpoint()
    {
        Label label = new(){ Name = "foo", Value = "bar" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLabel(label);

        await Assert.That(endpoint.SyncApi.AddLabel((v) => ReferenceEquals(v, label))).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddLabelModelDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLabel(new() { Name = "label-name", Value = "label-value" });
    }

    [Test]
    public async Task AddLabelModelAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelAsync(new() { Name = "label-name", Value = "label-value" });
    }

    [Test]
    public async Task AddLabelModelAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelAsync(
            new() { Name = "label-name", Value = "label-value" },
            CancellationToken.None
        );
    }

    [Test]
    public void AddLabelByNameAndValueDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLabel("label-name", "label-value");
    }

    [Test]
    public async Task AddLabelByNameAndValueAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelAsync("label-name", "label-value");
    }

    [Test]
    public async Task AddLabelByNameAndValueAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelAsync(
            "label-name",
            "label-value",
            CancellationToken.None
        );
    }
    [Test]
    public async Task AddLabelByNameValueRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLabel("foo", "bar");

        await Assert.That(endpoint.SyncApi.AddLabel((v) => v.Name == "foo" && v.Value == "bar")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelAsyncRoutedToEndpoint()
    {
        Label label = new(){ Name = "foo", Value = "bar" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelAsync(label);

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (v) => ReferenceEquals(v, label),
                CancellationToken.None))
            .WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelAsync(new(){ Name = "foo", Value = "bar" });

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLabelAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelAsync(new(){ Name = "foo", Value = "bar" }, ts.Token);

        await Assert.That(endpoint.AsyncApi.AddLabelAsync(Any(), ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelAsync(new(){ Name = "foo", Value = "bar" }, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLabelAsyncByNameValueRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelAsync("foo", "bar");

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (v) => v.Name == "foo" && v.Value == "bar",
                CancellationToken.None))
            .WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelAsyncByNameValueResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelAsync("foo", "bar");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLabelAsyncByNameValueWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelAsync("foo", "bar", ts.Token);

        await Assert.That(endpoint.AsyncApi.AddLabelAsync(Any(), ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelAsyncByNameValueWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelAsync("foo", "bar", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
