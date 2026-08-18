using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.TestingPlatform.Internal.Runtime;

sealed class AllureTestingPlatformSyncOperations(
    AllureTestingPlatformAsyncOperations asyncOperations
) :
    IAllureInProcessSyncOperations
{
    public void AddAttachment(string name, Stream content, string? mediaType, string fileExtension) =>
        asyncOperations.AddAttachmentAsync(name, content, mediaType, fileExtension, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddAttachmentFromFile(string name, string path, string? mediaType, string fileExtension) =>
        asyncOperations.AddAttachmentFromFileAsync(name, path, mediaType, fileExtension, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension) =>
        asyncOperations.AddGlobalAttachmentAsync(name, content, mediaType, fileExtension, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddGlobalAttachmentFromFile(string name, string path, string? mediaType, string fileExtension) =>
        asyncOperations.AddGlobalAttachmentFromFileAsync(name, path, mediaType, fileExtension, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddGlobalError(GlobalError error) =>
        asyncOperations.AddGlobalErrorAsync(error, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddLabel(Label label) =>
        asyncOperations.AddLabelAsync(label, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddLabels(IEnumerable<Label> labels) =>
        asyncOperations.AddLabelsAsync(labels, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddLink(Link link) =>
        asyncOperations.AddLinkAsync(link, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddLinks(IEnumerable<Link> links) =>
        asyncOperations.AddLinksAsync(links, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddScreenDiff(Stream expected, Stream actual, Stream diff) =>
        asyncOperations.AddScreenDiffAsync(expected, actual, diff, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddScreenDiffFromFiles(string expectedPath, string actualPath, string diffPath) =>
        asyncOperations.AddScreenDiffFromFilesAsync(expectedPath, actualPath, diffPath, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void AddTestParameter(Parameter parameter) =>
        asyncOperations.AddTestParameterAsync(parameter, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetDescription(string description) =>
        asyncOperations.SetDescriptionAsync(description, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetDescriptionHtml(string descriptionHtml) =>
        asyncOperations.SetDescriptionHtmlAsync(descriptionHtml, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetFixtureName(string newName) =>
        asyncOperations.SetFixtureNameAsync(newName, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetLabel(string name, string value) =>
        asyncOperations.SetLabelAsync(name, value, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetName(string newName) =>
        asyncOperations.SetNameAsync(newName, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetTestName(string newName) =>
        asyncOperations.SetTestNameAsync(newName, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncFixtureContext> body) =>
        asyncOperations.SetUpAsync(name, parameters, (ctx, _) =>
        {
            body(new AllureTestingPlatformSyncFixtureContext(ctx));
            return Task.CompletedTask;
        }, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessSyncFixtureContext, TResult> body) =>
        asyncOperations.SetUpAsync(name, parameters, (ctx, _) => Task.FromResult(body(new AllureTestingPlatformSyncFixtureContext(ctx))), default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails) =>
        asyncOperations.StepAsync(name, parameters, status, statusDetails, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void Step(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncStepContext> body) =>
        asyncOperations.StepAsync(name, parameters, (ctx, _) =>
        {
            body(new AllureTestingPlatformSyncStepContext(ctx));
            return Task.CompletedTask;
        }, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessSyncStepContext, TResult> body) =>
        asyncOperations.StepAsync(name, parameters, (ctx, _) => Task.FromResult(body(new AllureTestingPlatformSyncStepContext(ctx))), default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncFixtureContext> body) =>
        asyncOperations.TearDownAsync(name, parameters, (ctx, _) =>
        {
            body(new AllureTestingPlatformSyncFixtureContext(ctx));
            return Task.CompletedTask;
        }, default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessSyncFixtureContext, TResult> body) =>
        asyncOperations.TearDownAsync(name, parameters, (ctx, _) => Task.FromResult(body(new AllureTestingPlatformSyncFixtureContext(ctx))), default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, out TResult result)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, out TResult result)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public bool TryReadTestResult<TResult>(Func<TestResult, TResult> read, out TResult result)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateStepResult(Action<StepResult> update)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateTestResult(Action<TestResult> update)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }
}
