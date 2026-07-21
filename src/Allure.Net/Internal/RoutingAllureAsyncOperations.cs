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
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileScreenDiffAsync(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddFileScreenDiffAsync(expectedPath, actualPath, diffPath, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(error, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabelAsync(Label label, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddLabelAsync(label, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddLabelsAsync(labels, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddLinkAsync(link, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLinksAsync(IEnumerable<Link> links, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddLinksAsync(links, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddScreenDiffAsync(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(expected, actual, diff, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.AddTestParameterAsync(parameter, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescriptionAsync(string description, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetDescriptionAsync(description, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetDescriptionHtmlAsync(descriptionHtml, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetFixtureNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetLabelAsync(string name, string value, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(name, value, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetTestNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetTestNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.SetUpAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.StepAsync(name, parameters, status, statusDetails, cancellationToken)
            ?? Task.CompletedTask;

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessStepContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.StepAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body();

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    public static RoutingAllureAsyncOperations Instance { get; } = new();
}