using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

class SynchronizedInProcessTestApiAsync : IAllureInProcessTestApiAsync
{
    public Task AddAttachment(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddAttachment(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileAttachment(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddFileAttachment(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddFileScreenDiff(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddFileScreenDiff(expectedPath, actualPath, diffPath, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalAttachment(name, content, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalError(GlobalError error, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalError(error, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddGlobalFileAttachment(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddGlobalFileAttachment(name, path, mediaType, fileExtension, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabel(Label label, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLabel(label, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLabels(IEnumerable<Label> labels, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLabels(labels, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLink(Link link, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLink(link, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddLinks(IEnumerable<Link> links, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddLinks(links, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddScreenDiff(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddScreenDiff(expected, actual, diff, cancellationToken)
            ?? Task.CompletedTask;

    public Task AddTestParameter(Parameter parameter, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.AddTestParameter(parameter, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescription(string description, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetDescription(description, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetDescriptionHtml(string descriptionHtml, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetDescriptionHtml(descriptionHtml, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetFixtureName(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetFixtureName(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetLabel(string name, string value, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetLabel(name, value, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetName(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetName(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetTestName(string newName, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetTestName(newName, cancellationToken)
            ?? Task.CompletedTask;

    public Task SetUp(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            ?? body();

    public Task SetUp(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task SetUp(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, default);

    public Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.SetUp(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, default);

    public Task Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.Step(name, parameters, status, statusDetails, cancellationToken)
            ?? Task.CompletedTask;

    public Task Step(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.Step(name, parameters, body, cancellationToken)
            ?? body();

    public Task Step(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContextAsync, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.Step(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task Step(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContextAsync, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.Step(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, default);

    public Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.Step(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContextAsync, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.Step(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContextAsync, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.Step(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, default);

    public Task TearDown(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            ?? body();

    public Task TearDown(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public Task TearDown(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend?.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            ?? body(NullOperationContext.Instance, default);

    public Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            : body();

    public Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance);

    public Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContextAsync, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Async.TearDown(name, parameters, body, cancellationToken)
            : body(NullOperationContext.Instance, default);

    public static SynchronizedInProcessTestApiAsync Instance { get; } = new();
}