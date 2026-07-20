using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

class RoutingAllureAsyncOperations : IAllureAsyncInProcessOperations
{
    public Task AddAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddAttachmentAsync(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddFileAttachmentAsync(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileScreenDiffAsync(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddFileScreenDiffAsync(expectedPath, actualPath, diffPath, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalAttachmentAsync(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalErrorAsync(error, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalFileAttachmentAsync(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabelAsync(Label label, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLabelAsync(label, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLabelsAsync(labels, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLinkAsync(link, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLinksAsync(IEnumerable<Link> links, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLinksAsync(links, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddScreenDiffAsync(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddScreenDiffAsync(expected, actual, diff, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddTestParameterAsync(parameter, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescriptionAsync(string description, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetDescriptionAsync(description, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetDescriptionHtmlAsync(descriptionHtml, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetFixtureNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetLabelAsync(string name, string value, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetLabelAsync(name, value, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetTestNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetTestNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.StepAsync(name, parameters, status, statusDetails, cancellationToken)
            ?? Task.CompletedTask;

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.StepAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, default);

    public static RoutingAllureAsyncOperations Instance { get; } = new();
}